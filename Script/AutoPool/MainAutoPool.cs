using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AutoPool_Tool
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // © 2025 NSJ. All rights reserved.

    public class MainAutoPool : MonoBehaviour, IObjectPool
    {
        private static bool s_isApplicationQuit = false;



        public static MainAutoPool CreatePool()
        {
            if (s_isApplicationQuit == true)
                return null;
            GameObject newPool = new GameObject("MainAutoPool");
            MainAutoPool pool = newPool.AddComponent<MainAutoPool>();
            return pool;
        }


        public Dictionary<int, PoolInfo> PoolDic = new Dictionary<int, PoolInfo>();

        public Dictionary<string, int> ResourcesPoolDic = new Dictionary<string, int>();

        public Dictionary<Type, GenericPoolInfo> GenericPoolDic = new Dictionary<Type, GenericPoolInfo>();

        public Dictionary<float, WaitForSeconds> DelayDic = new Dictionary<float, WaitForSeconds>();

        private AutoPoolGetHandler _getHandler;
        private AutoPoolReturnHandler _returnHandler;
        private AutoPoolPreloadHandler _preloadHandler;
        private AutoPoolFindPoolHandler _findPoolHandler;
        private AutoPoolCreatePoolHandler _createPoolHandler;
        private AutoPoolSetRbHandler _setRbHandler;
#if UNITY_EDITOR
        public List<IPoolInfoReadOnly> GetAllPoolInfos()
        {
            return PoolDic.Values.Cast<IPoolInfoReadOnly>().ToList();
        }
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

        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count) => _preloadHandler.SetPreload(prefab, count);

        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component => _preloadHandler.SetPreload(prefab, count);

        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count) => _preloadHandler.SetResourcesPreload(resources, count);

        public IPoolInfoReadOnly ClearPool(GameObject prefab) => _preloadHandler.ClearPool(prefab);

        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component => _preloadHandler.ClearPool(prefab);

        public IPoolInfoReadOnly ClearResourcesPool(string resources) => _preloadHandler.ClearResourcesPool(resources);
        public IGenericPoolInfoReadOnly ClearGenericPool<T>() where T : class, IPoolGeneric, new() => _preloadHandler.ClearGenericPool<T>();
        public void ClearPool(PoolInfo info) => _preloadHandler.ClearPool(info);
        #endregion
        #region GetPool
        #region Common

        public GameObject Get(GameObject prefab) => _getHandler.Get(prefab);

        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _getHandler.Get(prefab, transform, worldPositionStay);

        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _getHandler.Get(prefab, pos, rot);

        public T Get<T>(T prefab) where T : Component => _getHandler.Get(prefab);

        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _getHandler.Get(prefab, transform, worldPositionStay);

        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _getHandler.Get(prefab, pos, rot);
        #endregion
        #region Resources

        public GameObject ResourcesGet(string resources) => _getHandler.ResourcesGet(resources);
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _getHandler.ResourcesGet(resources, transform, worldPositionStay);

        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _getHandler.ResourcesGet(resources, pos, rot);
        public T ResourcesGet<T>(string resources) where T : Component => _getHandler.ResourcesGet<T>(resources);

        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _getHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _getHandler.ResourcesGet<T>(resources, pos, rot);
        #endregion
        #region Generic
        public T GenericPool<T>() where T : class, IPoolGeneric, new() => _getHandler.GenericGet<T>();
        #endregion
        #endregion
        #region ReturnPool

        public IPoolInfoReadOnly Return(GameObject instance) => _returnHandler.Return(instance);

        public IPoolInfoReadOnly Return<T>(T instance) where T : Component => _returnHandler.Return(instance);

        public void Return(GameObject instance, float delay) => _returnHandler.Return(instance, delay);

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

        public WaitForSeconds Second(float time)
        {
            float normalize = Mathf.Round(time * 100f) * 0.01f;

            if (DelayDic.ContainsKey(normalize) == false)
            {
                DelayDic.Add(normalize, new WaitForSeconds(normalize));
            }
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