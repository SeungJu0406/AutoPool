using System;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolGetHandler
    {
        AutoPool _autoPool;
        AutoPoolResourcesGetHandler _resourcesGetHandler;
        AutoPoolCommonGetHandler _commonGetHandler;
        AutoPoolGenericPoolGetHandler _genericGetHandler;
        AutoPoolProcessGetHandler _processGetHandler;
        public AutoPoolGetHandler(AutoPool autoPool)
        {
            _autoPool = autoPool;
            _resourcesGetHandler = new AutoPoolResourcesGetHandler(this, autoPool);
            _commonGetHandler = new AutoPoolCommonGetHandler(this, autoPool);
            _genericGetHandler = new AutoPoolGenericPoolGetHandler(this, autoPool);
            _processGetHandler = new AutoPoolProcessGetHandler(this, autoPool);
        }


        #region GetPool
        #region Common
        /// <summary>
        /// Ǯ���� ������Ʈ�� �����ɴϴ�.
        /// </summary>
        public GameObject Get(GameObject prefab) => _commonGetHandler.Get(prefab);
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _commonGetHandler.Get(prefab, transform, worldPositionStay);
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ ��ġ�� ȸ���� �����մϴ�.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _commonGetHandler.Get(prefab, pos, rot);

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ��ȯ�մϴ�.
        /// </summary>
        public T Get<T>(T prefab) where T : Component => _commonGetHandler.Get(prefab);

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _commonGetHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ������ ��ġ�� ȸ���� �����մϴ�.
        /// </summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _commonGetHandler.Get(prefab, pos, rot);
        #endregion
        #region Resources
        /// <summary>
        /// Ǯ���� ������Ʈ�� �����ɴϴ�.(Resources)
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public GameObject ResourcesGet(string resources) => _resourcesGetHandler.ResourcesGet(resources);
        /// <summary>
        ///  Ǯ���� ������Ʈ�� ��������, ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _resourcesGetHandler.ResourcesGet(resources, transform, worldPositionStay);

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ ��ġ�� ȸ���� �����մϴ�.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _resourcesGetHandler.ResourcesGet(resources, pos, rot);
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ��ȯ�մϴ�.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources);

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, �ش� ������Ʈ�� ������ ��ġ�� ȸ���� �����մϴ�.
        /// </summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, pos, rot);
        #endregion
        #region Generic
        public T GenericGet<T>() where T : class, IPoolGeneric, new() => _genericGetHandler.Get<T>();
        #endregion
        #endregion

        /// <summary>
        /// ������Ʈ�� Ǯ���� �������� ���� ó�� �����Դϴ�. ������Ʈ�� ���������� �����ϰ�, ������ ���� �����մϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info) => _processGetHandler.ProcessGet(info);
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�. �ش� �����տ� ���� Ǯ ������ ã��, Ȱ��ȭ�� ������Ʈ�� ������ ��ȯ�ϰ�, ������ ���� �����մϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false) => _processGetHandler.ProcessGet(info, transform, worldPositionStay);
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ ��ġ�� ȸ���� �����մϴ�. �ش� �����տ� ���� Ǯ ������ ã��, Ȱ��ȭ�� ������Ʈ�� ������ ��ȯ�ϰ�, ������ ���� �����մϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot) => _processGetHandler.ProcessGet(info, pos, rot);

        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new() => _processGetHandler.ProcessGenericGet<T>(poolInfo);
    }
}