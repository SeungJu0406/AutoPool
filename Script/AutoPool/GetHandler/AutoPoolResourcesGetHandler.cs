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
        /// Ǯ���� ������Ʈ�� �����ɴϴ�.(Resources)
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
        ///  Ǯ���� ������Ʈ�� ��������, ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            return instance;
        }

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ ��ġ�� ȸ���� �����մϴ�.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, pos, rot);
            return instance;
        }
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ��ȯ�մϴ�.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ������ ��ġ�� ȸ���� �����մϴ�.
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