using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    public class AutoPoolFindPoolHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolFindPoolHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// �ش� �����տ� ���� Ǯ ������ ã�ų�, ������ ���� �����մϴ�.
        /// �������� �ν��Ͻ� ID�� �������� Dictionary���� �����˴ϴ�.
        /// </summary>
        public PoolInfo FindPool(GameObject poolPrefab)
        {
            if (poolPrefab == null)
            {
                Debug.LogError($"{poolPrefab}�� �����Ǿ� ���� �ʽ��ϴ�");
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
        /// �ش� �����տ� ���� Ǯ ������ ã�ų�, ������ ���� �����մϴ�.
        /// �������� �ν��Ͻ� ID�� �������� Dictionary���� �����˴ϴ�.
        /// </summary>
        public PoolInfo FindResourcesPool(string resources)
        {
            Dictionary<string, int> resourcePool = _autoPool.ResourcesPoolDic;
            PoolInfo pool = default;
            if (resourcePool.ContainsKey(resources) == false)
            {
                // ���ҽý� ������ �ε�
                GameObject prefab = Resources.Load<GameObject>(resources);
                if (prefab == null)
                {
                    Debug.LogError($"Resources�� {resources}�� ��ġ�ϴ� ���ҽ��� �����ϴ�");
                    return null;
                }
                // ������ instanceID�� ĳ��
                int prefabID = prefab.GetInstanceID();
                // Ǯ�� ���
                _autoPool.RegisterPool(prefab, prefabID);
                // ���ҽý� Ǯ�� ���
                resourcePool.Add(resources, prefabID);
            }

            pool = _autoPool.PoolDic[resourcePool[resources]];
            pool.IsUsed = true;
            _autoPool.PoolDic[resourcePool[resources]] = pool;
            return pool;
        }

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
        /// ���� Ǯ�� ���� ������ ������Ʈ�� �����ϴ��� Ȯ���մϴ�.
        /// null ������Ʈ�� �� ������ �����մϴ�.
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

                // null ����
                info.Pool.Pop();
            }
            return true;
        }

        public bool FindGeneric<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new()
        {
            if(poolInfo == null) return false;
            IPoolGeneric instance = null;

            while (true)
            {
                if (poolInfo.Pool.Count <= 0)
                    return false;
                instance = poolInfo.Pool.Peek();
                if (instance != null)
                    break;
                // null ����
                poolInfo.Pool.Pop();
            }
            return true;
        }
    }
}