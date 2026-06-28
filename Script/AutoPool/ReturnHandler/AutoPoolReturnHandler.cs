using System;
using System.Collections;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Handles returning GameObject and generic instances to the pool,
    /// both immediately and after a timed delay.
    /// </summary>
    public class AutoPoolReturnHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolReturnHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        #region ReturnPool

        /// <summary>Returns a GameObject instance to the pool immediately.</summary>
        public IPoolInfoReadOnly Return(GameObject instance)
        {
            return ProcessReturn(instance.gameObject);
        }

        /// <summary>Returns a Component's GameObject to the pool immediately.</summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            return ProcessReturn(instance.gameObject);
        }

        /// <summary>
        /// Starts a coroutine that returns <paramref name="instance"/> to the pool after
        /// <paramref name="delay"/> seconds. If the object is returned early (e.g. by
        /// SetActive(false)) the coroutine is cancelled to avoid a double-return.
        /// </summary>
        public void Return(GameObject instance, float delay)
        {
            if (instance == null)
                return;
            if (instance.activeSelf == false)
                return;

            PooledObject pooledObject = instance.GetComponent<PooledObject>();
            if (pooledObject == null)
                return;

            CoroutineRef coroutineRef = new CoroutineRef();
            coroutineRef.coroutine = _autoPool.StartCoroutine(
                ReturnRoutine(instance, delay, coroutineRef));

            System.Action callback = null;
            callback = () =>
            {
                if (coroutineRef.coroutine != null)
                {
                    _autoPool.StopCoroutine(coroutineRef.coroutine);
                    coroutineRef.coroutine = null;
                }
                pooledObject.OnReturn -= callback;
            };

            pooledObject.OnReturn += callback;
        }

        /// <summary>Returns a Component's GameObject to the pool after <paramref name="delay"/> seconds.</summary>
        public void Return<T>(T instance, float delay) where T : Component
        {
            Return(instance.gameObject, delay);
        }

        /// <summary>Returns a generic instance to the pool immediately.</summary>
        public IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new()
        {
            if (instance == null)
                return null;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)
                return null;

            GenericPoolInfo genericPool = _autoPool.FindGenericPool<T>();
            if (genericPool == null)
            {
                Debug.LogError($"Generic Pool for {typeof(T)} not found.");
                return null;
            }

            poolGeneric.Pool.IsActive = false;
            genericPool.ActiveCount--;
            genericPool.Pool.Push(instance);
            poolGeneric.OnReturnToPool();
            poolGeneric.Pool.Return();

            return genericPool;
        }

        /// <summary>Returns a generic instance to the pool after <paramref name="delay"/> seconds.</summary>
        public void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            if (instance == null)
                return;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)
                return;

            CoroutineRef coroutineRef = new CoroutineRef();
            coroutineRef.coroutine = _autoPool.StartCoroutine(
                GenericReturnRoutine(instance, delay, coroutineRef));
        }

        private IEnumerator GenericReturnRoutine<T>(T instance, float delay, CoroutineRef coroutineRef) where T : class, IPoolGeneric, new()
        {
            yield return _autoPool.Second(delay);
            if (instance == null)
                yield break;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)
                yield break;

            coroutineRef.coroutine = null;
            GenericReturn(instance);
        }

        IEnumerator ReturnRoutine(GameObject instance, float delay, CoroutineRef coroutineRef = null)
        {
            yield return _autoPool.Second(delay);
            if (instance == null)
                yield break;

            if (instance.activeSelf == false)
                yield break;

            coroutineRef.coroutine = null;
            Return(instance);
        }

        #endregion

        /// <summary>
        /// Core return logic: resets transform to prefab defaults, re-parents under the pool
        /// root, sleeps physics, fires OnReturnToPool, deactivates the object, and pushes it
        /// back onto the stack.
        /// </summary>
        private IPoolInfoReadOnly ProcessReturn(GameObject instance)
        {
            if (instance == null)
                return null;

            if (instance.activeSelf == false)
                return null;

            PooledObject poolObject = instance.GetComponent<PooledObject>();
            if (poolObject == null)
                return null;

            PoolInfo info = _autoPool.FindPool(poolObject.PoolInfo.Prefab);

            // Sync PoolInfo reference if the pool was recreated (e.g. after ClearPool).
            if (poolObject.PoolInfo != info)
                poolObject.PoolInfo = info;

            instance.transform.position = info.Prefab.transform.position;
            instance.transform.rotation = info.Prefab.transform.rotation;
            instance.transform.localScale = info.Prefab.transform.localScale;
            instance.transform.SetParent(info.Parent);

            _autoPool.SleepRigidbody(poolObject);
            poolObject.OnReturnToPool();
            instance.gameObject.SetActive(false);
            info.Pool.Push(poolObject);

            return info;
        }
    }
}
