using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Unity Resources 경로 기반으로 풀에서 객체를 가져오는 전용 핸들러입니다.
    /// </summary>
    public class AutoPoolResourcesGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        /// <summary>
        /// Resources Get 처리를 위해 상위 Get 핸들러와 메인 풀을 주입합니다.
        /// </summary>
        public AutoPoolResourcesGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public GameObject ResourcesGet(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info);
            return instance;
        }

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            return instance;
        }

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            return instance;
        }

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            T component = instance.GetComponent<T>();
            return component;
        }
    }
}