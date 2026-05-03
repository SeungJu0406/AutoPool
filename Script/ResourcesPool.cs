using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Static helper for pooling prefabs loaded from Unity's Resources folder.
    /// </summary>
    public static class ResourcesPool
    {
        private static IObjectPool s_objectPool;

        /// <summary>
        /// Returns the pool info for the given Resources path.
        /// </summary>
        public static IPoolInfoReadOnly GetInfo(string name)
        {
            CreatePool();
            return s_objectPool.GetResourcesInfo(name);
        }

        /// <summary>
        /// Pre-warms the pool for the given Resources path by creating the specified number of instances.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload(string name, int count)
        {
            CreatePool();
            return s_objectPool.SetResourcesPreload(name, count);
        }

        /// <summary>
        /// Clears all pooled instances associated with the given Resources path.
        /// </summary>
        public static IPoolInfoReadOnly ClearPool(string name)
        {
            CreatePool();
            return s_objectPool.ClearResourcesPool(name);
        }

        /// <summary>
        /// Retrieves a pooled GameObject instance for the given Resources path.
        /// </summary>
        public static GameObject Get(string name)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name);
        }

        /// <summary>
        /// Retrieves a pooled GameObject and places it under the given transform.
        /// </summary>
        public static GameObject Get(string name, Transform transform, bool worldPositionStay = false)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name, transform, worldPositionStay);
        }

        /// <summary>
        /// Retrieves a pooled GameObject and sets its position and rotation.
        /// </summary>
        public static GameObject Get(string name, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name, pos, rot);
        }

        /// <summary>
        /// Retrieves a pooled component instance for the given Resources path.
        /// </summary>
        public static T Get<T>(string name) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name);
        }

        /// <summary>
        /// Retrieves a pooled component instance and places it under the given transform.
        /// </summary>
        public static T Get<T>(string name, Transform transform, bool worldPositionStay = false) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name, transform, worldPositionStay);
        }

        /// <summary>
        /// Retrieves a pooled component instance and sets its position and rotation.
        /// </summary>
        public static T Get<T>(string name, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name, pos, rot);
        }

        /// <summary>
        /// Returns a GameObject instance to the Resources pool.
        /// </summary>
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }

        /// <summary>
        /// Returns a component instance to the Resources pool.
        /// </summary>
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }

        private static void CreatePool()
        {
            if (s_objectPool == null)
            {
                s_objectPool = MainAutoPool.CreatePool();
            }
        }
    }
}
