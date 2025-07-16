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
        /// Ǯ���� ������Ʈ�� ��ȯ�մϴ�. ��ȯ�� ������Ʈ�� ��Ȱ��ȭ�ǰ�, Ǯ�� �ٽ� �߰��˴ϴ�.
        /// </summary>
        public IPoolInfoReadOnly Return(GameObject instance)
        {
            return ProcessReturn(instance.gameObject);
        }
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��ȯ�մϴ�. ��ȯ�� ������Ʈ�� ��Ȱ��ȭ�ǰ�, Ǯ�� �ٽ� �߰��˴ϴ�.
        /// </summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            return ProcessReturn(instance.gameObject);
        }
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��ȯ�մϴ�. ��ȯ�� ������Ʈ�� ��Ȱ��ȭ�ǰ�, ������ ���� �ð� �Ŀ� Ǯ�� �ٽ� �߰��˴ϴ�.
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
        /// Ǯ���� ������Ʈ�� ��ȯ�մϴ�. ��ȯ�� ������Ʈ�� ��Ȱ��ȭ�ǰ�, ������ ���� �ð� �Ŀ� Ǯ�� �ٽ� �߰��˴ϴ�.
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
        /// Ǯ���� ������Ʈ�� ��ȯ�ϴ� �ڷ�ƾ�Դϴ�. ������ ���� �ð� �Ŀ� ������Ʈ�� Ǯ�� �ٽ� �߰��մϴ�.
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
        /// ������Ʈ�� Ǯ�� ��ȯ�ϰ� ��Ȱ��ȭ �� �ٽ� ���ÿ� �ֽ��ϴ�.
        /// ��ġ, ȸ��, ������, �θ� �� �ʱ� ���·� �����մϴ�.
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

            // Transform �ʱ�ȭ
            instance.transform.position = info.Prefab.transform.position;
            instance.transform.rotation = info.Prefab.transform.rotation;
            instance.transform.localScale = info.Prefab.transform.localScale;
            instance.transform.SetParent(info.Parent);

            // RigidBody �ʱ�ȭ
            _autoPool.SleepRigidbody(poolObject);

            // �����ϱ� ���� ȣ�� 
            poolObject.OnReturnToPool();

            instance.gameObject.SetActive(false);
            info.Pool.Push(instance.gameObject);

            return info;
        }
    }
}