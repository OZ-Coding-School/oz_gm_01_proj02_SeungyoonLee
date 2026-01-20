using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunarCube.GameManager
{
    [Serializable]
    public class SceneServiceConfig : ServiceConfig<SceneService>
    {
        [field: SerializeField] public string LoadingScreenName { get; set; }
    }

    public class SceneService : MonoBehaviour, IService
    {
        private string loadingScreenName;

        public void Configure(IServiceConfig iConfig)
        {
            SceneServiceConfig config = iConfig as SceneServiceConfig;
            if (!string.IsNullOrEmpty(config.LoadingScreenName)) loadingScreenName = config.LoadingScreenName;
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
