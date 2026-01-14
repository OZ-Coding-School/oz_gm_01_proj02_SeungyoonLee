using System;
using UnityEngine;

namespace LunarCube.GameManager
{
    public interface IServiceConfig
    {
        public enum RequiredStatus
        {
            NotRequired,
            NotRequired_ResetIfAvailable,
            Required,
            Required_ResetAlways,
            Shutdown
        }

        public RequiredStatus Requirement { get; }

        public Type ServiceType { get; }
    }

    [Serializable]
    public abstract class ServiceConfig<T> : IServiceConfig where T : IService
    {
        [field: SerializeField] public IServiceConfig.RequiredStatus Requirement { get; private set; }

        public Type ServiceType => typeof(T);
    }
}
