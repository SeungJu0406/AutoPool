using System;
using System.Collections;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Extension methods for pooled objects: delayed return, condition-based return, and editor debug helpers.
    /// </summary>
    public static class PoolExtensions
    {
        /// <summary>
        /// Returns this GameObject to the pool after the specified delay in seconds.
        /// </summary>
        public static GameObject ReturnAfter(this GameObject pooledObj, float delay)
        {
            ObjectPool.Return(pooledObj, delay);
            return pooledObj;
        }

        /// <summary>
        /// Returns this component's GameObject to the pool after the specified delay in seconds.
        /// </summary>
        public static T ReturnAfter<T>(this T pooledObj, float delay) where T : Component
        {
            ObjectPool.Return(pooledObj, delay);
            return pooledObj;
        }

        /// <summary>
        /// Returns this generic pool object after the specified delay in seconds.
        /// </summary>
        public static T ReturnAfterGeneric<T>(this T poolGeneric, float delay) where T : class, IPoolGeneric, new()
        {
            ObjectPool.ReturnGeneric(poolGeneric, delay);
            return poolGeneric;
        }

        /// <summary>
        /// Returns this GameObject to the pool once the given condition becomes true, or if the object is deactivated.
        /// </summary>
        public static GameObject ReturnWhen(this GameObject pooledObj, Func<bool> condition)
        {
            PooledObject pooledObject = pooledObj.GetComponent<PooledObject>();
            ObjectPool.Instance.StartCoroutine(ReturnWhenCoroutine(pooledObject, condition));
            return pooledObj;
        }

        /// <summary>
        /// Returns this component's GameObject to the pool once the given condition becomes true, or if the object is deactivated.
        /// </summary>
        public static T ReturnWhen<T>(this T pooledObj, Func<bool> condition) where T : Component
        {
            PooledObject pooledObject = pooledObj.GetComponent<PooledObject>();
            ObjectPool.Instance.StartCoroutine(ReturnWhenCoroutine(pooledObject, condition));
            return pooledObj;
        }

        /// <summary>
        /// Returns this generic pool object once the given condition becomes true.
        /// </summary>
        public static T ReturnWhenGeneric<T>(this T pooledObj, Func<bool> condition) where T : class, IPoolGeneric, new()
        {
            ObjectPool.Instance.StartCoroutine(ReturnWhenCoroutine(pooledObj, condition));
            return pooledObj;
        }

        /// <summary>
        /// (Editor only) Logs the pool state of this GameObject.
        /// </summary>
        public static GameObject OnDebug(this GameObject instance, string log = default)
        {
#if UNITY_EDITOR
            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            IPoolInfoReadOnly poolInfo = pooledObject.PoolInfo;
            if (log == default)
            {
                UnityEngine.Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
            }
            else
            {
                UnityEngine.Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
            }
#endif
            return instance;
        }

        /// <summary>
        /// (Editor only) Logs the pool state of this component or generic pool object.
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
            if (instance is IPoolGeneric poolGeneric)
            {
                IGenericPoolInfoReadOnly poolInfo = poolGeneric.Pool.PoolInfo;
                if (log == default)
                {
                    UnityEngine.Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
                }
                else
                {
                    UnityEngine.Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
                }
                return instance;
            }

            UnityEngine.Debug.Log($"[Unknown] {instance.GetType()} \n [Log] : {log}");
#endif
            return instance;
        }

        /// <summary>
        /// (Editor only) Logs the pool state of this GameObject at the moment it is returned to the pool.
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
                    UnityEngine.Debug.Log($"[Pool] {poolInfo.Name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
                }
                else
                {
                    UnityEngine.Debug.Log($"[Pool] {poolInfo.Name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
                }
                pooledObject.OnReturn -= callback;
            };

            pooledObject.OnReturn += callback;
#endif
            return instance;
        }

        /// <summary>
        /// (Editor only) Logs the pool state of this component or generic pool object at the moment it is returned to the pool.
        /// </summary>
        public static T OnDebugReturn<T>(this T instance, string log = default)
        {
#if UNITY_EDITOR
            if (instance == null)
                return instance;

            if (instance is Component component)
            {
                OnDebugReturn(component.gameObject, log);
                return instance;
            }
            if (instance is IPoolGeneric poolGeneric)
            {
                IGenericPoolInfoReadOnly poolInfo = poolGeneric.Pool.PoolInfo;
                Action callback = null;

                callback = () =>
                {
                    if (log == default)
                    {
                        UnityEngine.Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
                    }
                    else
                    {
                        UnityEngine.Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
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
        /// (Editor only) Logs the current state of this pool info object.
        /// </summary>
        public static IPoolInfoReadOnly OnDebug(this IPoolInfoReadOnly poolInfo, string log = default)
        {
#if UNITY_EDITOR
            if (poolInfo == null)
                return null;

            if (log == default)
            {
                UnityEngine.Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
            }
            else
            {
                UnityEngine.Debug.Log($"[Pool] {poolInfo.Prefab.name} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
            }
#endif
            return poolInfo;
        }

        /// <summary>
        /// (Editor only) Logs the current state of this generic pool info object.
        /// </summary>
        public static IGenericPoolInfoReadOnly OnDebug(this IGenericPoolInfoReadOnly poolInfo, string log = default)
        {
#if UNITY_EDITOR
            if (poolInfo == null)
                return null;

            if (log == default)
            {
                UnityEngine.Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount})");
            }
            else
            {
                UnityEngine.Debug.Log($"[Pool] {poolInfo.Type} (Active : {poolInfo.ActiveCount} / {poolInfo.PoolCount}) \n [Log] : {log}");
            }
#endif
            return poolInfo;
        }

        /// <summary>
        /// Gets the component of type T on the GameObject, or adds one if it doesn't exist.
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return obj.TryGetComponent(out T comp) ? comp : obj.AddComponent<T>();
        }

        #region ReturnWhenCoroutine

        static IEnumerator ReturnWhenCoroutine(PooledObject pooledObj, Func<bool> condition)
        {
            while (!condition() && pooledObj.gameObject.activeSelf == true)
            {
                yield return null;
            }
            ObjectPool.Return(pooledObj.gameObject);
        }

        static IEnumerator ReturnWhenCoroutine<T>(T pooledObj, Func<bool> condition) where T : class, IPoolGeneric, new()
        {
            while (!condition())
            {
                yield return null;
            }
            ObjectPool.ReturnGeneric(pooledObj);
        }

        #endregion
    }
}
