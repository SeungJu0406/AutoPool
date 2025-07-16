using UnityEngine;

namespace AutoPool
{
    public class AutoPoolCommonGetHandler
    {
        MainAutoPool _autoPool;
        AutoPoolGetHandler _getHandler;

        public AutoPoolCommonGetHandler( AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _getHandler = getHandler;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다.
        /// </summary>
        public GameObject Get(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler.ProcessGet(info);
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler. ProcessGet(info, transform, worldPositionStay);
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            return instance;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            GameObject instance = _getHandler.ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            T component = instance.GetComponent<T>();
            return component;
        }
    }
}