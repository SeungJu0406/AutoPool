using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Static helper that exposes get and return operations for generic (non-GameObject) pools.
    /// </summary>
    public static class GenericPool
    {
        /// <summary>
        /// Retrieves an instance of <typeparamref name="T"/> from the generic pool.
        /// </summary>
        public static T Get<T>() where T : class, IPoolGeneric, new()
        {
            return ObjectPool.GenericPool<T>();
        }

        /// <summary>
        /// Returns an instance to the generic pool and provides updated pool info.
        /// </summary>
        public static IGenericPoolInfoReadOnly Return<T>(T instance) where T : class, IPoolGeneric, new()
        {
            return ObjectPool.ReturnGeneric(instance);
        }
    }
}
