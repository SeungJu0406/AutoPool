using UnityEngine;

namespace AutoPool
{
    public class AutoPoolResourcesGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;
        public AutoPoolResourcesGetHandler(AutoPoolGetHandler getHandler ,MainAutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다.(Resources)
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public GameObject ResourcesGet(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info);
            return instance;
        }
        /// <summary>
        ///  풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            return instance;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 위치와 회전을 설정합니다.
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