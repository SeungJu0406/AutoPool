using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoPool
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // © 2025 NSJ. All rights reserved.

    /// <summary>
    /// 오브젝트 풀링을 위한 메인 클래스입니다.
    /// 자동 반환, 딜레이 반환, 재사용 가능한 풀 구조를 제공합니다.
    /// </summary>
    public class AutoPool : MonoBehaviour, IObjectPool
    {
        /// <summary>
        /// 풀 오브젝트 최대 유지 시간입니다. 시간이 지나면 자동 비활성화됩니다.
        /// </summary>
        private static bool s_isApplicationQuit = false;

        private static AutoPool _instance;
        /// <summary>
        /// AutoPool 싱글톤 인스턴스입니다. 자동 생성됩니다.
        /// </summary>
        public static AutoPool Instance
        {
            get
            {
                return CreatePool();
            }
            set
            {
                _instance = value;
            }
        }

        public static AutoPool CreatePool()
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                if (s_isApplicationQuit == true)
                    return null;
                // 새로운 AutoPool GameObject 생성 및 할당
                GameObject newPool = new GameObject("AutoPool");
                AutoPool pool = newPool.AddComponent<AutoPool>();
                return pool;
            }
        }


        /// <summary>
        /// 프리팹 ID를 기준으로 풀 정보를 저장하는 딕셔너리입니다.
        /// </summary>
        public Dictionary<int, PoolInfo> PoolDic = new Dictionary<int, PoolInfo>();

        /// <summary>
        /// Resources에 저장된 프리팹을 기준으로 풀 정보를 저장하는 딕셔너리 입니다
        /// </summary>
        public Dictionary<string, int> ResourcesPoolDic = new Dictionary<string, int>();

        /// <summary>
        /// 기본 타입을 기준으로 풀 정보를 저장하는 딕셔너리입니다.
        /// </summary>
        public Dictionary<Type, GenericPoolInfo> GenericPoolDic = new Dictionary<Type, GenericPoolInfo>();
        /// <summary>
        /// 동일한 시간 값에 대해 WaitForSeconds 인스턴스를 재사용하기 위한 캐시입니다.
        /// </summary>
        public Dictionary<float, WaitForSeconds> DelayDic = new Dictionary<float, WaitForSeconds>();

        private AutoPoolGetHandler _getHandler;
        private AutoPoolReturnHandler _returnHandler;
        private AutoPoolPreloadHandler _preloadHandler;
        private AutoPoolFindPoolHandler _findPoolHandler;
        private AutoPoolCreatePoolHandler _createPoolHandler;
        private AutoPoolSetRbHandler _setRbHandler;
        private AutoPoolLifeHandler _lifeHandler;
#if UNITY_EDITOR
        public List<IPoolInfoReadOnly> GetAllPoolInfos()
        {
            return PoolDic.Values.Cast<IPoolInfoReadOnly>().ToList();
        }
