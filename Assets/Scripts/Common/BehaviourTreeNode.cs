using System;
using System.Collections.Generic;
using System.Linq;
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

        public void Initialize(BehaviourTreeRunner runner);
        public State Tick();
    }

    [Serializable]
    public class Root : INode
    {
        [SerializeReference] private INode node;

        public void Initialize(BehaviourTreeRunner runner)
        {
            node.Initialize(runner);
        }

        public INode.State Tick()
        {
            return node?.Tick() ?? INode.State.Failure;
        }
    }

    [Serializable]
    public abstract class Compositor : INode
    {
        [SerializeReference] protected List<INode> nodes;

        public virtual void Initialize(BehaviourTreeRunner runner)
        {
            foreach (INode node in nodes ?? Enumerable.Empty<INode>()) node.Initialize(runner);
        }

        public abstract INode.State Tick();
    }

    [Serializable]
    public class SuccessAny : Compositor
    {
        public override INode.State Tick()
        {
            foreach (INode node in nodes ?? Enumerable.Empty<INode>())
            {
                INode.State state = node?.Tick() ?? INode.State.Failure;
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
            foreach (INode node in nodes ?? Enumerable.Empty<INode>())
            {
                INode.State state = node?.Tick() ?? INode.State.Failure;
                if (INode.State.Success != state) return state;
            }
            return INode.State.Success;
        }
    }

    [Serializable]
    public class RandomPick : Compositor
    {
        Unity.Mathematics.Random random;
        public override void Initialize(BehaviourTreeRunner runner)
        {
            base.Initialize(runner);
            random = new Unity.Mathematics.Random((uint)DateTime.Now.Ticks);
        }
        public override INode.State Tick()
        {
            if (null == nodes) return INode.State.Failure;

            int index = random.NextInt(0, nodes.Count - 1);
            return nodes[index]?.Tick() ?? INode.State.Failure;
        }
    }

    [Serializable]
    public abstract class Modifier : INode
    {
        [SerializeReference] protected INode node;

        public virtual void Initialize(BehaviourTreeRunner runner)
        {
            node.Initialize(runner);
        }

        public abstract INode.State Tick();
    }

    [Serializable]
    public class Inverter : Modifier
    {
        public override INode.State Tick()
        {
            if (null == node) return INode.State.Failure;

            else return node.Tick() switch
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
            if (null == node) return INode.State.Failure;

            node.Tick();
            return INode.State.Success;
        }
    }

    [Serializable]
    public class WaitSucceeder : Modifier
    {
        public override INode.State Tick()
        {
            if (null == node) return INode.State.Failure;

            else return node.Tick() switch
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
            if (null == node) return INode.State.Failure;

            node.Tick();
            return INode.State.Failure;
        }
    }

    [Serializable]
    public class WaitFailer : Modifier
    {
        public override INode.State Tick()
        {
            if (null == node) return INode.State.Failure;

            else return node.Tick() switch
            {
                INode.State.Running => INode.State.Running,
                _                   => INode.State.Failure,
            };
        }
    }

    [Serializable]
    public class Repeater : Modifier
    {
        [field: Min(1)] [field: SerializeField] public int Count { get; private set; }
        private int iteration;
        private bool isEverFailed;
        public override void Initialize(BehaviourTreeRunner runner)
        {
            base.Initialize(runner);
            iteration = 0;
            isEverFailed = false;
        }

        public override INode.State Tick()
        {
            if (null == node) return INode.State.Failure;
            if (Count <= 0) return INode.State.Success;

            INode.State state = node.Tick();
            if (state == INode.State.Failure) isEverFailed = true;

            switch (state)
            {
                case INode.State.Running:
                    return INode.State.Running;
                case INode.State.Success:
                case INode.State.Failure:
                    iteration++;
                    if (iteration >= Count)
                    {
                        iteration = 0;
                        return isEverFailed ? INode.State.Failure : INode.State.Success;
                    }
                    else return INode.State.Running;
                default:
                    return INode.State.Failure;
            }
        }
    }

    [Serializable]
    public class ConditionalRepeater : Modifier
    {
        [Tooltip("'Conditional Node' would be executed when the 'Node' returned success.")]
        [SerializeReference] private INode conditionalNode;

        public override INode.State Tick()
        {
            INode.State state = node?.Tick() ?? INode.State.Failure;
            switch (state)
            {
                case INode.State.Running:
                    return INode.State.Running;
                case INode.State.Success:
                    return conditionalNode?.Tick() ?? INode.State.Failure;
                case INode.State.Failure:
                    return INode.State.Success;
                default:
                    return INode.State.Failure;
            }
        }
    }

    [Serializable]
    public class NestedTree : INode
    {
        [field: SerializeField] private BehaviourTreeObject Nested;
        public void Initialize(BehaviourTreeRunner runner)
        {
            Nested?.root?.Initialize(runner);
        }

        public INode.State Tick()
        {
            return Nested?.root?.Tick() ?? INode.State.Failure;
        }
    }

    [Serializable]
    public class Leaf : INode
    {
        [field: SerializeField] private BehaviourTreeActionObject actionObject;
        BehaviourTreeRunner runner;
        public void Initialize(BehaviourTreeRunner runner)
        {
            this.runner = runner;
        }

        public INode.State Tick()
        {
            return actionObject?.Invoke(runner) ?? INode.State.Failure;
        }
    }


}
