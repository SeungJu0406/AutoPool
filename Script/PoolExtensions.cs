using System;
using UnityEngine;

namespace AutoPool
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // ? 2025 NSJ. All rights reserved.

    /// <summary>
    /// Ǯ�� ��ü�� ���� ����� �� ��ȯ ����� Ȯ�� �����ϴ� ��ƿ Ŭ�����Դϴ�.
    /// </summary>
    public static class PoolExtensions
    {
        /// <summary>
        /// ���� �ð�(delay) �� Ǯ�� �ڵ� ��ȯ�մϴ�. ��ȯ �������� ������Ʈ�� ����� �� �ֽ��ϴ�.
        /// </summary>
        /// <param name="pooledObj">Ǯ���� GameObject</param>
        /// <param name="delay">���� �ð� (��)</param>
        public static GameObject ReturnAfter(this GameObject pooledObj, float delay)
        {
            AutoPool.Return(pooledObj, delay);
            return pooledObj;
        }

        /// <summary>
        /// ���� �ð�(delay) �� Ǯ�� �ڵ� ��ȯ�մϴ�. ��ȯ �������� ������Ʈ�� ����� �� �ֽ��ϴ�.
        /// </summary>
        /// <typeparam name="T">Component Ÿ��</typeparam>
        /// <param name="pooledObj">Ǯ���� ������Ʈ</param>
        /// <param name="delay">���� �ð� (��)</param>
        public static T ReturnAfter<T>(this T pooledObj, float delay) where T : Component
        {
            AutoPool.Return(pooledObj, delay);
            return pooledObj;
        }
        public static T ReturnAfterGeneric<T>(this T poolGeneric, float delay) where T : class, IPoolGeneric, new()
        {
            AutoPool.ReturnGeneric(poolGeneric, delay);
            return poolGeneric;
        }

        /// <summary>
        /// GameObject�� Ǯ���� ������ ������ Ǯ ���� ����� �α׸� ����մϴ�.
        /// </summary>
        /// <param name="instance">Ǯ���� GameObject</param>
        /// <param name="log">�߰� �α� �޽��� (����)</param>
        public static GameObject OnDebug(this GameObject instance, string log = default)
        {
#if UNITY_EDITOR
            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            IPoolInfoReadOnly poolInfo = pooledObject.PoolInfo;
            if (log == default)
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
        /// Component Ÿ�Ե� GameObject ��� OnDebug()�� ����� �� �ֵ��� �����մϴ�.
        /// </summary>
        public static T OnDebug<T>(this T instance, string log = default)
        {
#if UNITY_EDITOR
            if (instance == null)
                return instance;

            if (instance is Component component)
            {
                OnDebug(component.gameObject, log);
                return instance;
            }
            if(instance is IPoolGeneric poolGeneric)
            {
                IGenericPoolInfoReadOnly poolInfo = poolGeneric.Pool.PoolInfo;
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

            Debug.Log($"[Unknown] {instance.GetType()} \n [Log] : {log}");
#endif
            return instance;
        }
        /// <summary>
        /// GameObject�� Ǯ�� ��ȯ�Ǵ� ������ ����� �α׸� ����մϴ�.  
        /// ��ȯ ���� �ڵ����� �̺�Ʈ ���� �����˴ϴ�.
        /// </summary>
        public static GameObject OnDebugReturn(this GameObject instance, string log = default)
        {
#if UNITY_EDITOR
            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            IPoolInfoReadOnly poolInfo = pooledObject.PoolInfo;

            Action callback = null;
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

            pooledObject.OnReturn += callback;
#endif
            return instance;
        }

        /// <summary>
        /// Component Ÿ�Ե� GameObject ��� OnDebugReturn()�� ����� �� �ֵ��� �����մϴ�.
        /// </summary>
        public static T OnDebugReturn<T>(this T instance, string log = default)
        {
#if UNITY_EDITOR
            if (instance == null)
                return instance;
            if(instance is Component component)
            {
                OnDebugReturn(component.gameObject, log);
                return instance;
            }
            if(instance is IPoolGeneric poolGeneric)
            {
                IGenericPoolInfoReadOnly poolInfo = poolGeneric.Pool.PoolInfo;
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
                    poolGeneric.Pool.OnReturn -= callback;
                };

                poolGeneric.Pool.OnReturn += callback;
                return instance;
            }
#endif
            return instance;
        }

        /// <summary>
        /// PoolInfo�� ������� ����� �α׸� ����մϴ�.  
        /// Ǯ�� ���� ���� (ActiveCount / PoolCount)�� Ȯ���� �� �ֽ��ϴ�.
        /// </summary>
        /// <param name="poolInfo">IPoolInfoReadOnly: �б� ���� Ǯ ����</param>
        /// <param name="log">�߰� �α� �޽��� (����)</param>
        public static IPoolInfoReadOnly OnDebug(this IPoolInfoReadOnly poolInfo, string log = default)
        {
#if UNITY_EDITOR
            if (poolInfo == null)
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

        public static IGenericPoolInfoReadOnly OnDebug(this IGenericPoolInfoReadOnly poolInfo, string log = default)
        {
#if UNITY_EDITOR
            if (poolInfo == null)
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
        /// ������Ʈ�� GameObject�� �߰��ϰų�, �̹� �����ϴ� ������Ʈ�� ��ȯ�մϴ�.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return obj.TryGetComponent(out T comp) ? comp : obj.AddComponent<T>();
        }
    }
}