#endif
        private void Awake()
        {
            SetSingleTon();
            SetHandler();
        }
        #region GetInfo
        public IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            return FindPool(prefab);
        }

        public IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            return FindPool(prefab.gameObject);
        }

        public IPoolInfoReadOnly GetResourcesInfo(string resources)
        {
            return FindResourcesPool(resources);
        }
        #endregion
        #region SePreload
        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count) => _preloadHandler.SetPreload(prefab, count);

        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. 컴포넌트를 타입으로 지정할 수 있습니다.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component => _preloadHandler.SetPreload(prefab, count);

        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. Resources에 저장된 프리팹을 기준으로 합니다.
        /// Sets the preload count for a specific prefab in the pool using a Resources path.
        /// </summary>
        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count) => _preloadHandler.SetResourcesPreload(resources, count);

        public IPoolInfoReadOnly ClearPool(GameObject prefab) => _preloadHandler.ClearPool(prefab);

        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component => _preloadHandler.ClearPool(prefab);

        public IPoolInfoReadOnly ClearResourcesPool(string resources) => _preloadHandler.ClearResourcesPool(resources);
        public IGenericPoolInfoReadOnly ClearGenericPool<T>() where T : class, IPoolGeneric, new() => _preloadHandler.ClearGenericPool<T>();
        public void ClearPool(PoolInfo info) => _preloadHandler.ClearPool(info);
        #endregion
        #region GetPool
        #region Common
        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다.
        /// </summary>
        public GameObject Get(GameObject prefab) => _getHandler.Get(prefab);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _getHandler.Get(prefab, transform, worldPositionStay);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _getHandler.Get(prefab, pos, rot);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab) where T : Component => _getHandler.Get(prefab);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _getHandler.Get(prefab, transform, worldPositionStay);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _getHandler.Get(prefab, pos, rot);
        #endregion
        #region Resources
        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다.(Resources)
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public GameObject ResourcesGet(string resources) => _getHandler.ResourcesGet(resources);
        /// <summary>
        ///  풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _getHandler.ResourcesGet(resources, transform, worldPositionStay);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _getHandler.ResourcesGet(resources, pos, rot);
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component => _getHandler.ResourcesGet<T>(resources);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _getHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _getHandler.ResourcesGet<T>(resources, pos, rot);
        #endregion
        #region Generic
        public T GenericPool<T>() where T : class, IPoolGeneric, new() => _getHandler.GenericGet<T>();
        #endregion
        #endregion
        #region ReturnPool
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 풀에 다시 추가됩니다.
        /// </summary>
        public IPoolInfoReadOnly Return(GameObject instance) => _returnHandler.Return(instance);
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 풀에 다시 추가됩니다.
        /// </summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component => _returnHandler.Return(instance);
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 지정된 지연 시간 후에 풀에 다시 추가됩니다.
        /// </summary>
        public void Return(GameObject instance, float delay) => _returnHandler.Return(instance, delay);
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 지정된 지연 시간 후에 풀에 다시 추가됩니다.
        /// </summary>
        public void Return<T>(T instance, float delay) where T : Component => _returnHandler.Return(instance, delay);
        public IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new() => _returnHandler.GenericReturn(instance);
        public void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new() => _returnHandler.GenericReturn(instance, delay);
        #endregion    
        public PoolInfo FindPool(GameObject poolPrefab) => _findPoolHandler.FindPool(poolPrefab);
        public PoolInfo FindResourcesPool(string resources) => _findPoolHandler.FindResourcesPool(resources);
        public GenericPoolInfo FindGenericPool<T>() where T : class, IPoolGeneric, new() => _findPoolHandler.FindGenericPool<T>();
        public bool FindObject(PoolInfo info) => _findPoolHandler.FindObject(info);
        public bool FindGeneric<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new() => _findPoolHandler.FindGeneric<T>(poolInfo);
        public PoolInfo RegisterPool(GameObject poolPrefab, int prefabID) => _createPoolHandler.RegisterPool(poolPrefab, prefabID);
        public GenericPoolInfo RegisterGenericPool<T>() where T : class, IPoolGeneric, new() => _createPoolHandler.RegisterGenericPool<T>();
        public PooledObject AddPoolObjectComponent(GameObject instance, PoolInfo info) => _createPoolHandler.AddPoolObjectComponent(instance, info);
        public void SleepRigidbody(PooledObject instance) => _setRbHandler.SleepRigidbody(instance);
        public void WakeUpRigidBody(PooledObject instance) => _setRbHandler.WakeUpRigidBody(instance);
        public IEnumerator IsActiveRoutine(int id) => _lifeHandler.IsActiveRoutine(id);
        public IEnumerator IsActiveGenericRoutine<T>() where T : class, IPoolGeneric, new() => _lifeHandler.IsActiveGenericRoutine<T>();

        /// <summary>
        /// 지정된 시간만큼 대기하는 WaitForSeconds 객체를 반환합니다. 이미 생성된 객체가 있으면 재사용합니다.
        /// </summary>
        public WaitForSeconds Second(float time)
        {
            float normalize = Mathf.Round(time * 100f) * 0.01f;

            if (DelayDic.ContainsKey(normalize) == false)
            {
                DelayDic.Add(normalize, new WaitForSeconds(normalize));
            }
            return DelayDic[normalize];
        }

        private void SetSingleTon()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }
        private void SetHandler()
        {
            _getHandler = new AutoPoolGetHandler(this);
            _returnHandler = new AutoPoolReturnHandler(this);
            _preloadHandler = new AutoPoolPreloadHandler(this);
            _findPoolHandler = new AutoPoolFindPoolHandler(this);
            _createPoolHandler = new AutoPoolCreatePoolHandler(this);
            _setRbHandler = new AutoPoolSetRbHandler(this);
            _lifeHandler = new AutoPoolLifeHandler(this);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnRuntimeLoad()
        {
            s_isApplicationQuit = false;
        }
        private void OnApplicationQuit()
        {
            s_isApplicationQuit = true;
        }
    }
}