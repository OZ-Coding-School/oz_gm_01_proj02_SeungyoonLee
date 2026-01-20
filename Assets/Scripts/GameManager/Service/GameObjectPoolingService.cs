using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace LunarCube.GameManager
{
    [Serializable]
    public class GameObjectPoolingServiceConfig : ServiceConfig<GameObjectPoolingService>
    {
        [field: SerializeField] public SerializableNullable<int> DefaultInitialCount { get; private set; }
        [field: SerializeField] public SerializableNullable<int> DefaultCapacity { get; private set; }
    }

    public class GameObjectPoolingService : MonoBehaviour, IService
    {
        public int DefaultInitialCount { get; private set; }
        public int DefaultCapacity { get; private set; }

        public void Configure(IServiceConfig iConfig)
        {
            GameObjectPoolingServiceConfig config = iConfig as GameObjectPoolingServiceConfig;
            if (config.DefaultInitialCount.HasValue)    DefaultInitialCount = config.DefaultInitialCount.Value;
            if (config.DefaultCapacity.HasValue)        DefaultCapacity     = config.DefaultCapacity.Value;
        }

        private GameObject pooler;

        /// <summary>
        /// Dictionary of prefabs to their capacities.
        /// </summary>
        private Dictionary<GameObject, int> capacities;

        /// <summary>
        /// Map from tracked GameObject instances to their prefabs.
        /// </summary>
        private Dictionary<GameObject, GameObject> trackingMap;

        /// <summary>
        /// Map from prefabs to stacks for their pooled instances available to use.
        /// </summary>
        private Dictionary<GameObject, Stack<GameObject>> poolMap;

        private void OnEnable()
        {
            pooler = new GameObject("Pooler");
            pooler.SetActive(false);
            DontDestroyOnLoad(pooler);

            capacities = new Dictionary<GameObject, int>();
            trackingMap = new Dictionary<GameObject, GameObject>();
            poolMap = new Dictionary<GameObject, Stack<GameObject>>();
        }

        private void OnDisable()
        {
            capacities = null;
            poolMap = null;
            trackingMap = null;

            Destroy(pooler);
        }

        private void OnSceneLoadStart(Scene scene, LoadSceneMode mode)
        {
            if (LoadSceneMode.Single == mode)
            {
                RetrievePooledGameObjects();
                MovePooledObjectsToScene(scene);
            }
        }

        private void OnSceneUnloadComplete(Scene scene, LoadSceneMode mode)
        {
            if (LoadSceneMode.Single == mode)
            {
                RetrievePooledGameObjects();
                MovePooledObjectsToScene(scene);
            }
        }

        private void RetrievePooledGameObjects()
        {
            foreach (GameObject obj in trackingMap.Keys)
            {
                if (null != obj.transform.parent && obj.transform.parent.gameObject == pooler) continue;
                ReturnOrDestroyGameObject(obj);
            }
        }

        private void MovePooledObjectsToScene(Scene scene)
        {
            SceneManager.MoveGameObjectToScene(pooler, scene);
        }

        public GameObject GetOrCreateInactivatedGameObject(GameObject prefab)
        {
            Stack<GameObject> pool = GetOrCreatePool(prefab);
            if (null == pool) return null;
            if (!pool.TryPop(out GameObject result) || null == result)
            {
                result = Instantiate(prefab, pooler.transform);
                if (null == result) return null;
                trackingMap[result] = prefab;
            }
            result.SetActive(false);
            Scene activeScene = SceneManager.GetActiveScene();
            if (result.scene == activeScene) result.transform.SetParent(null);
            else if (null != activeScene) SceneManager.MoveGameObjectToScene(result, activeScene);
            else throw new InvalidOperationException("Active Scene is null.");
            return result;
        }

        public void ReturnOrDestroyGameObject(GameObject obj)
        {
            if (null == obj) return;

            if (!trackingMap.TryGetValue(obj, out GameObject prefab)
                || !poolMap.TryGetValue(prefab, out Stack<GameObject> pool)
                || capacities.GetValueOrDefault(prefab, DefaultCapacity) <= pool.Count)
            {
                trackingMap.Remove(obj);
                Destroy(obj);
                return;
            }

            if (!pool.Contains(obj))
            {
                pool.Push(obj);
                obj.transform.SetParent(pooler.transform);
            }
        }

        public void Shrink()
        {
            foreach (GameObject prefab in poolMap.Keys) ShrinkPool(prefab);
        }

        public void ShrinkPool(GameObject prefab)
        {
            if (!poolMap.TryGetValue(prefab, out Stack<GameObject> pool)) return;
            while (pool.Count > 0)
            {
                GameObject obj = pool.Pop();
                trackingMap.Remove(obj);
                Destroy(obj);
            }
        }

        private Stack<GameObject> GetOrCreatePool(GameObject prefab)
        {
            if (null == prefab) return null;
            if (!poolMap.TryGetValue(prefab, out Stack<GameObject> result))
            {
                result = new Stack<GameObject>();
                PoolingHelperBehaviour helper = prefab.GetComponent<PoolingHelperBehaviour>();
                int initialCount = (null != helper) ? helper.ModifiedInitialCount : DefaultInitialCount;
                int capacity = (null != helper) ? helper.ModifiedCapacity : DefaultCapacity;

                if (null == result
                    || !poolMap.TryAdd(prefab, result)
                    || !capacities.TryAdd(prefab, initialCount))
                {
                    poolMap.Remove(prefab);
                    return null;
                }

                for (int i = 0; i < capacity; i++)
                {
                    GameObject obj = Instantiate(prefab, pooler.transform);
                    trackingMap[obj] = prefab;
                    result.Push(obj);
                }
            }
            return result;
        }
    }
}
