using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    public class AutoPoolPreloadHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolPreloadHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            return ProcessPreload(info, count);
        }

        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            return ProcessPreload(info, count);
        }

        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return ProcessPreload(info, count);
        }
        private IPoolInfoReadOnly ProcessPreload(PoolInfo info, int count)
        {
            if (info == null)
            {
                Debug.LogError("The pool information is invalid.");
                return null;
            }
            while (info.PoolCount < count)
            {
                GameObject instance = GameObject.Instantiate(info.Prefab);
                PooledObject poolObject = _autoPool.AddPoolObjectComponent(instance, info);
                instance.transform.SetParent(info.Parent);
                info.Pool.Push(instance);
                info.ActiveCount++;
                instance.gameObject.SetActive(false);
            }
            return info;
        }

        public IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            ClearPool(info);
            return info;
        }

        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            ClearPool(info);
            return info;
        }

        public IPoolInfoReadOnly ClearResourcesPool(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            ClearPool(info);
            return info;
        }

        public IGenericPoolInfoReadOnly ClearGenericPool<T>() where T : class, IPoolGeneric, new()
        {
            GenericPoolInfo info = _autoPool.FindGenericPool<T>();
            ClearGenericPool(info);
            return info;
        }

        public void ClearPool(PoolInfo info)
        {
            info.OnPoolDormant?.Invoke();

            info.Pool = new Stack<GameObject>();
            info.IsActive = false;
        }

        public void ClearGenericPool(GenericPoolInfo info)
        {
            info.OnPoolDormant?.Invoke();
            info.Pool = new Stack<IPoolGeneric>();
            info.IsActive = false;
        }
    }
}