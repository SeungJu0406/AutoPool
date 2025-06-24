using UnityEngine;

namespace AutoPool
{
    public static class ResourcesPool
    {
        // This script is part of a Unity Asset Store package.
        // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
        // © 2025 NSJ. All rights reserved.

        public static void SetMock()
        {
            ObjectPool.SetMock();
        }
        public static void SetReal()
        {
            ObjectPool.SetReal();
        }
        /// <summary>
        /// 풀의 정보를 가져옵니다. Resources에 저장된 프리팹을 기준으로 합니다.
        /// Gets the pool information for a specific prefab using a Resources path.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly GetInfo(string name)
        {
            return ObjectPool.GetResourcesInfo(name);
        }
        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. Resources에 저장된 프리팹을 기준으로 합니다.
        /// Sets the preload count for a specific prefab in the pool using a Resources path.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload(string name, int count)
        {
            return ObjectPool.SetResourcesPreload(name, count);
        }
        public static IPoolInfoReadOnly ClearPreload(string name)
        {
            return ObjectPool.ClearResourcesPool(name);
        }

        public static GameObject Get(string name, Transform transform)
        {
            return ObjectPool.ResourcesGet(name, transform);
        }
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