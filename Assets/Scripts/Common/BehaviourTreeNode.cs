using Lua;
using Lua.Unity;
using LunarCube.GameManager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LunarCube.BehaviourTree
{

    public interface INode
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        public void Initialize();
        public State Tick();
    }

    [Serializable]
    public class Root : INode
    {
        [SerializeField] private INode node;

        public void Initialize()
        {
            node.Initialize();
        }

        public INode.State Tick()
        {
            return node.Tick();
        }
    }

    [Serializable]
    public abstract class Compositor : INode
    {
        [SerializeField] protected List<INode> nodes;

        public void Initialize()
        {
            foreach (INode node in nodes) node.Initialize();
        }

        public abstract INode.State Tick();
    }

    [Serializable]
    public class SuccessAny : Compositor
    {
        public override INode.State Tick()
        {
            foreach (INode node in nodes)
            {
                INode.State state = node.Tick();
                if (INode.State.Failure != state) return state;
            }
            return INode.State.Failure;
        }
    }

    [Serializable]
    public class SuccessAll : Compositor
    {
        public override INode.State Tick()
        {
            foreach (INode node in nodes)
            {
                INode.State state = node.Tick();
                if (INode.State.Success != state) return state;
            }
            return INode.State.Success;
        }
    }

    [Serializable]
    public class RandomPick : Compositor
    {
        Unity.Mathematics.Random random = new Unity.Mathematics.Random((uint)System.DateTime.Now.Ticks);
        public override INode.State Tick()
        {
            int index = random.NextInt(0, nodes.Count - 1);
            return nodes[index].Tick();
        }
    }

    [Serializable]
    public abstract class Modifier : INode
    {
        [SerializeField] protected INode node;

        public void Initialize()
        {
            node.Initialize();
        }

        public abstract INode.State Tick();
    }

    [Serializable]
    public class Inverter : Modifier
    {
        public override INode.State Tick()
        {
            return node.Tick() switch
            {
                INode.State.Failure => INode.State.Success,
                INode.State.Running => INode.State.Running,
                _                   => INode.State.Failure,
            };
        }
    }

    [Serializable]
    public class Succeeder : Modifier
    {
        public override INode.State Tick()
        {
            node.Tick();
            return INode.State.Success;
        }
    }

    [Serializable]
    public class WaitSucceeder : Modifier
    {
        public override INode.State Tick()
        {
            return node.Tick() switch
            {
                INode.State.Running => INode.State.Running,
                _                   => INode.State.Success,
            };
        }
    }

    [Serializable]
    public class Failer : Modifier
    {
        public override INode.State Tick()
        {
            node.Tick();
            return INode.State.Failure;
        }
    }

    [Serializable]
    public class WaitFailer : Modifier
    {
        public override INode.State Tick()
        {
            return node.Tick() switch
            {
                INode.State.Running => INode.State.Running,
                _                   => INode.State.Failure,
            };
        }
    }

    [Serializable]
    public class Repeater : Modifier
    {
        [field: SerializeField] public int Count { get; private set; }
        public override INode.State Tick()
        {
            return node.Tick() switch
            {
                INode.State.Running => INode.State.Running,
                _                   => INode.State.Failure,
            };
        }
    }

    [Serializable]
    public class Evaluator : INode
    {
        [field: SerializeField] public LuaAsset Script { get; private set; }
        LuaFunction func; // TODO: currently no arguments expected. need change

        public void Initialize()
        {
            LuaState state = GameManager.GameManager.Instance.GetService<LuaRunnerService>().State;
            var results = state.DoFileAsync("lua2cs.lua").Result;
            if (1 == results.Length && results[0].TryRead(out LuaFunction returnVal)) func = returnVal;
        }

        public INode.State Tick()
        {
            if (null == func) return INode.State.Failure;
            LuaState state = GameManager.GameManager.Instance.GetService<LuaRunnerService>().State;
            var results = state.CallAsync(func, ReadOnlySpan<LuaValue>.Empty).Result;
            if (1 == results.Length && results[0].TryRead(out int returnVal))
            {
                return returnVal switch
                {
                    0 => INode.State.Success,
                    1 => INode.State.Running,
                    _ => INode.State.Failure,
                };
            }
            else return INode.State.Failure;
        }
    }


}
