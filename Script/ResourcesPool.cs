using UnityEngine;

namespace AutoPool_Tool
{
    public static class ResourcesPool
    {
        // This script is part of a Unity Asset Store package.
        // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
        // © 2025 NSJ. All rights reserved.

        private static IObjectPool s_objectPool;


        public static IPoolInfoReadOnly GetInfo(string name)
        {
            CreatePool();
            return s_objectPool.GetResourcesInfo(name);
        }

        public static IPoolInfoReadOnly SetPreload(string name, int count)
        {
            CreatePool();
            return s_objectPool.SetResourcesPreload(name, count);
        }
        public static IPoolInfoReadOnly ClearPool(string name)
        {
            CreatePool();
            return s_objectPool.ClearResourcesPool(name);
        }
        public static GameObject Get(string name)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name);
        }
        public static GameObject Get(string name, Transform transform, bool worldPositionStay = false)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name, transform, worldPositionStay);
        }
        public static GameObject Get(string name, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name, pos, rot);
        }
        public static T Get<T>(string name) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name);
        }
        public static T Get<T>(string name, Transform transform, bool worldPositionStay = false) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name, transform, worldPositionStay);
        }
        public static T Get<T>(string name, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name, pos, rot);
        }
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }
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