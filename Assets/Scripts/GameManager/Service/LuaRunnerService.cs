using Lua;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LunarCube.GameManager
{
    [Serializable]
    public class LuaRunnerServiceConfig : ServiceConfig<LuaRunnerService>
    {
        //[field: SerializeField] public SerializableNullable<int> MaxJobCount { get; private set; }
    }

    public class LuaRunnerService : MonoBehaviour, IService
    {
        // NOTE: currently working in single thread

        //private const int HardLimitMaxJobCount = 128;

        //public int MaxJobCount { get; private set; }

        private List<LuaState> luaStates = new List<LuaState>();

        public void Configure(IServiceConfig iConfig)
        {
            //LuaRunnerServiceConfig config = iConfig as LuaRunnerServiceConfig;
            //MaxJobCount = Mathf.Clamp(
            //    config.MaxJobCount.GetValueOrDefault(SystemInfo.processorCount),
            //    1,
            //    HardLimitMaxJobCount);


            luaStates.Clear();
            luaStates.Add(LuaState.Create());
        }

        public LuaState State
        {
            get { return luaStates[0]; }
        }
    }
}
