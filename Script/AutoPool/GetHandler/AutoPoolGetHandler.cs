using System;
using UnityEngine;

namespace AutoPool_Tool
{
    public class AutoPoolGetHandler
    {
        MainAutoPool _autoPool;
        AutoPoolResourcesGetHandler _resourcesGetHandler;
        AutoPoolCommonGetHandler _commonGetHandler;
        AutoPoolGenericPoolGetHandler _genericGetHandler;
        AutoPoolProcessGetHandler _processGetHandler;
        public AutoPoolGetHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _resourcesGetHandler = new AutoPoolResourcesGetHandler(this, autoPool);
            _commonGetHandler = new AutoPoolCommonGetHandler(this, autoPool);
            _genericGetHandler = new AutoPoolGenericPoolGetHandler(this, autoPool);
            _processGetHandler = new AutoPoolProcessGetHandler(this, autoPool);
        }


        #region GetPool
        #region Common

        public GameObject Get(GameObject prefab) => _commonGetHandler.Get(prefab);

        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _commonGetHandler.Get(prefab, transform, worldPositionStay);

        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _commonGetHandler.Get(prefab, pos, rot);

        public T Get<T>(T prefab) where T : Component => _commonGetHandler.Get(prefab);

        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _commonGetHandler.Get(prefab, transform, worldPositionStay);

        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _commonGetHandler.Get(prefab, pos, rot);
        #endregion
        #region Resources
        public GameObject ResourcesGet(string resources) => _resourcesGetHandler.ResourcesGet(resources);
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _resourcesGetHandler.ResourcesGet(resources, transform, worldPositionStay);

        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _resourcesGetHandler.ResourcesGet(resources, pos, rot);
        public T ResourcesGet<T>(string resources) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources);

        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, pos, rot);
        #endregion
        #region Generic
        public T GenericGet<T>() where T : class, IPoolGeneric, new() => _genericGetHandler.Get<T>();
        #endregion
        #endregion

        public GameObject ProcessGet(PoolInfo info) => _processGetHandler.ProcessGet(info);

        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false) => _processGetHandler.ProcessGet(info, transform, worldPositionStay);
        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot) => _processGetHandler.ProcessGet(info, pos, rot);

        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new() => _processGetHandler.ProcessGenericGet<T>(poolInfo);
    }
}