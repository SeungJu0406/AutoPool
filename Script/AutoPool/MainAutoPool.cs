using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoPool_Tool
{

    /// <summary>
    /// 씬 전체에서 사용하는 중앙 풀 매니저입니다.
    /// 프리팹 / Resources / 제네릭 풀에 대한 생성, 대여, 반환, 프리로드를 통합 관리합니다.
    /// </summary>
    public class MainAutoPool : MonoBehaviour, IObjectPool
    {
        /// <summary>
        /// 애플리케이션 종료 상태 플래그입니다. 종료 중에는 풀 생성을 방지합니다.
        /// </summary>
        private static bool s_isApplicationQuit = false;

        /// <summary>
        /// 새로운 메인 풀 GameObject를 생성하고 <see cref="MainAutoPool"/> 컴포넌트를 추가합니다.
        /// 애플리케이션 종료 중이면 null을 반환합니다.
        /// </summary>
        public static MainAutoPool CreatePool()
        {
            if (s_isApplicationQuit == true)
                return null;

            GameObject newPool = new GameObject("MainAutoPool"); // 풀 루트 오브젝트 생성
            MainAutoPool pool = newPool.AddComponent<MainAutoPool>(); // MainAutoPool 컴포넌트 부착
            return pool;
        }

        /// <summary>
        /// 프리팹 InstanceID를 키로 갖는 GameObject 풀 딕셔너리입니다.
        /// </summary>
        public Dictionary<int, PoolInfo> PoolDic = new Dictionary<int, PoolInfo>();

        /// <summary>
        /// Resources 경로 문자열을 키로, 프리팹 InstanceID를 값으로 갖는 매핑 딕셔너리입니다.
        /// </summary>
        public Dictionary<string, int> ResourcesPoolDic = new Dictionary<string, int>();

        /// <summary>
        /// 타입을 키로 갖는 제네릭 풀 딕셔너리입니다.
        /// </summary>
        public Dictionary<Type, GenericPoolInfo> GenericPoolDic = new Dictionary<Type, GenericPoolInfo>();

        /// <summary>
        /// 동일한 시간 값에 대한 <see cref="WaitForSeconds"/> 인스턴스를 캐싱하는 딕셔너리입니다.
        /// </summary>
        public Dictionary<float, WaitForSeconds> DelayDic = new Dictionary<float, WaitForSeconds>();

        /// <summary>Get 관련 로직을 담당하는 핸들러입니다.</summary>
        private AutoPoolGetHandler _getHandler;
        /// <summary>Return 관련 로직을 담당하는 핸들러입니다.</summary>
        private AutoPoolReturnHandler _returnHandler;
        /// <summary>Preload / Clear 관련 로직을 담당하는 핸들러입니다.</summary>
        private AutoPoolPreloadHandler _preloadHandler;
        /// <summary>Pool 검색 관련 로직을 담당하는 핸들러입니다.</summary>
        private AutoPoolFindPoolHandler _findPoolHandler;
        /// <summary>Pool 생성 관련 로직을 담당하는 핸들러입니다.</summary>
        private AutoPoolCreatePoolHandler _createPoolHandler;
        /// <summary>Rigidbody Sleep / Wake 제어를 담당하는 핸들러입니다.</summary>
        private AutoPoolSetRbHandler _setRbHandler;

#if UNITY_EDITOR
        /// <summary>
        /// 에디터에서 GameObject 풀의 전체 상태 정보를 조회합니다.
        /// </summary>
        public List<IPoolInfoReadOnly> GetAllPoolInfos()
        {
            return PoolDic.Values.Cast<IPoolInfoReadOnly>().ToList();
        }

        /// <summary>
        /// 에디터에서 제네릭 풀의 전체 상태 정보를 조회합니다.
        /// </summary>
        public List<IGenericPoolInfoReadOnly> GetAllGenericPoolInfos()
        {
            return GenericPoolDic.Values.Cast<IGenericPoolInfoReadOnly>().ToList();
        }
#endif

        /// <summary>
        /// 초기화 시 각 역할별 핸들러를 생성합니다.
        /// </summary>
        private void Awake()
        {
            SetHandler();
        }

        #region GetInfo

        /// <summary>
        /// 프리팹 기반 풀 정보를 조회합니다.
        /// </summary>
        public IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            return FindPool(prefab);
        }

        /// <summary>
        /// 컴포넌트 프리팹 기반 풀 정보를 조회합니다.
        /// </summary>
        public IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            return FindPool(prefab.gameObject);
        }

        /// <summary>
        /// Resources 경로 기반 풀 정보를 조회합니다.
        /// </summary>
        public IPoolInfoReadOnly GetResourcesInfo(string resources)
        {
            return FindResourcesPool(resources);
        }

        #endregion

        #region SePreload

        /// <summary>
        /// 프리팹 기반 풀에 대해 지정 수량만큼 프리로드를 수행합니다.
        /// </summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count) => _preloadHandler.SetPreload(prefab, count);

        /// <summary>
        /// 컴포넌트 프리팹 기반 풀에 대해 지정 수량만큼 프리로드를 수행합니다.
        /// </summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component => _preloadHandler.SetPreload(prefab, count);

        /// <summary>
        /// Resources 경로 기반 풀에 대해 지정 수량만큼 프리로드를 수행합니다.
        /// </summary>
        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count) => _preloadHandler.SetResourcesPreload(resources, count);

        /// <summary>
        /// 프리팹 기반 풀의 객체를 정리합니다.
        /// </summary>
        public IPoolInfoReadOnly ClearPool(GameObject prefab) => _preloadHandler.ClearPool(prefab);

        /// <summary>
        /// 컴포넌트 프리팹 기반 풀의 객체를 정리합니다.
        /// </summary>
        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component => _preloadHandler.ClearPool(prefab);

        /// <summary>
        /// Resources 경로 기반 풀의 객체를 정리합니다.
        /// </summary>
        public IPoolInfoReadOnly ClearResourcesPool(string resources) => _preloadHandler.ClearResourcesPool(resources);

        /// <summary>
        /// 제네릭 타입 <typeparamref name="T"/> 기반 풀의 객체를 정리합니다.
        /// </summary>
        public IGenericPoolInfoReadOnly ClearGenericPool<T>() where T : class, IPoolGeneric, new() => _preloadHandler.ClearGenericPool<T>();

        /// <summary>
        /// 지정된 풀 정보를 직접 전달하여 정리합니다.
        /// </summary>
        public void ClearPool(PoolInfo info) => _preloadHandler.ClearPool(info);

        #endregion

        #region GetPool

        #region Common

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public GameObject Get(GameObject prefab) => _getHandler.Get(prefab);

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _getHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 가져와 위치/회전을 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _getHandler.Get(prefab, pos, rot);

        /// <summary>
        /// 컴포넌트 프리팹 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public T Get<T>(T prefab) where T : Component => _getHandler.Get(prefab);

        /// <summary>
        /// 컴포넌트 프리팹 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _getHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>
        /// 컴포넌트 프리팹 인스턴스를 가져와 위치/회전을 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _getHandler.Get(prefab, pos, rot);

        #endregion

        #region Resources

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public GameObject ResourcesGet(string resources) => _getHandler.ResourcesGet(resources);

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _getHandler.ResourcesGet(resources, transform, worldPositionStay);

        /// <summary>
        /// Resources 경로 기반 GameObject 인스턴스를 가져와 위치/회전을 설정합니다.
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _getHandler.ResourcesGet(resources, pos, rot);

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component => _getHandler.ResourcesGet<T>(resources);

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _getHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 가져와 위치/회전을 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _getHandler.ResourcesGet<T>(resources, pos, rot);

        #endregion

        #region Generic

        /// <summary>
        /// 제네릭 풀에서 타입 <typeparamref name="T"/> 인스턴스를 가져옵니다.
        /// </summary>
        public T GenericPool<T>() where T : class, IPoolGeneric, new() => _getHandler.GenericGet<T>();

        #endregion

        #endregion

        #region ReturnPool

        /// <summary>
        /// GameObject 인스턴스를 풀에 반환합니다.
        /// </summary>
        public IPoolInfoReadOnly Return(GameObject instance) => _returnHandler.Return(instance);

        /// <summary>
        /// 컴포넌트 인스턴스를 풀에 반환합니다.
        /// </summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component => _returnHandler.Return(instance);

        /// <summary>
        /// 지정된 지연 시간 후 GameObject 인스턴스를 풀에 반환합니다.
        /// </summary>
        public void Return(GameObject instance, float delay) => _returnHandler.Return(instance, delay);

        /// <summary>
        /// 지정된 지연 시간 후 컴포넌트 인스턴스를 풀에 반환합니다.
        /// </summary>
        public void Return<T>(T instance, float delay) where T : Component => _returnHandler.Return(instance, delay);

        /// <summary>
        /// 제네릭 풀 인스턴스를 반환하고 풀 정보를 반환합니다.
        /// </summary>
        public IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new() => _returnHandler.GenericReturn(instance);

        /// <summary>
        /// 지정된 지연 시간 후 제네릭 풀 인스턴스를 반환합니다.
        /// </summary>
        public void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new() => _returnHandler.GenericReturn(instance, delay);

        #endregion

        /// <summary>
        /// 프리팹 기반 풀 정보를 찾거나 생성합니다.
        /// </summary>
        public PoolInfo FindPool(GameObject poolPrefab) => _findPoolHandler.FindPool(poolPrefab);

        /// <summary>
        /// Resources 경로 기반 풀 정보를 찾거나 생성합니다.
        /// </summary>
        public PoolInfo FindResourcesPool(string resources) => _findPoolHandler.FindResourcesPool(resources);

        /// <summary>
        /// 제네릭 타입 기반 풀 정보를 찾거나 생성합니다.
        /// </summary>
        public GenericPoolInfo FindGenericPool<T>() where T : class, IPoolGeneric, new() => _findPoolHandler.FindGenericPool<T>();

        /// <summary>
        /// 지정된 풀에서 유효한 GameObject 인스턴스가 존재하는지 검사합니다.
        /// </summary>
        public bool FindObject(PoolInfo info) => _findPoolHandler.FindObject(info);

        /// <summary>
        /// 지정된 제네릭 풀에서 유효한 인스턴스가 존재하는지 검사합니다.
        /// </summary>
        public bool FindGeneric<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new() => _findPoolHandler.FindGeneric<T>(poolInfo);

        /// <summary>
        /// 새 GameObject 풀을 생성하고 등록합니다.
        /// </summary>
        public PoolInfo RegisterPool(GameObject poolPrefab, int prefabID) => _createPoolHandler.RegisterPool(poolPrefab, prefabID);

        /// <summary>
        /// 새 제네릭 풀을 생성하고 등록합니다.
        /// </summary>
        public GenericPoolInfo RegisterGenericPool<T>() where T : class, IPoolGeneric, new() => _createPoolHandler.RegisterGenericPool<T>();

        /// <summary>
        /// 인스턴스에 <see cref="PooledObject"/> 컴포넌트를 부착하고 풀 정보에 등록합니다.
        /// </summary>
        public PooledObject AddPoolObjectComponent(GameObject instance, PoolInfo info) => _createPoolHandler.AddPoolObjectComponent(instance, info);

        /// <summary>
        /// 풀 오브젝트의 Rigidbody / Rigidbody2D를 Sleep 상태로 전환합니다.
        /// </summary>
        public void SleepRigidbody(PooledObject instance) => _setRbHandler.SleepRigidbody(instance);

        /// <summary>
        /// 풀 오브젝트의 Rigidbody / Rigidbody2D를 깨웁니다.
        /// </summary>
        public void WakeUpRigidBody(PooledObject instance) => _setRbHandler.WakeUpRigidBody(instance);

        /// <summary>
        /// 지정된 시간에 대한 <see cref="WaitForSeconds"/> 인스턴스를 캐싱하여 반환합니다.
        /// </summary>
        public WaitForSeconds Second(float time)
        {
            float normalize = Mathf.Round(time * 100f) * 0.01f; // 소수 둘째 자리까지 정규화

            if (DelayDic.ContainsKey(normalize) == false)
            {
                DelayDic.Add(normalize, new WaitForSeconds(normalize));
            }
            return DelayDic[normalize];
        }

        /// <summary>
        /// 각 역할별 내부 핸들러 인스턴스를 생성합니다.
        /// </summary>
        private void SetHandler()
        {
            _getHandler = new AutoPoolGetHandler(this);
            _returnHandler = new AutoPoolReturnHandler(this);
            _preloadHandler = new AutoPoolPreloadHandler(this);
            _findPoolHandler = new AutoPoolFindPoolHandler(this);
            _createPoolHandler = new AutoPoolCreatePoolHandler(this);
            _setRbHandler = new AutoPoolSetRbHandler(this);
        }

        /// <summary>
        /// 씬 로드 전에 애플리케이션 종료 플래그를 초기화합니다.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnRuntimeLoad()
        {
            s_isApplicationQuit = false;
        }

        /// <summary>
        /// 애플리케이션 종료 시 추가 풀 생성이 되지 않도록 플래그를 설정합니다.
        /// </summary>
        private void OnApplicationQuit()
        {
            s_isApplicationQuit = true;
        }
    }
}