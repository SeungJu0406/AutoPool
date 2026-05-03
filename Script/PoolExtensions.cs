using System;
using System.Collections;
using UnityEngine;

namespace AutoPool_Tool
{

    /// <summary>
    /// 풀링된 객체에 대한 반환/디버그/보조 기능을 확장 메서드 형태로 제공하는 정적 클래스입니다.
    /// </summary>
    public static class PoolExtensions
    {
        /// <summary>
        /// 지정한 지연 시간 후 이 GameObject를 풀에 반환합니다.
        /// </summary>
        public static GameObject ReturnAfter(this GameObject pooledObj, float delay)
        {
            ObjectPool.Return(pooledObj, delay);
            return pooledObj;
        }

        /// <summary>
        /// 지정한 지연 시간 후 이 컴포넌트를 포함한 GameObject를 풀에 반환합니다.
        /// </summary>
        public static T ReturnAfter<T>(this T pooledObj, float delay) where T : Component
        {
            ObjectPool.Return(pooledObj, delay);
            return pooledObj;
        }

        /// <summary>
        /// 지정한 지연 시간 후 제네릭 풀 객체를 반환합니다.
        /// </summary>
        public static T ReturnAfterGeneric<T>(this T poolGeneric, float delay) where T : class, IPoolGeneric, new()
        {
            ObjectPool.ReturnGeneric(poolGeneric, delay);
            return poolGeneric;
        }

        /// <summary>
        /// 조건이 만족될 때까지 대기한 뒤 이 GameObject를 풀에 반환합니다.
        /// </summary>
        public static GameObject ReturnWhen(this GameObject pooledObj, Func<bool> condition)
        {
            PooledObject pooledObject = pooledObj.GetComponent<PooledObject>();                    // 1) PooledObject 컴포넌트 획득
            ObjectPool.Instance.StartCoroutine(ReturnWhenCoroutine(pooledObject, condition));       // 2) 조건을 기다리는 코루틴 시작
            return pooledObj;                                                                      // 3) 메서드 체이닝을 위해 자기 자신 반환
        }

        /// <summary>
        /// 조건이 만족될 때까지 대기한 뒤 이 컴포넌트를 포함한 GameObject를 풀에 반환합니다.
        /// </summary>
        public static T ReturnWhen<T>(this T pooledObj, Func<bool> condition) where T : Component
        {
            PooledObject pooledObject = pooledObj.GetComponent<PooledObject>();                    // 1) GameObject에서 PooledObject 찾기
            ObjectPool.Instance.StartCoroutine(ReturnWhenCoroutine(pooledObject, condition));       // 2) 조건 기반 반환 코루틴 실행
            return pooledObj;                                                                      // 3) 메서드 체이닝 지원
        }

        /// <summary>
        /// 조건이 만족될 때까지 대기한 뒤 제네릭 풀 객체를 반환합니다.
        /// </summary>
        public static T ReturnWhenGeneric<T>(this T pooledObj, Func<bool> condition) where T : class, IPoolGeneric, new()
        {
            ObjectPool.Instance.StartCoroutine(ReturnWhenCoroutine(pooledObj, condition));          // 1) 제네릭 객체를 대상으로 하는 코루틴 실행
            return pooledObj;                                                                      // 2) 메서드 체이닝 지원
        }

        /// <summary>
        /// 에디터에서 이 GameObject의 풀 상태를 로그로 출력합니다.
        /// </summary>
        public static GameObject OnDebug(this GameObject instance, string log = default)
        {
#if UNITY_EDITOR
            PooledObject pooledObject = instance.GetComponent<PooledObject>();                     // 1) PooledObject 참조 획득
            IPoolInfoReadOnly poolInfo = pooledObject.PoolInfo;                                   // 2) 풀 정보 참조
            if (log == default)                                                                    // 3) 추가 메시지 유무에 따라
            {
                Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
            }
            else
            {
                Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
            }
#endif
            return instance;
        }

        /// <summary>
        /// 에디터에서 컴포넌트 또는 제네릭 풀 객체의 풀 상태를 로그로 출력합니다.
        /// </summary>
        public static T OnDebug<T>(this T instance, string log = default)
        {
#if UNITY_EDITOR
            if (instance == null)                                                                  // 1) null이면 아무 것도 하지 않음
                return instance;

            if (instance is Component component)                                                   // 2) Unity 컴포넌트인 경우
            {
                OnDebug(component.gameObject, log);                                                //    GameObject 기준 디버그 호출
                return instance;
            }
            if (instance is IPoolGeneric poolGeneric)                                              // 3) 제네릭 풀 객체인 경우
            {
                IGenericPoolInfoReadOnly poolInfo = poolGeneric.Pool.PoolInfo;                    //    풀 정보 참조
                if (log == default)
                {
                    Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
                }
                else
                {
                    Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
                }
                return instance;
            }

            Debug.Log($"[Unknown] {instance.GetType()} \n [Log] : {log}");                         // 4) 둘 다 아니면 타입만 출력
#endif
            return instance;
        }

