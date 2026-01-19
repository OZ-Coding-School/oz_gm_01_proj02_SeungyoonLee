using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    [field: SerializeField] public LunarCube.BehaviourTree.INode Root { get; private set; }

    private void FixedUpdate()
    {
        Tick();
    }

    private LunarCube.BehaviourTree.INode.State Tick()
    {
        if (null != Root) return Root.Tick();
        else return LunarCube.BehaviourTree.INode.State.Failure;
    }
}
