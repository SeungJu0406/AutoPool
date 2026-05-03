using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Static helper for generic (non-GameObject) object pooling.
    /// </summary>
    public static class GenericPool
    {
        /// <summary>
        /// Retrieves a pooled instance of type <typeparamref name="T"/> from the generic pool.
        /// </summary>
        public static T Get<T>() where T : class, IPoolGeneric, new()
        {
            return ObjectPool.GenericPool<T>();
        }

        /// <summary>
        /// Returns an instance to the generic pool and returns the pool info.
        /// </summary>
        public static IGenericPoolInfoReadOnly Return<T>(T instance) where T : class, IPoolGeneric, new()
        {
            return ObjectPool.ReturnGeneric(instance);
        }
    }
}
