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
            // 풀용 부모 오브젝트 생성 후 계층 구조 정리
            Transform newParent = new GameObject(poolPrefab.name).transform;
            newParent.SetParent(_autoPool.transform, true); // parent

            // 새로운 풀 스택과 정보 생성
            Stack<GameObject> newPool = new Stack<GameObject>(); // pool
            PoolInfo newPoolInfo = GetPoolInfo(newPool, poolPrefab, newParent);

            // 풀 딕셔너리 추가
            _autoPool.PoolDic.Add(prefabID, newPoolInfo);

            // 비활성화 여부 감지 코루틴 시작
            _autoPool.StartCoroutine(_autoPool.IsActiveRoutine(prefabID));
            return newPoolInfo;
        }

        public GenericPoolInfo RegisterGenericPool<T>() where T : class, IPoolGeneric,new()
        {
            // 새로운 풀 스택과 정보 생성
            Stack<IPoolGeneric> newPool = new Stack<IPoolGeneric>();
            GenericPoolInfo genericPoolInfo = GetGenericPoolInfo<T>(newPool);
            // 풀 딕셔너리 추가
            _autoPool.GenericPoolDic.Add(typeof(T), genericPoolInfo);
            // 비활성화 여부 감지 코루틴 시작
            _autoPool.StartCoroutine(_autoPool.IsActiveGenericRoutine<T>());
            return genericPoolInfo;
        }
        /// <summary>
        /// PooledObject 컴포넌트를 오브젝트에 추가하거나 가져옵니다.
        /// PoolInfo를 연결하고, 풀 개수를 증가시키며, 자동 비활성화 이벤트를 구독합니다.
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
        /// 풀 정보 생성
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