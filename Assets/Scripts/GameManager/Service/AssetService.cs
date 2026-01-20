using System;
using UnityEngine;

namespace LunarCube.GameManager
{
    [Serializable]
    public class AssetServiceConfig : ServiceConfig<AssetService>
    {

    }

    public class AssetService : MonoBehaviour, IService
    {

        public void Configure(IServiceConfig iConfig)
        {

        }


    }
}
