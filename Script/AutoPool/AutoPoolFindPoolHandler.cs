using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 프리팹, Resources, 제네릭 타입 기준으로 풀을 검색·지연 생성하는 핸들러입니다.
    /// </summary>
    public class AutoPoolFindPoolHandler
    {
        /// <summary>
        /// 풀 딕셔너리를 보유한 메인 풀 객체입니다.
        /// </summary>
        MainAutoPool _autoPool;

        /// <summary>
        /// 지정된 메인 풀 인스턴스로 풀 검색 핸들러를 초기화합니다.
        /// </summary>
        public AutoPoolFindPoolHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// 프리팹을 키로 사용하는 풀을 찾거나, 없으면 새로 등록한 뒤 반환합니다.
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
            {
                _autoPool.RegisterPool(poolPrefab, prefabID);
            }
            pool = _autoPool.PoolDic[prefabID];
            pool.IsUsed = true;
            _autoPool.PoolDic[prefabID] = pool;
            return pool;
        }

        /// <summary>
        /// Resources 경로 문자열을 키로 사용하는 풀을 찾거나, 없으면 새로 등록한 뒤 반환합니다.
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
        /// 제네릭 타입 <typeparamref name="T"/> 에 대한 제네릭 풀을 찾거나, 없으면 새로 등록한 뒤 반환합니다.
        /// </summary>
        public GenericPoolInfo FindGenericPool<T>() where T : class, IPoolGeneric, new()
        {
            GenericPoolInfo genericPool = default;
            if (_autoPool.GenericPoolDic.ContainsKey(typeof(T)) == false)
            {
                _autoPool.RegisterGenericPool<T>();
            }
            genericPool = _autoPool.GenericPoolDic[typeof(T)];
            genericPool.IsUsed = true;
            _autoPool.GenericPoolDic[typeof(T)] = genericPool;
            return genericPool;
        }

        /// <summary>
        /// 지정된 풀에서 유효한 GameObject 인스턴스가 존재하는지 검사하고, null 항목은 스택에서 제거합니다.
        /// </summary>
        public bool FindObject(PoolInfo info)
        {
            if (info == null) return false;

            GameObject instance = null;
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
        /// 지정된 제네릭 풀에서 유효한 인스턴스가 존재하는지 검사하고, null 항목은 스택에서 제거합니다.
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