using UnityEngine;

namespace AutoPool_Tool
{
    public class AutoPoolResourcesGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        public AutoPoolResourcesGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        public GameObject ResourcesGet(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info);
        }

        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info, transform, worldPositionStay);
        }

        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info, pos, rot);
        }

        public T ResourcesGet<T>(string resources) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info);
            return instance.GetComponent<T>();
        }

        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            return instance.GetComponent<T>();
        }

        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            return instance.GetComponent<T>();
        }
    }
}
