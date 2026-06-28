using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Scene-wide static facade for all pooling operations.
    /// Automatically creates the underlying MainAutoPool when first accessed.
    /// </summary>
    public static class ObjectPool
    {
        /// <summary>
        /// The shared pool manager instance. Created on first access.
        /// </summary>
        public static MainAutoPool Instance
        {
            get
            {
                if (s_objectPool == null)
                    CreatePool();
                return s_objectPool;
            }
        }

        private static MainAutoPool s_objectPool;

        /// <summary>
        /// True when the pool manager exists and has not been destroyed.
        /// </summary>
        public static bool HasPool => s_objectPool != null && !s_objectPool.Equals(null);

        /// <summary>Returns pool info for the given prefab.</summary>
        public static IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        /// <summary>Returns pool info for the given Component prefab.</summary>
        public static IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        /// <summary>Pre-warms the prefab pool to at least <paramref name="count"/> instances.</summary>
        public static IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>Pre-warms the Component prefab pool to at least <paramref name="count"/> instances.</summary>
        public static IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>Destroys all pooled instances for the given prefab.</summary>
        public static IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>Destroys all pooled instances for the given Component prefab.</summary>
        public static IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>Retrieves a GameObject instance from the pool.</summary>
        public static GameObject Get(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>Retrieves a GameObject instance and parents it under <paramref name="transform"/>.</summary>
        public static GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = default)
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }

        /// <summary>Retrieves a GameObject instance at the given position and rotation.</summary>
        public static GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        /// <summary>Retrieves a Component instance from the pool.</summary>
        public static T Get<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>Retrieves a Component instance and parents it under <paramref name="transform"/>.</summary>
        public static T Get<T>(T prefab, Transform transform, bool worldPositionStay = default) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }

        /// <summary>Retrieves a Component instance at the given position and rotation.</summary>
        public static T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        /// <summary>Retrieves a Resources-path Component instance at the given position and rotation.</summary>
        public static T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(resouces, pos, rot);
        }

        /// <summary>Retrieves a generic pool instance of type <typeparamref name="T"/>.</summary>
        public static T GenericPool<T>() where T : class, IPoolGeneric, new()
        {
            CreatePool();
            return s_objectPool.GenericPool<T>();
        }

        /// <summary>
        /// Returns a GameObject to the pool, or destroys it if the pool no longer exists
        /// (e.g. when called from an additively loaded scene after the host scene unloads).
        /// </summary>
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            if (HasPool)
                return s_objectPool.Return(instance);

            if (instance != null)
                GameObject.Destroy(instance);

            return null;
        }

        /// <summary>
        /// Returns a Component's GameObject to the pool, or destroys it if the pool no longer exists.
        /// </summary>
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            if (HasPool)
                return s_objectPool.Return(instance);

            if (instance != null)
                GameObject.Destroy(instance);

            return null;
        }

        /// <summary>
        /// Returns a GameObject to the pool after a delay, or destroys it immediately if the pool is gone.
        /// </summary>
        public static void Return(GameObject instance, float delay)
        {
            if (HasPool)
            {
                s_objectPool.Return(instance, delay);
            }
            else
            {
                if (instance != null) GameObject.Destroy(instance);
            }
        }

        /// <summary>
        /// Returns a Component's GameObject to the pool after a delay, or destroys it immediately if the pool is gone.
        /// </summary>
        public static void Return<T>(T instance, float delay) where T : Component
        {
            if (HasPool)
            {
                s_objectPool.Return(instance, delay);
            }
            else
            {
                if (instance != null) GameObject.Destroy(instance);
            }
        }

        /// <summary>
        /// Returns a generic instance to the pool, or fires its OnReturnToPool callback
        /// if the pool no longer exists.
        /// </summary>
        public static IGenericPoolInfoReadOnly ReturnGeneric<T>(T instance) where T : class, IPoolGeneric, new()
        {
            if (HasPool == true)
                return s_objectPool.GenericReturn(instance);

            if (instance != null)
                instance.OnReturnToPool();

            return null;
        }

        /// <summary>
        /// Returns a generic instance to the pool after a delay, or fires its callback immediately
        /// if the pool no longer exists.
        /// </summary>
        public static void ReturnGeneric<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            if (HasPool == true)
            {
                s_objectPool.GenericReturn(instance, delay);
                return;
            }
            if (instance != null)
                instance.OnReturnToPool();
        }

        private static void CreatePool()
        {
            if (s_objectPool == null)
                s_objectPool = MainAutoPool.CreatePool();
        }

        /// <summary>
        /// Resets the pool reference before each scene load so a fresh instance is created.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetRunTime()
        {
            s_objectPool = null;
        }
    }
}
