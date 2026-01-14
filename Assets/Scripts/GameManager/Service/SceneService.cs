using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunarCube.GameManager
{
    public class SceneService : MonoBehaviour, IService
    {
        [Serializable]
        public class SceneServiceConfig : ServiceConfig<SceneService>
        {
            [field: SerializeField] public string LoadScreenName { get; set; }
        }

        private string loadScreenName;

        public void Configure(IServiceConfig iConfig)
        {
            SceneServiceConfig config = iConfig as SceneServiceConfig;
            if (!string.IsNullOrEmpty(config.LoadScreenName)) loadScreenName = config.LoadScreenName;
        }

        //public bool LoadSceneSynchronized(string sceneName) => LoadScene(sceneName, false);

        //public bool LoadSceneSynchronized(string sceneName, bool isAdditive)
        //{
        //    LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        //    SceneManager.LoadScene(sceneName, mode);
        //}

        //public bool LoadScene(string sceneName, bool isAdditive, LocalPhysicsMode physicsMode)
        //{
        //    LoadSceneMode mode = isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        //    SceneManager.LoadScene(sceneName, mode);
        //}
    }
}
