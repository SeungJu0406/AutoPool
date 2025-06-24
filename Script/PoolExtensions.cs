using System;
using UnityEngine;

namespace NSJ_EasyPoolKit
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // ? 2025 NSJ. All rights reserved.

    /// <summary>
    /// Ǯ�� ��ü�� ���� ����� �� ��ȯ ����� Ȯ�� �����ϴ� ��ƿ Ŭ�����Դϴ�.
    /// </summary>
    public static class PoolExtensions
    {
        private static bool _isMock = false;

        /// <summary>
        /// ���� �ð�(delay) �� Ǯ�� �ڵ� ��ȯ�մϴ�. ��ȯ �������� ������Ʈ�� ����� �� �ֽ��ϴ�.
        /// </summary>
        /// <param name="pooledObj">Ǯ���� GameObject</param>
        /// <param name="delay">���� �ð� (��)</param>
        public static GameObject ReturnAfter(this GameObject pooledObj, float delay)
        {
            ObjectPool.Return(pooledObj, delay);
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
            ObjectPool.Return(pooledObj, delay);
            return pooledObj;
        }

        /// <summary>
        /// GameObject�� Ǯ���� ������ ������ Ǯ ���� ����� �α׸� ����մϴ�.
        /// </summary>
        /// <param name="instance">Ǯ���� GameObject</param>
        /// <param name="log">�߰� �α� �޽��� (����)</param>
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
        /// Component Ÿ�Ե� GameObject ��� OnDebug()�� ����� �� �ֵ��� �����մϴ�.
        /// </summary>
        public static T OnDebug<T>(this T instance, string log = default) where T : Component
        {
            OnDebug(instance.gameObject, log);
            return instance;
        }

        /// <summary>
        /// GameObject�� Ǯ�� ��ȯ�Ǵ� ������ ����� �α׸� ����մϴ�.  
        /// ��ȯ ���� �ڵ����� �̺�Ʈ ���� �����˴ϴ�.
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
        /// Component Ÿ�Ե� GameObject ��� OnDebugReturn()�� ����� �� �ֵ��� �����մϴ�.
        /// </summary>
        public static T OnDebugReturn<T>(this T instance, string log = default) where T : Component
        {
            OnDebugReturn(instance.gameObject, log);
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
        /// ������Ʈ�� GameObject�� �߰��ϰų�, �̹� �����ϴ� ������Ʈ�� ��ȯ�մϴ�.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return obj.TryGetComponent(out T comp) ? comp : obj.AddComponent<T>();
        }

        /// <summary>
        /// Mock ��带 �����մϴ�. Mock ��忡���� ���� ObjectPool�� ������� �ʰ�, Mock ObjectPool�� ����մϴ�.
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
