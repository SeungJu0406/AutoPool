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
        /// Ǯ���� ������Ʈ�� �����ɴϴ�.
        /// </summary>
        public GameObject Get(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler.ProcessGet(info);
            return instance;
        }
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler. ProcessGet(info, transform, worldPositionStay);
            return instance;
        }
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ ��ġ�� ȸ���� �����մϴ�.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            return instance;
        }

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ��ȯ�մϴ�.
        /// </summary>
        public T Get<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            GameObject instance = _getHandler.ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ������ ��ġ�� ȸ���� �����մϴ�.
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