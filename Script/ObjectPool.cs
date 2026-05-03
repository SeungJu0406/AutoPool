using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Static facade for the central object pool.
    /// All pooling operations are accessible through this class. The pool instance is created automatically on first use.
    /// </summary>
    public static class ObjectPool
    {
        /// <summary>
        /// The active pool instance. Created automatically when first accessed.
        /// </summary>
        public static MainAutoPool Instance
        {
            get
            {
                if (s_objectPool == null)
                {
                    CreatePool();
                }
                return s_objectPool;
            }
        }

        private static MainAutoPool s_objectPool;

        /// <summary>
        /// Returns true if the pool instance exists and is valid.
        /// </summary>
        public static bool HasPool => s_objectPool != null && !s_objectPool.Equals(null);

        /// <summary>
        /// Returns the pool info for the given prefab.
        /// </summary>
        public static IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        /// <summary>
        /// Returns the pool info for the given component prefab.
        /// </summary>
        public static IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        /// <summary>
        /// Pre-warms the pool for the given prefab by creating the specified number of instances.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>
        /// Pre-warms the pool for the given component prefab by creating the specified number of instances.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>
        /// Clears all pooled instances associated with the given prefab.
        /// </summary>
        public static IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>
        /// Clears all pooled instances associated with the given component prefab.
        /// </summary>
        public static IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>
        /// Retrieves a pooled GameObject instance for the given prefab.
        /// </summary>
        public static GameObject Get(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>
        /// Retrieves a pooled GameObject and places it under the given transform.
        /// </summary>
        public static GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = default)
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }

        /// <summary>
        /// Retrieves a pooled GameObject and sets its position and rotation.
        /// </summary>
        public static GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        /// <summary>
        /// Retrieves a pooled component instance for the given prefab.
        /// </summary>
        public static T Get<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>
        /// Retrieves a pooled component instance and places it under the given transform.
        /// </summary>
        public static T Get<T>(T prefab, Transform transform, bool worldPositionStay = default) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }

        /// <summary>
        /// Retrieves a pooled component instance and sets its position and rotation.
        /// </summary>
        public static T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        /// <summary>
        /// Retrieves a pooled component instance loaded from Resources at the given path, and sets its position and rotation.
        /// </summary>
        public static T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(resouces, pos, rot);
        }

        /// <summary>
        /// Retrieves a generic pool instance of type <typeparamref name="T"/>.
        /// </summary>
        public static T GenericPool<T>() where T : class, IPoolGeneric, new()
        {
            CreatePool();
            return s_objectPool.GenericPool<T>();
        }

        /// <summary>
        /// Returns a GameObject instance to the pool, or destroys it if the pool no longer exists.
        /// </summary>
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            if (HasPool)
            {
                return s_objectPool.Return(instance);
            }

            if (instance != null)
            {
                GameObject.Destroy(instance);
            }

            return null;
        }

        /// <summary>
        /// Returns a component instance to the pool, or destroys its GameObject if the pool no longer exists.
        /// </summary>
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            if (HasPool)
            {
                return s_objectPool.Return(instance);
            }

            if (instance != null)
            {
                GameObject.Destroy(instance);
            }

            return null;
        }

        /// <summary>
        /// Returns a GameObject instance to the pool after the specified delay in seconds.
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
        /// Returns a component instance to the pool after the specified delay in seconds.
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
        /// Returns a generic pool instance and invokes its return callback.
        /// </summary>
        public static IGenericPoolInfoReadOnly ReturnGeneric<T>(T instance) where T : class, IPoolGeneric, new()
        {
            if (HasPool == true)
            {
                return s_objectPool.GenericReturn(instance);
            }

            if (instance != null)
            {
                instance.OnReturnToPool();
            }
            return null;
        }

        /// <summary>
        /// Returns a generic pool instance after the specified delay in seconds.
        /// </summary>
        public static void ReturnGeneric<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            if (HasPool == true)
            {
                s_objectPool.GenericReturn(instance, delay);
                return;
            }
            if (instance != null)
            {
                instance.OnReturnToPool();
            }
        }

        private static void CreatePool()
        {
            if (s_objectPool == null)
            {
                s_objectPool = MainAutoPool.CreatePool();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetRunTime()
        {
            s_objectPool = null;
        }
    }
}
