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
        /// 풀에서 오브젝트를 가져옵니다.
        /// </summary>
        public GameObject Get(GameObject prefab) => _commonGetHandler.Get(prefab);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _commonGetHandler.Get(prefab, transform, worldPositionStay);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _commonGetHandler.Get(prefab, pos, rot);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab) where T : Component => _commonGetHandler.Get(prefab);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _commonGetHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _commonGetHandler.Get(prefab, pos, rot);
        #endregion
        #region Resources
        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다.(Resources)
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public GameObject ResourcesGet(string resources) => _resourcesGetHandler.ResourcesGet(resources);
        /// <summary>
        ///  풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _resourcesGetHandler.ResourcesGet(resources, transform, worldPositionStay);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _resourcesGetHandler.ResourcesGet(resources, pos, rot);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, pos, rot);
        #endregion
        #region Generic
        public T GenericGet<T>() where T : class, IPoolGeneric, new() => _genericGetHandler.Get<T>();
        #endregion
        #endregion

        /// <summary>
        /// 오브젝트를 풀에서 가져오는 실제 처리 로직입니다. 오브젝트가 남아있으면 재사용하고, 없으면 새로 생성합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info) => _processGetHandler.ProcessGet(info);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다. 해당 프리팹에 대한 풀 정보를 찾고, 활성화된 오브젝트가 있으면 반환하고, 없으면 새로 생성합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false) => _processGetHandler.ProcessGet(info, transform, worldPositionStay);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다. 해당 프리팹에 대한 풀 정보를 찾고, 활성화된 오브젝트가 있으면 반환하고, 없으면 새로 생성합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot) => _processGetHandler.ProcessGet(info, pos, rot);

        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new() => _processGetHandler.ProcessGenericGet<T>(poolInfo);
    }
}