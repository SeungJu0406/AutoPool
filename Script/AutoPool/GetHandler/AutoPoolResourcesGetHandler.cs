using UnityEngine;

namespace AutoPool_Tool
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

        public GameObject ResourcesGet(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info);
            return instance;
        }
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            return instance;
        }

        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            return instance;
        }

        public T ResourcesGet<T>(string resources) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            T component = instance.GetComponent<T>();
            return component;
        }
    }
}