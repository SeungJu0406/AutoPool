using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Handles Get requests for pools backed by Unity Resources-loaded prefabs.
    /// </summary>
    public class AutoPoolResourcesGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        public AutoPoolResourcesGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        /// <summary>Retrieves a GameObject from the Resources pool.</summary>
        public GameObject ResourcesGet(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info).gameObject;
        }

        /// <summary>Retrieves a Resources-path GameObject and parents it under <paramref name="transform"/>.</summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info, transform, worldPositionStay).gameObject;
        }

        /// <summary>Retrieves a Resources-path GameObject at the given position and rotation.</summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info, pos, rot).gameObject;
        }

        /// <summary>Retrieves a Component from the Resources pool.</summary>
        public T ResourcesGet<T>(string resources) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info).GetCachedComponent<T>();
        }

        /// <summary>Retrieves a Resources-path Component and parents it under <paramref name="transform"/>.</summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info, transform, worldPositionStay).GetCachedComponent<T>();
        }

        /// <summary>Retrieves a Resources-path Component at the given position and rotation.</summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return _getHandler.ProcessGet(info, pos, rot).GetCachedComponent<T>();
        }
    }
}
