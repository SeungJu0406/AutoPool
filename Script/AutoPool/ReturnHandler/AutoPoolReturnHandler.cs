using System;
using System.Collections;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolReturnHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolReturnHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        #region ReturnPool
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 풀에 다시 추가됩니다.
        /// </summary>
        public IPoolInfoReadOnly Return(GameObject instance)
        {
            return ProcessReturn(instance.gameObject);
        }
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 풀에 다시 추가됩니다.
        /// </summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            return ProcessReturn(instance.gameObject);
        }
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 지정된 지연 시간 후에 풀에 다시 추가됩니다.
        /// </summary>
        public void Return(GameObject instance, float delay)
        {
            if (instance == null)
                return;
            if (instance.activeSelf == false)
                return;

            PooledObject pooledObject = instance.GetComponent<PooledObject>();

            CoroutineRef coroutineRef = new CoroutineRef();
            coroutineRef.coroutine = _autoPool.StartCoroutine(ReturnRoutine(instance, delay, coroutineRef));
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
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 지정된 지연 시간 후에 풀에 다시 추가됩니다.
        /// </summary>
        public void Return<T>(T instance, float delay) where T : Component
        {
            Return(instance.gameObject, delay);
        }

        public IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new()
        {
            if (instance == null)
                return null;
            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if(poolGeneric.Pool.IsActive == false)
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

        public void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            if (instance == null)
                return;
            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)
                return;
            CoroutineRef coroutineRef = new CoroutineRef();
            coroutineRef.coroutine = _autoPool.StartCoroutine(GenericReturnRoutine(instance, delay, coroutineRef));
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

        /// <summary>
        /// 풀에서 오브젝트를 반환하는 코루틴입니다. 지정된 지연 시간 후에 오브젝트를 풀에 다시 추가합니다.
        /// </summary>
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
        /// 오브젝트를 풀에 반환하고 비활성화 후 다시 스택에 넣습니다.
        /// 위치, 회전, 스케일, 부모 등 초기 상태로 복원합니다.
        /// </summary>
        private IPoolInfoReadOnly ProcessReturn(GameObject instance)
        {
            //CreateObjectPool();
            if (instance == null)
                return null;

            if (instance.activeSelf == false)
                return null;

            PooledObject poolObject = instance.GetComponent<PooledObject>();
            PoolInfo info = _autoPool.FindPool(poolObject.PoolInfo.Prefab);

            // Transform 초기화
            instance.transform.position = info.Prefab.transform.position;
            instance.transform.rotation = info.Prefab.transform.rotation;
            instance.transform.localScale = info.Prefab.transform.localScale;
            instance.transform.SetParent(info.Parent);

            // RigidBody 초기화
            _autoPool.SleepRigidbody(poolObject);

            // 리턴하기 전에 호출 
            poolObject.OnReturnToPool();

            instance.gameObject.SetActive(false);
            info.Pool.Push(instance.gameObject);

            return info;
        }
    }
}