using System;
using UnityEngine;

namespace AutoPool
{
    /// <summary>
    ///  ObjectPool 클래스는 게임 오브젝트 풀링을 관리하는 유틸리티 클래스입니다.
    ///  The ObjectPool class is a utility class for managing game object pooling.
    /// </summary>
    public static class ObjectPool
    {
        private static IObjectPool s_objectPool;

        /// <summary>
        /// 풀의 정보를 가져옵니다.
        /// Gets the information of the object pool.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }
        /// <summary>
        /// 풀의 정보를 가져옵니다. 컴포넌트를 타입으로 지정할 수 있습니다.
        /// Gets the information of the object pool for a specific component type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }
        /// <summary>
        /// 풀의 정보를 가져옵니다. Resources에 저장된 프리팹을 기준으로 합니다.
        /// Gets the information of the object pool for a specific prefab stored in Resources.
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly GetResourcesInfo(string resources)
        {
            CreatePool();
            return s_objectPool.GetResourcesInfo(resources);
        }

        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. 컴포넌트를 타입으로 지정할 수 있습니다.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }
        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. Resources에 저장된 프리팹을 기준으로 합니다.
        /// Sets the preload count for a specific prefab in the pool using a Resources path.
        /// </summary>
        public static IPoolInfoReadOnly SetResourcesPreload(string resouces, int count)
        {
            CreatePool();
            return s_objectPool.SetResourcesPreload(resouces, count);
        }

        /// <summary>
        /// 풀을 비웁니다. 지정된 프리팹에 대한 풀을 비웁니다.
        /// Clears the pool for a specific prefab.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>
        /// 풀을 비웁니다. 지정된 컴포넌트 타입에 대한 풀을 비웁니다.
        /// Clears the pool for a specific component type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>
        /// 풀을 비웁니다. Resources에 저장된 프리팹에 대한 풀을 비웁니다.
        /// Clears the pool for a specific prefab stored in Resources.
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly ClearResourcesPool(string resources)
        {
            CreatePool();
            return s_objectPool.ClearResourcesPool(resources);
        }

        /// <summary>
        /// 오브젝트를 가져옵니다.
        /// Gets an objects from the pool.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static GameObject Get(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>
        /// 오브젝트를 가져옵니다. 트랜스폼을 지정할 수 있습니다.
        /// Gets an object from the pool with a specified transform.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="transform"></param>
        /// <param name="worldPositionStay"></param>
        /// <returns></returns>
        public static GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = default)
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }
        /// <summary>
        /// 오브젝트를 가져옵니다. 위치와 회전을 지정할 수 있습니다.
        /// Gets an object from the pool with a specified position and rotation.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }
        /// <summary>
        /// 오브젝트를 가져옵니다. 컴포넌트를 반환합니다.
        /// Gets an object from the pool and returns a component of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <returns></returns>
        public static T Get<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }
        /// <summary>
        /// 오브젝트를 가져옵니다. 컴포넌트를 반환합니다. 트랜스폼을 지정할 수 있습니다.
        /// Get an object from the pool and returns a component of type T with a specified transform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="transform"></param>
        /// <param name="worldPositionStay"></param>
        /// <returns></returns>
        public static T Get<T>(T prefab, Transform transform, bool worldPositionStay = default) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }
        /// <summary>
        /// 오브젝트를 가져옵니다. 컴포넌트를 반환합니다. 위치와 회전을 지정할 수 있습니다.
        ///  Get an object from the pool and returns a component of type T with a specified position and rotation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefab"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }
        /// <summary>
        /// 오브젝트를 Resources에서 가져옵니다.
        /// Get an object from Resources.
        /// </summary>
        /// <param name="resouces"></param>
        /// <returns></returns>
        public static GameObject ResourcesGet(string resouces)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(resouces);
        }
        /// <summary>
        /// 오브젝트를 Resources에서 가져옵니다. 트랜스폼을 지정할 수 있습니다.
        ///  Get an object from Resources with a specified transform.
        /// </summary>
        /// <param name="resouces"></param>
        /// <param name="transform"></param>
        /// <param name="worldPositionStay"></param>
        /// <returns></returns>
        public static GameObject ResourcesGet(string resouces, Transform transform, bool worldPositionStay = default)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(resouces, transform, worldPositionStay);
        }
        /// <summary>
        /// 오브젝트를 Resources에서 가져옵니다. 위치와 회전을 지정할 수 있습니다.
        /// Get an object from Resources with a specified position and rotation.
        /// </summary>
        /// <param name="resouces"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static GameObject ResourcesGet(string resouces, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(resouces, pos, rot);
        }
        /// <summary>
        /// 오브젝트를 Resources에서 가져옵니다. 컴포넌트를 반환합니다.
        /// Get an object from Resources and returns a component of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resouces"></param>
        /// <returns></returns>
        public static T ResourcesGet<T>(string resouces) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(resouces);
        }
        /// <summary>
        /// 오브젝트를 Resources에서 가져옵니다. 컴포넌트를 반환합니다. 트랜스폼을 지정할 수 있습니다.
        /// Get an object from Resources and returns a component of type T with a specified transform.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resouces"></param>
        /// <param name="transform"></param>
        /// <param name="worldPositionStay"></param>
        /// <returns></returns>
        public static T ResourcesGet<T>(string resouces, Transform transform, bool worldPositionStay = default) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(resouces, transform, worldPositionStay);
        }
        /// <summary>
        /// 오브젝트를 Resources에서 가져옵니다. 컴포넌트를 반환합니다. 위치와 회전을 지정할 수 있습니다.
        /// Get an object from Resources and returns a component of type T with a specified position and rotation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resouces"></param>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        /// <returns></returns>
        public static T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(resouces, pos, rot);
        }

        public static T GenericPool<T>() where T : class, IPoolGeneric, new()
        {
            CreatePool();
            return s_objectPool.GenericPool<T>();
        }
        /// <summary>
        /// 오브젝트를 반환합니다.
        /// objects are returned to the pool.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }
        /// <summary>
        /// 오브젝트를 반환합니다.
        /// objects are returned to the pool.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }
        /// <summary>
        /// 오브젝트를 반환합니다. 지연 시간을 지정할 수 있습니다.
        /// objects are returned to the pool with a specified delay.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="delay"></param>
        public static void Return(GameObject instance, float delay)
        {
            CreatePool();
            s_objectPool.Return(instance, delay);
        }
        /// <summary>
        /// 오브젝트를 반환합니다. 지연 시간을 지정할 수 있습니다.
        /// objects are returned to the pool with a specified delay.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="delay"></param>
        public static void Return<T>(T instance, float delay) where T : Component
        {
            CreatePool();
            s_objectPool.Return(instance, delay);
        }

        public static IGenericPoolInfoReadOnly ReturnGeneric<T>(T instance) where T : class, IPoolGeneric, new()
        {
            CreatePool();
            return s_objectPool.GenericReturn(instance);
        }
        public static void ReturnGeneric<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            CreatePool();
            s_objectPool.GenericReturn(instance, delay);
        }
        private static void CreatePool()
        {
            if (s_objectPool == null)
            {
                s_objectPool = AutoPool.CreatePool();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetRunTime()
        {
            s_objectPool = null;
        }
    }
}