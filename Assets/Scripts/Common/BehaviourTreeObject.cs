using UnityEngine;

[CreateAssetMenu(fileName = "NewBehaviourTree", menuName = "Game/Behaviour Tree/Behaviour Tree Object", order = int.MinValue)]
public class BehaviourTreeObject : ScriptableObject
{
    [SerializeReference] public LunarCube.BehaviourTree.Root root;
}
