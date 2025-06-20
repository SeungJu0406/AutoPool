using UnityEngine;

namespace NSJ_EasyPoolKit
{
    public static class ResourcesPool
    {
        // This script is part of a Unity Asset Store package.
        // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
        // © 2025 NSJ. All rights reserved.
        public static GameObject Get(string name)
        {
            return ObjectPool.ResourcesGet(name);
        }
        public static GameObject Get(string name, Transform transform, bool worldPositionStay = false)
        {
            return ObjectPool.ResourcesGet(name, transform, worldPositionStay);
        }
        public static GameObject Get(string name, Vector3 pos, Quaternion rot)
        {
            return ObjectPool.ResourcesGet(name, pos, rot);
        }
        public static T Get<T>(string name) where T : Component
        {
            return ObjectPool.ResourcesGet<T>(name);
        }
        public static T Get<T>(string name, Transform transform, bool worldPositionStay = false) where T : Component
        {
            return ObjectPool.ResourcesGet<T>(name, transform, worldPositionStay);
        }
        public static T Get<T>(string name, Vector3 pos, Quaternion rot) where T : Component
        {
            return ObjectPool.ResourcesGet<T>(name, pos, rot);
        }
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            return ObjectPool.Return(instance);
        }
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            return ObjectPool.Return(instance);
        }
    }
}