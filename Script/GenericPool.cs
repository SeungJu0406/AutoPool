using UnityEngine;

namespace AutoPool
{
    public static class GenericPool
    {
        public static T Get<T>() where T : class, IPoolGeneric, new()
        {
            return AutoPool.GenericPool<T>();
        }

        public static IGenericPoolInfoReadOnly Return<T>(T instance) where T : class, IPoolGeneric, new()
        {
            return AutoPool.ReturnGeneric(instance);
        }
    }
}