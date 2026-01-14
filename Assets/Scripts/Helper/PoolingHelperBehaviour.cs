using LunarCube;
using System;
using UnityEngine;

public class PoolingHelperBehaviour : MonoBehaviour
{
    [field: SerializeField] public SerializableNullable<int> ModifiedInitialCount { get; set; }
    [field: SerializeField] public SerializableNullable<int> ModifiedCapacity { get; set; }

    private void Awake()
    {
#if UNITY_EDITOR
        if (
            ModifiedInitialCount.HasValue
            && ModifiedCapacity.HasValue
            && ModifiedCapacity < ModifiedInitialCount)
        {
            Debug.LogWarning($"ModifiedInitialCount is bigger than ModifiedCapacity. ({ModifiedCapacity} < {ModifiedInitialCount})");
        }
#endif
        Destroy(this);
    }
}
