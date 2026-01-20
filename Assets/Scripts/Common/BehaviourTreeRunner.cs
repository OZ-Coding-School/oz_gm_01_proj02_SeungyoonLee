using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    [field: SerializeField] public BehaviourTreeObject target { get; private set; }

    private void OnEnable()
    {
        target.root.Initialize(this);
    }

    private void FixedUpdate()
    {
        Tick();
    }

    private LunarCube.BehaviourTree.INode.State Tick()
    {
        if (null != target.root) return target.root.Tick();
        else return LunarCube.BehaviourTree.INode.State.Failure;
    }
}
