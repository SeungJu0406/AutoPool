using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 풀과 관련된 GameObject/제네릭 풀을 생성하고 등록하는 핸들러 클래스입니다.
    /// </summary>
    public class AutoPoolCreatePoolHandler
    {
        /// <summary>
        /// 풀 딕셔너리 및 트랜스폼을 보유한 메인 풀 객체입니다.
        /// </summary>
        MainAutoPool _autoPool;

        /// <summary>
        /// 지정된 메인 풀 인스턴스로 풀 생성 핸들러를 초기화합니다.
        /// </summary>
        public AutoPoolCreatePoolHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// 프리팹을 기반으로 새로운 GameObject 풀을 생성하고 메인 풀에 등록합니다.
        /// </summary>
        public PoolInfo RegisterPool(GameObject poolPrefab, int prefabID)
        {
            Transform newParent = new GameObject(poolPrefab.name).transform; // 풀 루트 GameObject 생성
            newParent.SetParent(_autoPool.transform, true);                  // 메인 풀 아래에 부모 설정

            Stack<GameObject> newPool = new Stack<GameObject>();            // 실제 오브젝트를 담을 스택 생성

            PoolInfo newPoolInfo = GetPoolInfo(newPool, poolPrefab, newParent); // PoolInfo 구성
            _autoPool.PoolDic.Add(prefabID, newPoolInfo);                       // 딕셔너리에 등록

            return newPoolInfo;
        }

        /// <summary>
        /// 제네릭 타입 <typeparamref name="T"/> 에 대한 새로운 제네릭 풀을 생성하고 등록합니다.
        /// </summary>
        public GenericPoolInfo RegisterGenericPool<T>() where T : class, IPoolGeneric, new()
        {
            Stack<IPoolGeneric> newPool = new Stack<IPoolGeneric>();             // 제네릭 풀 스택 생성
            GenericPoolInfo genericPoolInfo = GetGenericPoolInfo<T>(newPool);    // GenericPoolInfo 구성

            _autoPool.GenericPoolDic.Add(typeof(T), genericPoolInfo);            // 타입 키로 등록

            return genericPoolInfo;
        }

        /// <summary>
        /// 인스턴스에 <see cref="PooledObject"/> 컴포넌트를 부착(또는 재사용)하고 풀 정보와 이벤트를 설정합니다.
        /// </summary>
        public PooledObject AddPoolObjectComponent(GameObject instance, PoolInfo info)
        {
            PooledObject poolObject = instance.GetOrAddComponent<PooledObject>(); // 없으면 추가, 있으면 재사용
            poolObject.PoolInfo = info;                                           // 풀 정보 연결
            info.PoolCount++;                                                     // 풀에 속한 개수 증가
            poolObject.SubscribePoolDeactivateEvent();                            // 풀 휴면 시 파괴 이벤트 구독

            return poolObject;
        }

        /// <summary>
        /// GameObject 풀에 사용될 <see cref="PoolInfo"/> 인스턴스를 초기화합니다.
        /// </summary>
        private PoolInfo GetPoolInfo(Stack<GameObject> pool, GameObject prefab, Transform parent)
        {
            PoolInfo info = new PoolInfo();
            info.Pool = pool;
            info.Parent = parent;
            info.Prefab = prefab;
            return info;
        }

        /// <summary>
        /// 제네릭 풀에 사용될 <see cref="GenericPoolInfo"/> 인스턴스를 초기화합니다.
        /// </summary>
        private GenericPoolInfo GetGenericPoolInfo<T>(Stack<IPoolGeneric> pool) where T : class, new()
        {
            GenericPoolInfo genericPool = new GenericPoolInfo();
            genericPool.Pool = pool;
            genericPool.Type = typeof(T);
            return genericPool;
        }
    }
}