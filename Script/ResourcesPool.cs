using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Unity Resources 경로 기반 프리팹들을 대상으로 풀링 기능을 제공하는 정적 헬퍼 클래스입니다.
    /// </summary>
    public static class ResourcesPool
    {

        /// <summary>
        /// Resources 기반 풀링을 실제로 처리하는 내부 객체 풀 인스턴스입니다.
        /// 필요 시 자동으로 생성됩니다.
        /// </summary>
        private static IObjectPool s_objectPool;

        /// <summary>
        /// 지정한 Resources 이름에 대한 풀 정보를 가져옵니다.
        /// </summary>
        public static IPoolInfoReadOnly GetInfo(string name)
        {
            CreatePool();
            return s_objectPool.GetResourcesInfo(name);
        }

        /// <summary>
        /// 지정한 Resources 이름에 대해 미리 생성할 개수를 설정합니다.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload(string name, int count)
        {
            CreatePool();
            return s_objectPool.SetResourcesPreload(name, count);
        }

        /// <summary>
        /// 지정한 Resources 이름과 연결된 풀을 비웁니다.
        /// </summary>
        public static IPoolInfoReadOnly ClearPool(string name)
        {
            CreatePool();
            return s_objectPool.ClearResourcesPool(name);
        }

        /// <summary>
        /// Resources 이름을 사용하여 GameObject 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public static GameObject Get(string name)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name);
        }

        /// <summary>
        /// Resources 이름을 사용하여 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public static GameObject Get(string name, Transform transform, bool worldPositionStay = false)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name, transform, worldPositionStay);
        }

        /// <summary>
        /// Resources 이름을 사용하여 인스턴스를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public static GameObject Get(string name, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.ResourcesGet(name, pos, rot);
        }

        /// <summary>
        /// Resources 이름을 사용하여 컴포넌트 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public static T Get<T>(string name) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name);
        }

        /// <summary>
        /// Resources 이름을 사용하여 컴포넌트 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public static T Get<T>(string name, Transform transform, bool worldPositionStay = false) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name, transform, worldPositionStay);
        }

        /// <summary>
        /// Resources 이름을 사용하여 컴포넌트 인스턴스를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public static T Get<T>(string name, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(name, pos, rot);
        }

        /// <summary>
        /// GameObject 인스턴스를 Resources 기반 풀에 반환합니다.
        /// </summary>
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }

        /// <summary>
        /// 컴포넌트 인스턴스를 Resources 기반 풀에 반환합니다.
        /// </summary>
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            CreatePool();
            return s_objectPool.Return(instance);
        }

        /// <summary>
        /// 내부 IObjectPool 인스턴스를 생성합니다. 이미 존재하면 아무 작업도 하지 않습니다.
        /// </summary>
        private static void CreatePool()
        {
            if (s_objectPool == null)
            {
                s_objectPool = MainAutoPool.CreatePool();
            }
        }
    }
}