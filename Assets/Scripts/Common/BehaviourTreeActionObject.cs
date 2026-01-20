using UnityEngine;

public abstract class BehaviourTreeActionObject : ScriptableObject
{
    public abstract LunarCube.BehaviourTree.INode.State Invoke(BehaviourTreeRunner runner);
}
