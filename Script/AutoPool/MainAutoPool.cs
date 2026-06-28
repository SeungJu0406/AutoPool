using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Central pool manager for the scene.
    /// Integrates create, get, return, and preload operations for prefab, Resources, and generic pools.
    /// </summary>
    public class MainAutoPool : MonoBehaviour, IObjectPool
    {
        /// <summary>
        /// Prevents new pool GameObjects from being created during application shutdown.
        /// </summary>
        private static bool s_isApplicationQuit = false;

        /// <summary>
        /// Creates a new MainAutoPool GameObject and attaches the component.
        /// Returns null if the application is quitting.
        /// </summary>
        public static MainAutoPool CreatePool()
        {
            if (s_isApplicationQuit == true)
                return null;

            GameObject newPool = new GameObject("MainAutoPool");
            MainAutoPool pool = newPool.AddComponent<MainAutoPool>();
            return pool;
        }

        /// <summary>
        /// Prefab instance-ID keyed dictionary of all GameObject pools.
        /// </summary>
        public Dictionary<int, PoolInfo> PoolDic = new Dictionary<int, PoolInfo>();

        /// <summary>
        /// Maps a Resources path string to its corresponding prefab instance ID.
        /// </summary>
        public Dictionary<string, int> ResourcesPoolDic = new Dictionary<string, int>();

        /// <summary>
        /// Type-keyed dictionary of all generic pools.
        /// </summary>
        public Dictionary<Type, GenericPoolInfo> GenericPoolDic = new Dictionary<Type, GenericPoolInfo>();

        /// <summary>
        /// Cache of WaitForSeconds instances keyed by duration to avoid repeated allocations.
        /// </summary>
        public Dictionary<float, WaitForSeconds> DelayDic = new Dictionary<float, WaitForSeconds>();

        private AutoPoolGetHandler _getHandler;
        private AutoPoolReturnHandler _returnHandler;
        private AutoPoolPreloadHandler _preloadHandler;
        private AutoPoolFindPoolHandler _findPoolHandler;
        private AutoPoolCreatePoolHandler _createPoolHandler;
        private AutoPoolSetRbHandler _setRbHandler;

#if UNITY_EDITOR
        /// <summary>
        /// Returns read-only info for all GameObject pools (editor use only).
        /// </summary>
        public List<IPoolInfoReadOnly> GetAllPoolInfos()
        {
            return PoolDic.Values.Cast<IPoolInfoReadOnly>().ToList();
        }

        /// <summary>
        /// Returns read-only info for all generic pools (editor use only).
        /// </summary>
        public List<IGenericPoolInfoReadOnly> GetAllGenericPoolInfos()
        {
            return GenericPoolDic.Values.Cast<IGenericPoolInfoReadOnly>().ToList();
        }
#endif

        private void Awake()
        {
            SetHandler();
        }

        #region GetInfo

        /// <summary>Returns pool info for the given prefab.</summary>
        public IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            return FindPool(prefab);
        }

        /// <summary>Returns pool info for the given Component prefab.</summary>
        public IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            return FindPool(prefab.gameObject);
        }

        /// <summary>Returns pool info for a Resources-loaded prefab.</summary>
        public IPoolInfoReadOnly GetResourcesInfo(string resources)
        {
            return FindResourcesPool(resources);
        }

        #endregion

        #region SePreload

        /// <summary>Pre-warms the prefab pool to at least <paramref name="count"/> instances.</summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count) => _preloadHandler.SetPreload(prefab, count);

        /// <summary>Pre-warms the Component prefab pool to at least <paramref name="count"/> instances.</summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component => _preloadHandler.SetPreload(prefab, count);

        /// <summary>Pre-warms the Resources pool to at least <paramref name="count"/> instances.</summary>
        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count) => _preloadHandler.SetResourcesPreload(resources, count);

        /// <summary>Destroys all pooled instances for the given prefab.</summary>
        public IPoolInfoReadOnly ClearPool(GameObject prefab) => _preloadHandler.ClearPool(prefab);

        /// <summary>Destroys all pooled instances for the given Component prefab.</summary>
        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component => _preloadHandler.ClearPool(prefab);

        /// <summary>Destroys all pooled instances for the given Resources path.</summary>
        public IPoolInfoReadOnly ClearResourcesPool(string resources) => _preloadHandler.ClearResourcesPool(resources);

        /// <summary>Destroys all instances in the generic pool for type <typeparamref name="T"/>.</summary>
        public IGenericPoolInfoReadOnly ClearGenericPool<T>() where T : class, IPoolGeneric, new() => _preloadHandler.ClearGenericPool<T>();

        /// <summary>Destroys all instances described by the given PoolInfo directly.</summary>
        public void ClearPool(PoolInfo info) => _preloadHandler.ClearPool(info);

        #endregion

        #region GetPool

        #region Common

        /// <summary>Retrieves a GameObject instance from the pool.</summary>
        public GameObject Get(GameObject prefab) => _getHandler.Get(prefab);

        /// <summary>Retrieves a GameObject instance and parents it under <paramref name="transform"/>.</summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _getHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>Retrieves a GameObject instance at the given position and rotation.</summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _getHandler.Get(prefab, pos, rot);

        /// <summary>Retrieves a Component instance from the pool.</summary>
        public T Get<T>(T prefab) where T : Component => _getHandler.Get(prefab);

        /// <summary>Retrieves a Component instance and parents it under <paramref name="transform"/>.</summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _getHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>Retrieves a Component instance at the given position and rotation.</summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _getHandler.Get(prefab, pos, rot);

        #endregion

        #region Resources

        /// <summary>Retrieves a GameObject from the Resources pool.</summary>
        public GameObject ResourcesGet(string resources) => _getHandler.ResourcesGet(resources);

        /// <summary>Retrieves a Resources-path GameObject and parents it under <paramref name="transform"/>.</summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _getHandler.ResourcesGet(resources, transform, worldPositionStay);

        /// <summary>Retrieves a Resources-path GameObject at the given position and rotation.</summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _getHandler.ResourcesGet(resources, pos, rot);

        /// <summary>Retrieves a Component from the Resources pool.</summary>
        public T ResourcesGet<T>(string resources) where T : Component => _getHandler.ResourcesGet<T>(resources);

        /// <summary>Retrieves a Resources-path Component and parents it under <paramref name="transform"/>.</summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _getHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        /// <summary>Retrieves a Resources-path Component at the given position and rotation.</summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _getHandler.ResourcesGet<T>(resources, pos, rot);

        #endregion

        #region Generic

        /// <summary>Retrieves an instance of <typeparamref name="T"/> from the generic pool.</summary>
        public T GenericPool<T>() where T : class, IPoolGeneric, new() => _getHandler.GenericGet<T>();

        #endregion

        #endregion

        #region ReturnPool

        /// <summary>Returns a GameObject instance to the pool.</summary>
        public IPoolInfoReadOnly Return(GameObject instance) => _returnHandler.Return(instance);

        /// <summary>Returns a Component instance to the pool.</summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component => _returnHandler.Return(instance);

        /// <summary>Returns a GameObject instance to the pool after a delay.</summary>
        public void Return(GameObject instance, float delay) => _returnHandler.Return(instance, delay);

        /// <summary>Returns a Component instance to the pool after a delay.</summary>
        public void Return<T>(T instance, float delay) where T : Component => _returnHandler.Return(instance, delay);

        /// <summary>Returns a generic instance and provides updated pool info.</summary>
        public IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new() => _returnHandler.GenericReturn(instance);

        /// <summary>Returns a generic instance after a delay.</summary>
        public void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new() => _returnHandler.GenericReturn(instance, delay);

        #endregion

        /// <summary>Finds or creates a pool entry for the given prefab.</summary>
        public PoolInfo FindPool(GameObject poolPrefab) => _findPoolHandler.FindPool(poolPrefab);

        /// <summary>Finds or creates a pool entry for the given Resources path.</summary>
        public PoolInfo FindResourcesPool(string resources) => _findPoolHandler.FindResourcesPool(resources);

        /// <summary>Finds or creates a generic pool entry for type <typeparamref name="T"/>.</summary>
        public GenericPoolInfo FindGenericPool<T>() where T : class, IPoolGeneric, new() => _findPoolHandler.FindGenericPool<T>();

        /// <summary>Returns true if the pool stack contains at least one valid (non-null) instance.</summary>
        public bool FindObject(PoolInfo info) => _findPoolHandler.FindObject(info);

        /// <summary>Returns true if the generic pool stack contains at least one valid instance.</summary>
        public bool FindGeneric<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new() => _findPoolHandler.FindGeneric<T>(poolInfo);

        /// <summary>Creates a new GameObject pool entry and registers it in PoolDic.</summary>
        public PoolInfo RegisterPool(GameObject poolPrefab, int prefabID) => _createPoolHandler.RegisterPool(poolPrefab, prefabID);

        /// <summary>Creates a new generic pool entry and registers it in GenericPoolDic.</summary>
        public GenericPoolInfo RegisterGenericPool<T>() where T : class, IPoolGeneric, new() => _createPoolHandler.RegisterGenericPool<T>();

        /// <summary>Adds or retrieves a PooledObject component on the instance and links it to the pool.</summary>
        public PooledObject AddPoolObjectComponent(GameObject instance, PoolInfo info) => _createPoolHandler.AddPoolObjectComponent(instance, info);

        /// <summary>Zeros velocity and puts the instance's Rigidbody/Rigidbody2D to sleep.</summary>
        public void SleepRigidbody(PooledObject instance) => _setRbHandler.SleepRigidbody(instance);

        /// <summary>Zeros velocity and wakes the instance's Rigidbody/Rigidbody2D.</summary>
        public void WakeUpRigidBody(PooledObject instance) => _setRbHandler.WakeUpRigidBody(instance);

        /// <summary>
        /// Returns a cached WaitForSeconds for the given duration, rounded to two decimal places.
        /// </summary>
        public WaitForSeconds Second(float time)
        {
            float normalize = Mathf.Round(time * 100f) * 0.01f;

            if (DelayDic.ContainsKey(normalize) == false)
                DelayDic.Add(normalize, new WaitForSeconds(normalize));

            return DelayDic[normalize];
        }

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
        /// Resets the quit flag before each scene load so the pool can be recreated.
        /// </summary>
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
