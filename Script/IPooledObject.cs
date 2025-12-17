using UnityEngine;

namespace AutoPool_Tool
{

    public interface IPooledObject
    {
        void OnCreateFromPool();

        void OnReturnToPool();
    }

    public interface  IPoolGeneric
    {
        PoolGenericInfo Pool { get; set; }

        void OnCreateFromPool();

        void OnReturnToPool();
    }

    public class PoolGenericInfo
    {
        public GenericPoolInfo PoolInfo;
        public bool IsActive;
        public event System.Action OnReturn;

        public void Return()
        {
            OnReturn?.Invoke();
        }
    }
}