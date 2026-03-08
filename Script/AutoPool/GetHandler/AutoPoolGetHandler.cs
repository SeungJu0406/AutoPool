using System;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 풀에서 객체를 가져오는 전체 흐름을 관리하는 중앙 Get 핸들러입니다.
    /// 프리팹, Resources, 제네릭 풀에 대한 요청을 각각 전용 서브 핸들러로 위임합니다.
    /// </summary>
    public class AutoPoolGetHandler
    {
        MainAutoPool _autoPool;
        AutoPoolResourcesGetHandler _resourcesGetHandler;
        AutoPoolCommonGetHandler _commonGetHandler;
        AutoPoolGenericPoolGetHandler _genericGetHandler;
        AutoPoolProcessGetHandler _processGetHandler;

        /// <summary>
        /// 메인 풀 인스턴스를 받아 각 Get 계열 서브 핸들러를 초기화합니다.
        /// </summary>
        public AutoPoolGetHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _resourcesGetHandler = new AutoPoolResourcesGetHandler(this, autoPool); // Resources 기반 Get 처리
            _commonGetHandler = new AutoPoolCommonGetHandler(this, autoPool);      // 프리팹/컴포넌트 공통 Get 처리
            _genericGetHandler = new AutoPoolGenericPoolGetHandler(this, autoPool);// 제네릭 풀 Get 처리
            _processGetHandler = new AutoPoolProcessGetHandler(this, autoPool);    // 실제 인스턴스 생성/재사용 로직
        }

        #region GetPool
        #region Common

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public GameObject Get(GameObject prefab) => _commonGetHandler.Get(prefab);

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _commonGetHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _commonGetHandler.Get(prefab, pos, rot);

        /// <summary>
        /// 컴포넌트 프리팹을 기준으로 풀에서 인스턴스를 가져와 해당 타입의 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab) where T : Component => _commonGetHandler.Get(prefab);

        /// <summary>
        /// 컴포넌트 프리팹을 기준으로 인스턴스를 가져와 지정된 트랜스폼에 배치한 뒤, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _commonGetHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>
        /// 컴포넌트 프리팹을 기준으로 인스턴스를 가져와 위치/회전을 설정한 뒤, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _commonGetHandler.Get(prefab, pos, rot);

        #endregion

        #region Resources

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public GameObject ResourcesGet(string resources) => _resourcesGetHandler.ResourcesGet(resources);

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _resourcesGetHandler.ResourcesGet(resources, transform, worldPositionStay);

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 가져와 위치/회전을 설정합니다.
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _resourcesGetHandler.ResourcesGet(resources, pos, rot);

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources);

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 가져와 위치/회전을 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, pos, rot);

        #endregion

        #region Generic

        /// <summary>
        /// 제네릭 풀에서 타입 <typeparamref name="T"/> 인스턴스를 가져옵니다.
        /// </summary>
        public T GenericGet<T>() where T : class, IPoolGeneric, new() => _genericGetHandler.Get<T>();

        #endregion
        #endregion

        /// <summary>
        /// 지정된 풀 정보에서 GameObject 인스턴스를 실제로 생성/재사용합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info) => _processGetHandler.ProcessGet(info);

        /// <summary>
        /// 지정된 풀 정보에서 인스턴스를 가져와 트랜스폼에 배치합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false) => _processGetHandler.ProcessGet(info, transform, worldPositionStay);

        /// <summary>
        /// 지정된 풀 정보에서 인스턴스를 가져와 위치/회전을 설정합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot) => _processGetHandler.ProcessGet(info, pos, rot);

        /// <summary>
        /// 제네릭 풀 정보에서 타입 <typeparamref name="T"/> 인스턴스를 생성/재사용합니다.
        /// </summary>
        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new() => _processGetHandler.ProcessGenericGet<T>(poolInfo);
    }
}