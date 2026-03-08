using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 풀 및 제네릭 풀의 선로딩(Preload)과 비우기(Clear)를 담당하는 핸들러입니다.
    /// </summary>
    public class AutoPoolPreloadHandler
    {
        /// <summary>
        /// 실제 풀 딕셔너리와 유틸리티를 보유한 메인 풀 인스턴스입니다.
        /// </summary>
        MainAutoPool _autoPool;

        /// <summary>
        /// 지정된 메인 풀 인스턴스로 프리로드 핸들러를 초기화합니다.
        /// </summary>
        public AutoPoolPreloadHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// 프리팹 기준 풀을 찾고, 지정 수량만큼 선로딩을 설정합니다.
        /// </summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// 컴포넌트 프리팹 기준 풀을 찾고, 지정 수량만큼 선로딩을 설정합니다.
        /// </summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// Resources 경로 기준 풀을 찾고, 지정 수량만큼 선로딩을 설정합니다.
        /// </summary>
        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// 공통 프리로드 처리 로직으로, 지정된 수량에 도달할 때까지 인스턴스를 생성하여 풀에 채웁니다.
        /// </summary>
        private IPoolInfoReadOnly ProcessPreload(PoolInfo info, int count)
        {
            if (info == null)
            {
                Debug.LogError("The pool information is invalid.");
                return null;
            }

            // 목표 수량(count)에 도달할 때까지 프리팹 인스턴스를 생성
            while (info.PoolCount < count)
            {
                GameObject instance = GameObject.Instantiate(info.Prefab);       // 1) 프리팹으로 새 인스턴스 생성
                PooledObject poolObject = _autoPool.AddPoolObjectComponent(instance, info); // 2) PooledObject 부착 및 PoolInfo 연결
                instance.transform.SetParent(info.Parent);                       // 3) 풀 전용 부모 트랜스폼 아래로 정리
                info.Pool.Push(instance);                                        // 4) 풀 스택에 푸시
                info.ActiveCount++;                                              // 5) 활성 카운트 증가 (초기화 정책에 따라 사용)
                instance.gameObject.SetActive(false);                            // 6) 비활성화하여 대기 상태로 전환
            }

            return info;
        }

        /// <summary>
        /// 프리팹 기준 풀을 찾아 내부 객체들을 정리(초기화)합니다.
        /// </summary>
        public IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            ClearPool(info);
            return info;
        }

        /// <summary>
        /// 컴포넌트 프리팹 기준 풀을 찾아 내부 객체들을 정리(초기화)합니다.
        /// </summary>
        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            ClearPool(info);
            return info;
        }

        /// <summary>
        /// Resources 경로 기준 풀을 찾아 내부 객체들을 정리(초기화)합니다.
        /// </summary>
        public IPoolInfoReadOnly ClearResourcesPool(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            ClearPool(info);
            return info;
        }

        /// <summary>
        /// 제네릭 타입 <typeparamref name="T"/> 에 대한 제네릭 풀을 찾아 내부 객체들을 정리합니다.
        /// </summary>
        public IGenericPoolInfoReadOnly ClearGenericPool<T>() where T : class, IPoolGeneric, new()
        {
            GenericPoolInfo info = _autoPool.FindGenericPool<T>();
            ClearGenericPool(info);
            return info;
        }

        /// <summary>
        /// GameObject 풀의 모든 객체를 비우고 비활성 상태로 표시합니다.
        /// </summary>
        public void ClearPool(PoolInfo info)
        {
            info.OnPoolDormant?.Invoke();                       // 1) 풀 휴면 콜백 호출 (구독된 오브젝트 정리 등)

            info.Pool = new Stack<GameObject>();                // 2) 새 스택으로 교체하여 기존 레퍼런스 제거
            info.IsActive = false;                              // 3) 풀 비활성 상태 플래그 설정
        }

        /// <summary>
        /// 제네릭 풀의 모든 객체를 비우고 비활성 상태로 표시합니다.
        /// </summary>
        public void ClearGenericPool(GenericPoolInfo info)
        {
            info.OnPoolDormant?.Invoke();                       // 1) 제네릭 풀 휴면 콜백 호출
            info.Pool = new Stack<IPoolGeneric>();              // 2) 새 스택으로 교체하여 기존 레퍼런스 제거
            info.IsActive = false;                              // 3) 풀 비활성 상태 플래그 설정
        }
    }
}