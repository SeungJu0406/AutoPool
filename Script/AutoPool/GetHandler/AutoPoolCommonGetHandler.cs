using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Handles Get requests for prefab-based GameObject and Component pools.
    /// </summary>
    public class AutoPoolCommonGetHandler
    {
        MainAutoPool _autoPool;
        AutoPoolGetHandler _getHandler;

        public AutoPoolCommonGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _getHandler = getHandler;
        }

        /// <summary>Retrieves a GameObject instance from the prefab pool.</summary>
        public GameObject Get(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler.ProcessGet(info);
            return instance;
        }

        /// <summary>Retrieves a GameObject instance and parents it under <paramref name="transform"/>.</summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            return instance;
        }

        /// <summary>Retrieves a GameObject instance at the given position and rotation.</summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            return instance;
        }

        /// <summary>Retrieves a Component instance from the prefab pool.</summary>
        public T Get<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            GameObject instance = _getHandler.ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>Retrieves a Component instance and parents it under <paramref name="transform"/>.</summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>Retrieves a Component instance at the given position and rotation.</summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            T component = instance.GetComponent<T>();
            return component;
        }
    }
}
