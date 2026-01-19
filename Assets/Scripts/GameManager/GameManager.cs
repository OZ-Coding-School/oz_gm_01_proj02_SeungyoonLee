using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LunarCube.GameManager
{
    public class GameManager : MonoBehaviour
    {
        // TODO: manage event system

        public static GameManager Instance { get; private set; }

        [field: SerializeReference] public List<IServiceConfig> Configs { get; set; } = new List<IServiceConfig>();
        private readonly Dictionary<Type, IService> regesteredServices = new Dictionary<Type, IService>();

        private void Awake()
        {
            if (null != Instance && this != Instance)
            {
                ForwardConfig(Configs);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // TODO: event system
        }

        private static IService InitializeService(IServiceConfig config)
        {
            IService service = Instance.gameObject.AddComponent(config.ServiceType) as IService;
            service?.Configure(config);
            return service;
        }

        private static void TerminateService(IService service)
        {
            if (service is MonoBehaviour) Destroy(service as MonoBehaviour);
        }

        private static void ForwardConfig(List<IServiceConfig> Configs)
        {
            foreach (IServiceConfig config in Configs)
            {
                Type t = config.ServiceType;
                if (!Instance.regesteredServices.TryGetValue(t, out IService service)) service = null;
                switch (config.Requirement)
                {
                    case IServiceConfig.RequiredStatus.NotRequired_ResetIfAvailable:
                        service?.Configure(config);
                        break;
                    case IServiceConfig.RequiredStatus.Required:
                        if (null == service) Instance.regesteredServices.Add(t, InitializeService(config));
                        break;
                    case IServiceConfig.RequiredStatus.Required_ResetAlways:
                        if (null == service) Instance.regesteredServices.Add(t, InitializeService(config));
                        else service.Configure(config);
                        break;
                    case IServiceConfig.RequiredStatus.Shutdown:
                        TerminateService(service);
                        break;
                }
            }
        }

        public T GetService<T>() where T : MonoBehaviour, IService
        {
            if (!regesteredServices.TryGetValue(typeof(T), out IService iService)) iService = null;
            return iService as T;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Dictionary<Type, bool> configFoundFlags =
                TypeCache.GetTypesDerivedFrom<IServiceConfig>()
                    .Where(t => !t.IsAbstract)
                    .ToDictionary(t => t, t => false);

            Configs.RemoveAll(config => null == config);

            foreach (IServiceConfig config in Configs)
            {
                Type configType = config.GetType();
                if (configFoundFlags.ContainsKey(configType)) configFoundFlags[configType] = true;
            }

            foreach (KeyValuePair<Type, bool> kv in configFoundFlags)
            {
                if (null == kv.Key || kv.Value) continue;
                if (typeof(IServiceConfig).IsAssignableFrom(kv.Key))
                {
                    IServiceConfig config = Activator.CreateInstance(kv.Key) as IServiceConfig;
                    if (null != config) Configs.Add(config);
                }
            }

            Configs.Sort((left, right) => left.ServiceType.FullName.CompareTo(right.ServiceType.FullName));
        }
#endif
    }
}
