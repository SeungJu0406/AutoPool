using System;
using System.Collections;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 풀에 대한 반환(즉시/지연) 및 제네릭 풀 반환을 처리하는 핸들러입니다.
    /// </summary>
    public class AutoPoolReturnHandler
    {
        MainAutoPool _autoPool;

        /// <summary>
        /// 메인 풀 인스턴스를 받아 반환 핸들러를 초기화합니다.
        /// </summary>
        public AutoPoolReturnHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        #region ReturnPool

        /// <summary>
        /// GameObject 인스턴스를 풀에 즉시 반환합니다.
        /// </summary>
        public IPoolInfoReadOnly Return(GameObject instance)
        {
            return ProcessReturn(instance.gameObject);
        }

        /// <summary>
        /// 컴포넌트 인스턴스를 포함한 GameObject를 풀에 즉시 반환합니다.
        /// </summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            return ProcessReturn(instance.gameObject);
        }

        /// <summary>
        /// 지정된 지연 시간 후 GameObject 인스턴스를 풀에 반환합니다.
        /// </summary>
        public void Return(GameObject instance, float delay)
        {
            if (instance == null)
                return;
            if (instance.activeSelf == false)
                return;

            PooledObject pooledObject = instance.GetComponent<PooledObject>();

            CoroutineRef coroutineRef = new CoroutineRef();                                   // 1) 코루틴 참조를 담을 래퍼 생성
            coroutineRef.coroutine = _autoPool.StartCoroutine(                               // 2) 지연 반환 코루틴 시작
                ReturnRoutine(instance, delay, coroutineRef));

            System.Action callback = null;
            callback = () =>
            {
                if (coroutineRef.coroutine != null)                                          // 3) 이미 반환되었으면 코루틴 중지
                {
                    _autoPool.StopCoroutine(coroutineRef.coroutine);
                    coroutineRef.coroutine = null;
                }
                pooledObject.OnReturn -= callback;                                           // 4) 이벤트에서 콜백 해제
            };

            pooledObject.OnReturn += callback;                                               // 5) 오브젝트가 먼저 비활성화될 때 코루틴 정리
        }

        /// <summary>
        /// 지정된 지연 시간 후 컴포넌트 인스턴스를 포함한 GameObject를 풀에 반환합니다.
        /// </summary>
        public void Return<T>(T instance, float delay) where T : Component
        {
            Return(instance.gameObject, delay);
        }

        /// <summary>
        /// 제네릭 풀 인스턴스를 즉시 반환합니다.
        /// </summary>
        public IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new()
        {
            if (instance == null)
                return null;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)                                         // 이미 비활성 상태면 중복 반환 방지
                return null;

            GenericPoolInfo genericPool = _autoPool.FindGenericPool<T>();                   // 1) 타입 T 기반 제네릭 풀 찾기
            if (genericPool == null)
            {
                Debug.LogError($"Generic Pool for {typeof(T)} not found.");
                return null;
            }

            poolGeneric.Pool.IsActive = false;                                              // 2) 개별 객체 활성 플래그 해제
            genericPool.ActiveCount--;                                                      // 3) 활성 카운트 감소
            genericPool.Pool.Push(instance);                                                // 4) 풀 스택에 푸시
            poolGeneric.OnReturnToPool();                                                   // 5) 개별 반환 콜백 실행
            poolGeneric.Pool.Return();                                                      // 6) PoolGenericInfo.OnReturn 이벤트 호출

            return genericPool;
        }

        /// <summary>
        /// 지정된 지연 시간 후 제네릭 풀 인스턴스를 반환합니다.
        /// </summary>
        public void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            if (instance == null)
                return;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)                                         // 이미 비활성 상태면 중복 반환 방지
                return;

            CoroutineRef coroutineRef = new CoroutineRef();                                 // 1) 제네릭 반환용 코루틴 래퍼
            coroutineRef.coroutine = _autoPool.StartCoroutine(
                GenericReturnRoutine(instance, delay, coroutineRef));                       // 2) 지연 반환 코루틴 시작
        }

        /// <summary>
        /// 지정된 시간 후 제네릭 인스턴스를 풀에 반환하는 코루틴입니다.
        /// </summary>
        private IEnumerator GenericReturnRoutine<T>(T instance, float delay, CoroutineRef coroutineRef) where T : class, IPoolGeneric, new()
        {
            yield return _autoPool.Second(delay);                                           // 1) 캐싱된 WaitForSeconds 사용
            if (instance == null)
                yield break;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)                                        // 2) 이미 반환되었으면 종료
                yield break;

            coroutineRef.coroutine = null;                                                 // 3) 코루틴 완료 표시
            GenericReturn(instance);                                                       // 4) 실제 반환 처리 재사용
        }

        /// <summary>
        /// 지정된 시간 후 GameObject를 풀에 반환하는 코루틴입니다.
        /// </summary>
        IEnumerator ReturnRoutine(GameObject instance, float delay, CoroutineRef coroutineRef = null)
        {
            yield return _autoPool.Second(delay);                                          // 1) 캐싱된 WaitForSeconds 사용
            if (instance == null)
                yield break;

            if (instance.activeSelf == false)                                              // 2) 이미 비활성화되었으면 종료
                yield break;

            coroutineRef.coroutine = null;                                                 // 3) 코루틴 완료 표시
            Return(instance);                                                              // 4) 실제 반환 처리 재사용
        }

        #endregion

        /// <summary>
        /// GameObject를 실제 풀에 반환하는 공통 처리 로직입니다.
        /// </summary>
        private IPoolInfoReadOnly ProcessReturn(GameObject instance)
        {
            if (instance == null)
                return null;

            if (instance.activeSelf == false)                                              // 이미 비활성화된 경우 중복 반환 방지
                return null;

            PooledObject poolObject = instance.GetComponent<PooledObject>();               // 1) 풀 관리를 위한 PooledObject 참조
            PoolInfo info = _autoPool.FindPool(poolObject.PoolInfo.Prefab);               // 2) 원본 프리팹 기준 풀 정보 조회
            if (poolObject.PoolInfo != info)                                              // 3) 참조가 다르면 최신 PoolInfo로 동기화
            {
                poolObject.PoolInfo = info;
            }

            instance.transform.position = info.Prefab.transform.position;                  // 4) 프리팹의 위치/회전/스케일로 리셋
            instance.transform.rotation = info.Prefab.transform.rotation;
            instance.transform.localScale = info.Prefab.transform.localScale;
            instance.transform.SetParent(info.Parent);                                     // 5) 풀 전용 부모 트랜스폼으로 복귀

            _autoPool.SleepRigidbody(poolObject);                                          // 6) 물리 상태 Sleep 처리

            poolObject.OnReturnToPool();                                                   // 7) IPooledObject 반환 콜백 실행

            instance.gameObject.SetActive(false);                                          // 8) 오브젝트 비활성화
            info.Pool.Push(instance.gameObject);                                           // 9) 풀 스택에 푸시

            return info;
        }
    }
}