using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Static helper for pooling prefabs loaded from the Resources folder.
    /// </summary>
    public static class ResourcesPool
    {
        private static IObjectPool s_objectPool;

        /// <summary>Returns pool info for the given Resources path.</summary>
        public static IPoolInfoReadOnly GetInfo(string name)
        {
            CreatePool();
            return s_objectPool.GetResourcesInfo(name);
        }

        /// <summary>Pre-warms the Resources pool to at least <paramref name="count"/> instances.</summary>
        public static IPoolInfoReadOnly SetPreload(string name, int count)
        {
            CreatePool();
            return s_objectPool.SetResourcesPreload(name, count);
        }

        /// <summary>Destroys all pooled instances for the given Resources path.</summary>
        public static IPoolInfoReadOnly ClearPool(string name)
        {
            CreatePool();
            return s_objectPool.ClearResourcesPool(name);
        }

        /// <summary>Retrieves a GameObject from the Resources pool.</summary>
        public static GameObject Get(string name)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name);
        }

        /// <summary>Retrieves a Resources-path GameObject and parents it under <paramref name="transform"/>.</summary>
        public static GameObject Get(string name, Transform transform, bool worldPositionStay = false)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name, transform, worldPositionStay);
        }

        /// <summary>Retrieves a Resources-path GameObject at the given position and rotation.</summary>
        public static GameObject Get(string name, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name, pos, rot);
        }

        /// <summary>Retrieves a Component from the Resources pool.</summary>
        public static T Get<T>(string name) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name);
        }

        /// <summary>Retrieves a Resources-path Component and parents it under <paramref name="transform"/>.</summary>
        public static T Get<T>(string name, Transform transform, bool worldPositionStay = false) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name, transform, worldPositionStay);
        }

        /// <summary>Retrieves a Resources-path Component at the given position and rotation.</summary>
        public static T Get<T>(string name, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name, pos, rot);
        }

        /// <summary>Returns a GameObject to the Resources pool.</summary>
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }

        /// <summary>Returns a Component's GameObject to the Resources pool.</summary>
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }

        private static void CreatePool()
        {
            if (s_objectPool == null)
                s_objectPool = MainAutoPool.CreatePool();
        }
    }
}
