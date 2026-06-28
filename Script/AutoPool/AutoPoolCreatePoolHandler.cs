using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Creates and registers new GameObject and generic pool entries.
    /// </summary>
    public class AutoPoolCreatePoolHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolCreatePoolHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// Creates a child GameObject to serve as the pool's parent transform,
        /// initialises an empty stack, and registers the entry in PoolDic.
        /// </summary>
        public PoolInfo RegisterPool(GameObject poolPrefab, int prefabID)
        {
            Transform newParent = new GameObject(poolPrefab.name).transform;
            newParent.SetParent(_autoPool.transform, true);

            Stack<PooledObject> newPool = new Stack<PooledObject>();

            PoolInfo newPoolInfo = GetPoolInfo(newPool, poolPrefab, newParent);
            _autoPool.PoolDic.Add(prefabID, newPoolInfo);

            return newPoolInfo;
        }

        /// <summary>
        /// Creates an empty generic pool stack for type <typeparamref name="T"/>
        /// and registers the entry in GenericPoolDic.
        /// </summary>
        public GenericPoolInfo RegisterGenericPool<T>() where T : class, IPoolGeneric, new()
        {
            Stack<IPoolGeneric> newPool = new Stack<IPoolGeneric>();
            GenericPoolInfo genericPoolInfo = GetGenericPoolInfo<T>(newPool);

            _autoPool.GenericPoolDic.Add(typeof(T), genericPoolInfo);

            return genericPoolInfo;
        }

        /// <summary>
        /// Adds (or retrieves) a PooledObject component on the instance,
        /// links it to the pool, and subscribes the dormant-destroy event.
        /// </summary>
        public PooledObject AddPoolObjectComponent(GameObject instance, PoolInfo info)
        {
            PooledObject poolObject = instance.GetOrAddComponent<PooledObject>();
            poolObject.PoolInfo = info;
            info.PoolCount++;
            poolObject.SubscribePoolDeactivateEvent();

            return poolObject;
        }

        private PoolInfo GetPoolInfo(Stack<PooledObject> pool, GameObject prefab, Transform parent)
        {
            PoolInfo info = new PoolInfo();
            info.Pool = pool;
            info.Parent = parent;
            info.Prefab = prefab;
            return info;
        }

        private GenericPoolInfo GetGenericPoolInfo<T>(Stack<IPoolGeneric> pool) where T : class, new()
        {
            GenericPoolInfo genericPool = new GenericPoolInfo();
            genericPool.Pool = pool;
            genericPool.Type = typeof(T);
            return genericPool;
        }
    }
}