        /// <summary>
        /// 이 GameObject가 풀로 반환되는 시점에 풀 상태를 로그로 출력합니다.
        /// </summary>
        public static GameObject OnDebugReturn(this GameObject instance, string log = default)
        {
#if UNITY_EDITOR
            PooledObject pooledObject = instance.GetComponent<PooledObject>();                     // 1) PooledObject 참조
            IPoolInfoReadOnly poolInfo = pooledObject.PoolInfo;                                   // 2) 풀 정보 참조

            Action callback = null;                                                                // 3) 로컬 콜백 변수 선언
            callback = () =>
            {
                if (log == default)
                {
                    Debug.Log($"[Pool] {poolInfo.Name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
                }
                else
                {
                    Debug.Log($"[Pool] {poolInfo.Name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
                }
                pooledObject.OnReturn -= callback;                                                 // 4) 한 번 호출 후 이벤트에서 제거
            };

            pooledObject.OnReturn += callback;                                                     // 5) 반환 이벤트에 콜백 등록
#endif
            return instance;
        }

        /// <summary>
        /// 컴포넌트 또는 제네릭 풀 객체가 반환되는 시점에 풀 상태를 로그로 출력합니다.
        /// </summary>
        public static T OnDebugReturn<T>(this T instance, string log = default)
        {
#if UNITY_EDITOR
            if (instance == null)                                                                  // 1) null 보호
                return instance;

            if (instance is Component component)                                                   // 2) 컴포넌트이면 GameObject 버전 사용
            {
                OnDebugReturn(component.gameObject, log);
                return instance;
            }
            if (instance is IPoolGeneric poolGeneric)                                              // 3) 제네릭 풀 객체면 전용 처리
            {
                IGenericPoolInfoReadOnly poolInfo = poolGeneric.Pool.PoolInfo;                    //    풀 정보 참조
                Action callback = null;

                callback = () =>
                {
                    if (log == default)
                    {
                        Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
                    }
                    else
                    {
                        Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
                    }
                    poolGeneric.Pool.OnReturn -= callback;                                         //    한 번 호출 후 이벤트 해제
                };

                poolGeneric.Pool.OnReturn += callback;                                             //    반환 이벤트에 등록
                return instance;
            }
#endif
            return instance;
        }

        /// <summary>
        /// 풀 정보 객체의 현재 상태를 에디터 로그로 출력합니다.
        /// </summary>
        public static IPoolInfoReadOnly OnDebug(this IPoolInfoReadOnly poolInfo, string log = default)
        {
#if UNITY_EDITOR
            if (poolInfo == null)                                                                  // 1) null이면 바로 반환
                return null;

            if (log == default)
            {
                Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
            }
            else
            {
                Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
            }
#endif
            return poolInfo;
        }

        /// <summary>
        /// 제네릭 풀 정보 객체의 현재 상태를 에디터 로그로 출력합니다.
        /// </summary>
        public static IGenericPoolInfoReadOnly OnDebug(this IGenericPoolInfoReadOnly poolInfo, string log = default)
        {
#if UNITY_EDITOR
            if (poolInfo == null)                                                                  // 1) null이면 바로 반환
                return null;

            if (log == default)
            {
                Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
            }
            else
            {
                Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
            }
#endif
            return poolInfo;
        }

        /// <summary>
        /// GameObject에서 지정 타입의 컴포넌트를 가져오거나, 없으면 새로 추가합니다.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return obj.TryGetComponent(out T comp) ? comp : obj.AddComponent<T>();
        }

        #region ReturnWhenCoroutine

        /// <summary>
        /// 조건이 참이 되거나 오브젝트가 비활성화될 때까지 대기 후 GameObject를 풀에 반환하는 코루틴입니다.
        /// </summary>
        static IEnumerator ReturnWhenCoroutine(PooledObject pooledObj, Func<bool> condition)
        {
            while (!condition() && pooledObj.gameObject.activeSelf == true)                        // 1) 조건이 거짓이고 오브젝트가 활성인 동안 계속 대기
            {
                yield return null;                                                                 // 2) 다음 프레임까지 기다리기
            }
            ObjectPool.Return(pooledObj.gameObject);                                               // 3) 조건 충족 또는 비활성 시 풀에 반환
        }

        /// <summary>
        /// 조건이 참이 될 때까지 대기한 뒤 제네릭 풀 객체를 반환하는 코루틴입니다.
        /// </summary>
        static IEnumerator ReturnWhenCoroutine<T>(T pooledObj, Func<bool> condition) where T : class, IPoolGeneric, new()
        {
            while (!condition())                                                                   // 1) 조건이 거짓인 동안 계속 대기
            {
                yield return null;                                                                 // 2) 다음 프레임까지 기다리기
            }
            ObjectPool.ReturnGeneric(pooledObj);                                                   // 3) 조건 충족 시 제네릭 풀에 반환
        }

        #endregion
    }
}
