using UnityEngine;

namespace AutoPool_Tool
{
    public static class GenericPool
    {
        public static T Get<T>() where T : class, IPoolGeneric, new()
        {
            return ObjectPool.GenericPool<T>();
        }

        public static IGenericPoolInfoReadOnly Return<T>(T instance) where T : class, IPoolGeneric, new()
        {
            return ObjectPool.ReturnGeneric(instance);
        }
    }
}