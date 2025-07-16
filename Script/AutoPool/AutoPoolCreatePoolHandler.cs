using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolCreatePoolHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolCreatePoolHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        public PoolInfo RegisterPool(GameObject poolPrefab, int prefabID)
        {
            // Ǯ�� �θ� ������Ʈ ���� �� ���� ���� ����
            Transform newParent = new GameObject(poolPrefab.name).transform;
            newParent.SetParent(_autoPool.transform, true); // parent

            // ���ο� Ǯ ���ð� ���� ����
            Stack<GameObject> newPool = new Stack<GameObject>(); // pool
            PoolInfo newPoolInfo = GetPoolInfo(newPool, poolPrefab, newParent);

            // Ǯ ��ųʸ� �߰�
            _autoPool.PoolDic.Add(prefabID, newPoolInfo);

            // ��Ȱ��ȭ ���� ���� �ڷ�ƾ ����
            _autoPool.StartCoroutine(_autoPool.IsActiveRoutine(prefabID));
            return newPoolInfo;
        }

        public GenericPoolInfo RegisterGenericPool<T>() where T : class, IPoolGeneric,new()
        {
            // ���ο� Ǯ ���ð� ���� ����
            Stack<IPoolGeneric> newPool = new Stack<IPoolGeneric>();
            GenericPoolInfo genericPoolInfo = GetGenericPoolInfo<T>(newPool);
            // Ǯ ��ųʸ� �߰�
            _autoPool.GenericPoolDic.Add(typeof(T), genericPoolInfo);
            // ��Ȱ��ȭ ���� ���� �ڷ�ƾ ����
            _autoPool.StartCoroutine(_autoPool.IsActiveGenericRoutine<T>());
            return genericPoolInfo;
        }
        /// <summary>
        /// PooledObject ������Ʈ�� ������Ʈ�� �߰��ϰų� �����ɴϴ�.
        /// PoolInfo�� �����ϰ�, Ǯ ������ ������Ű��, �ڵ� ��Ȱ��ȭ �̺�Ʈ�� �����մϴ�.
        /// </summary>
        public PooledObject AddPoolObjectComponent(GameObject instance, PoolInfo info)
        {
            PooledObject poolObject = instance.GetOrAddComponent<PooledObject>();
            poolObject.PoolInfo = info;
            info.PoolCount++;
            poolObject.SubscribePoolDeactivateEvent();

            return poolObject;
        }
        /// <summary>
        /// Ǯ ���� ����
        /// </summary>
        private PoolInfo GetPoolInfo(Stack<GameObject> pool, GameObject prefab, Transform parent)
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