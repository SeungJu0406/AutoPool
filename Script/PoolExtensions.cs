using System;
using UnityEngine;

namespace NSJ_EasyPoolKit
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // ? 2025 NSJ. All rights reserved.

    /// <summary>
    /// 풀링 객체에 대한 디버그 및 반환 기능을 확장 제공하는 유틸 클래스입니다.
    /// </summary>
    public static class PoolExtensions
    {
        private static bool _isMock = false;

        /// <summary>
        /// 일정 시간(delay) 후 풀로 자동 반환합니다. 반환 전까지는 오브젝트를 사용할 수 있습니다.
        /// </summary>
        /// <param name="pooledObj">풀링된 GameObject</param>
        /// <param name="delay">지연 시간 (초)</param>
        public static GameObject ReturnAfter(this GameObject pooledObj, float delay)
        {
            ObjectPool.Return(pooledObj, delay);
            return pooledObj;
        }

        /// <summary>
        /// 일정 시간(delay) 후 풀로 자동 반환합니다. 반환 전까지는 오브젝트를 사용할 수 있습니다.
        /// </summary>
        /// <typeparam name="T">Component 타입</typeparam>
        /// <param name="pooledObj">풀링된 컴포넌트</param>
        /// <param name="delay">지연 시간 (초)</param>
        public static T ReturnAfter<T>(this T pooledObj, float delay) where T : Component
        {
            ObjectPool.Return(pooledObj, delay);
            return pooledObj;
        }

        /// <summary>
        /// GameObject가 풀에서 생성된 시점에 풀 상태 디버그 로그를 출력합니다.
        /// </summary>
        /// <param name="instance">풀링된 GameObject</param>
        /// <param name="log">추가 로그 메시지 (선택)</param>
        public static GameObject OnDebug(this GameObject instance, string log = default)
        {
            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            IPoolInfoReadOnly poolInfo = pooledObject.PoolInfo;

            if (poolInfo.IsMock == true)
            {
                if (log == default)
                {
                    Debug.Log($"[MockPool] : {poolInfo.Name}");
                }
                else
                {
                    Debug.Log($"[MockPool] : {poolInfo.Name} \n[Log] : {log}");
                }
            }
            else
            {
                if (log == default)
                {
                    Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
                }
                else
                {
                    Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
                }
            }
            return instance;
        }

        /// <summary>
        /// Component 타입도 GameObject 기반 OnDebug()를 사용할 수 있도록 래핑합니다.
        /// </summary>
        public static T OnDebug<T>(this T instance, string log = default) where T : Component
        {
            OnDebug(instance.gameObject, log);
            return instance;
        }

        /// <summary>
        /// GameObject가 풀로 반환되는 시점에 디버그 로그를 출력합니다.  
        /// 반환 이후 자동으로 이벤트 구독 해제됩니다.
        /// </summary>
        public static GameObject OnDebugReturn(this GameObject instance, string log = default)
        {
            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            IPoolInfoReadOnly poolInfo = pooledObject.PoolInfo;



            Action callback = null;

            if (poolInfo.IsMock == true)
            {
                callback = () =>
                {
                    if (log == default)
                    {
                        Debug.Log($"[MockPool] {poolInfo.Name}");
                    }
                    else
                    {
                        Debug.Log($"[MockPool] {poolInfo.Name} \n [Log] : {log}");
                    }
                    pooledObject.OnReturn -= callback;
                };
            }
            else
            {
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
                    pooledObject.OnReturn -= callback;
                };
            }

            pooledObject.OnReturn += callback;
            return instance;
        }

        /// <summary>
        /// Component 타입도 GameObject 기반 OnDebugReturn()을 사용할 수 있도록 래핑합니다.
        /// </summary>
        public static T OnDebugReturn<T>(this T instance, string log = default) where T : Component
        {
            OnDebugReturn(instance.gameObject, log);
            return instance;
        }

        /// <summary>
        /// PoolInfo를 기반으로 디버그 로그를 출력합니다.  
        /// 풀의 현재 상태 (ActiveCount / PoolCount)를 확인할 수 있습니다.
        /// </summary>
        /// <param name="poolInfo">IPoolInfoReadOnly: 읽기 전용 풀 정보</param>
        /// <param name="log">추가 로그 메시지 (선택)</param>
        public static IPoolInfoReadOnly OnDebug(this IPoolInfoReadOnly poolInfo, string log = default)
        {
            if(poolInfo == null)
                return null;
            if(poolInfo.IsMock == true)
            {
                if (log == default)
                {
                    Debug.Log($"[MockPool] {poolInfo.Name}");
                }
                else
                {
                    Debug.Log($"[MockPool] {poolInfo.Name} \n [Log] : {log}");
                }
            }
            else
            {
                if (log == default)
                {
                    Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
                }
                else
                {

                    Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
                }
            }


            return poolInfo;
        }
        /// <summary>
        /// 컴포넌트를 GameObject에 추가하거나, 이미 존재하는 컴포넌트를 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return obj.TryGetComponent(out T comp) ? comp : obj.AddComponent<T>();
        }

        /// <summary>
        /// Mock 모드를 설정합니다. Mock 모드에서는 실제 ObjectPool을 사용하지 않고, Mock ObjectPool을 사용합니다.
        /// </summary>
        /// <param name="isMock"></param>
        public static void SetMockMode(bool isMock)
        {
            _isMock = isMock;
            if (_isMock)
            {
                Debug.LogWarning("Mock mode is enabled. This will not use the actual ObjectPool.");
            }
            else
            {
                Debug.Log("Mock mode is disabled. Using the actual ObjectPool.");
            }
        }
    }
}
