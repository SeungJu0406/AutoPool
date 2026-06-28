using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Locates existing pool entries for prefab, Resources, and generic types,
    /// creating new entries when none exists.
    /// </summary>
    public class AutoPoolFindPoolHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolFindPoolHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// Returns the pool entry for the given prefab, registering a new one if necessary.
        /// </summary>
        public PoolInfo FindPool(GameObject poolPrefab)
        {
            if (poolPrefab == null)
            {
                Debug.LogError($"{poolPrefab} is not referenced.");
                return null;
            }

            int prefabID = poolPrefab.GetInstanceID();

            PoolInfo pool = default;
            if (_autoPool.PoolDic.ContainsKey(prefabID) == false)
                _autoPool.RegisterPool(poolPrefab, prefabID);

            pool = _autoPool.PoolDic[prefabID];
            pool.IsUsed = true;
            _autoPool.PoolDic[prefabID] = pool;
            return pool;
        }

        /// <summary>
        /// Returns the pool entry for the given Resources path, loading and registering
        /// the prefab on first access.
        /// </summary>
        public PoolInfo FindResourcesPool(string resources)
        {
            Dictionary<string, int> resourcePool = _autoPool.ResourcesPoolDic;
            PoolInfo pool = default;
            if (resourcePool.ContainsKey(resources) == false)
            {
                GameObject prefab = Resources.Load<GameObject>(resources);
                if (prefab == null)
                {
                    Debug.LogError($"There's no resource in Resources that matches {resources}.");
                    return null;
                }

                int prefabID = prefab.GetInstanceID();
                _autoPool.RegisterPool(prefab, prefabID);
                resourcePool.Add(resources, prefabID);
            }

            pool = _autoPool.PoolDic[resourcePool[resources]];
            pool.IsUsed = true;
            _autoPool.PoolDic[resourcePool[resources]] = pool;
            return pool;
        }

        /// <summary>
        /// Returns the generic pool entry for type <typeparamref name="T"/>,
        /// registering a new one if necessary.
        /// </summary>
        public GenericPoolInfo FindGenericPool<T>() where T : class, IPoolGeneric, new()
        {
            GenericPoolInfo genericPool = default;
            if (_autoPool.GenericPoolDic.ContainsKey(typeof(T)) == false)
                _autoPool.RegisterGenericPool<T>();

            genericPool = _autoPool.GenericPoolDic[typeof(T)];
            genericPool.IsUsed = true;
            _autoPool.GenericPoolDic[typeof(T)] = genericPool;
            return genericPool;
        }

        /// <summary>
        /// Returns true if the pool stack contains at least one non-null instance.
        /// Silently purges any null entries (destroyed objects) from the top of the stack.
        /// </summary>
        public bool FindObject(PoolInfo info)
        {
            if (info == null) return false;

            PooledObject instance = null;
            while (true)
            {
                if (info.Pool.Count <= 0)
                    return false;

                instance = info.Pool.Peek();
                if (instance != null)
                    break;

                info.Pool.Pop();
            }
            return true;
        }

        /// <summary>
        /// Returns true if the generic pool stack contains at least one non-null instance.
        /// Silently purges null entries from the top of the stack.
        /// </summary>
        public bool FindGeneric<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new()
        {
            if (poolInfo == null) return false;
            IPoolGeneric instance = null;

            while (true)
            {
                if (poolInfo.Pool.Count <= 0)
                    return false;

                instance = poolInfo.Pool.Peek();
                if (instance != null)
                    break;

                poolInfo.Pool.Pop();
            }
            return true;
        }
    }
}
