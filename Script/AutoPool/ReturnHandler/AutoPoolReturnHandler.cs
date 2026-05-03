using System;
using System.Collections;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Ǯ�� ���� ��ȯ(���/����) �� ���׸� Ǯ ��ȯ�� ó���ϴ� �ڵ鷯�Դϴ�.
    /// </summary>
    public class AutoPoolReturnHandler
    {
        MainAutoPool _autoPool;

        /// <summary>
        /// ���� Ǯ �ν��Ͻ��� �޾� ��ȯ �ڵ鷯�� �ʱ�ȭ�մϴ�.
        /// </summary>
        public AutoPoolReturnHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        #region ReturnPool

        /// <summary>
        /// GameObject �ν��Ͻ��� Ǯ�� ��� ��ȯ�մϴ�.
        /// </summary>
        public IPoolInfoReadOnly Return(GameObject instance)
        {
            return ProcessReturn(instance.gameObject);
        }

        /// <summary>
        /// ������Ʈ �ν��Ͻ��� ������ GameObject�� Ǯ�� ��� ��ȯ�մϴ�.
        /// </summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            return ProcessReturn(instance.gameObject);
        }

        /// <summary>
        /// ������ ���� �ð� �� GameObject �ν��Ͻ��� Ǯ�� ��ȯ�մϴ�.
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

            CoroutineRef coroutineRef = new CoroutineRef();                                   // 1) �ڷ�ƾ ������ ���� ���� ����
            coroutineRef.coroutine = _autoPool.StartCoroutine(                               // 2) ���� ��ȯ �ڷ�ƾ ����
                ReturnRoutine(instance, delay, coroutineRef));

            System.Action callback = null;
            callback = () =>
            {
                if (coroutineRef.coroutine != null)                                          // 3) �̹� ��ȯ�Ǿ����� �ڷ�ƾ ����
                {
                    _autoPool.StopCoroutine(coroutineRef.coroutine);
                    coroutineRef.coroutine = null;
                }
                pooledObject.OnReturn -= callback;                                           // 4) �̺�Ʈ���� �ݹ� ����
            };

            pooledObject.OnReturn += callback;                                               // 5) ������Ʈ�� ���� ��Ȱ��ȭ�� �� �ڷ�ƾ ����
        }

        /// <summary>
        /// ������ ���� �ð� �� ������Ʈ �ν��Ͻ��� ������ GameObject�� Ǯ�� ��ȯ�մϴ�.
        /// </summary>
        public void Return<T>(T instance, float delay) where T : Component
        {
            Return(instance.gameObject, delay);
        }

        /// <summary>
        /// ���׸� Ǯ �ν��Ͻ��� ��� ��ȯ�մϴ�.
        /// </summary>
        public IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new()
        {
            if (instance == null)
                return null;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)                                         // �̹� ��Ȱ�� ���¸� �ߺ� ��ȯ ����
                return null;

            GenericPoolInfo genericPool = _autoPool.FindGenericPool<T>();                   // 1) Ÿ�� T ��� ���׸� Ǯ ã��
            if (genericPool == null)
            {
                Debug.LogError($"Generic Pool for {typeof(T)} not found.");
                return null;
            }

            poolGeneric.Pool.IsActive = false;                                              // 2) ���� ��ü Ȱ�� �÷��� ����
            genericPool.ActiveCount--;                                                      // 3) Ȱ�� ī��Ʈ ����
            genericPool.Pool.Push(instance);                                                // 4) Ǯ ���ÿ� Ǫ��
            poolGeneric.OnReturnToPool();                                                   // 5) ���� ��ȯ �ݹ� ����
            poolGeneric.Pool.Return();                                                      // 6) PoolGenericInfo.OnReturn �̺�Ʈ ȣ��

            return genericPool;
        }

        /// <summary>
        /// ������ ���� �ð� �� ���׸� Ǯ �ν��Ͻ��� ��ȯ�մϴ�.
        /// </summary>
        public void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            if (instance == null)
                return;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)                                         // �̹� ��Ȱ�� ���¸� �ߺ� ��ȯ ����
                return;

            CoroutineRef coroutineRef = new CoroutineRef();                                 // 1) ���׸� ��ȯ�� �ڷ�ƾ ����
            coroutineRef.coroutine = _autoPool.StartCoroutine(
                GenericReturnRoutine(instance, delay, coroutineRef));                       // 2) ���� ��ȯ �ڷ�ƾ ����
        }

        /// <summary>
        /// ������ �ð� �� ���׸� �ν��Ͻ��� Ǯ�� ��ȯ�ϴ� �ڷ�ƾ�Դϴ�.
        /// </summary>
        private IEnumerator GenericReturnRoutine<T>(T instance, float delay, CoroutineRef coroutineRef) where T : class, IPoolGeneric, new()
        {
            yield return _autoPool.Second(delay);                                           // 1) ĳ�̵� WaitForSeconds ���
            if (instance == null)
                yield break;

            IPoolGeneric poolGeneric = (IPoolGeneric)instance;
            if (poolGeneric.Pool.IsActive == false)                                        // 2) �̹� ��ȯ�Ǿ����� ����
                yield break;

            coroutineRef.coroutine = null;                                                 // 3) �ڷ�ƾ �Ϸ� ǥ��
            GenericReturn(instance);                                                       // 4) ���� ��ȯ ó�� ����
        }

        /// <summary>
        /// ������ �ð� �� GameObject�� Ǯ�� ��ȯ�ϴ� �ڷ�ƾ�Դϴ�.
        /// </summary>
        IEnumerator ReturnRoutine(GameObject instance, float delay, CoroutineRef coroutineRef = null)
        {
            yield return _autoPool.Second(delay);                                          // 1) ĳ�̵� WaitForSeconds ���
            if (instance == null)
                yield break;

            if (instance.activeSelf == false)                                              // 2) �̹� ��Ȱ��ȭ�Ǿ����� ����
                yield break;

            coroutineRef.coroutine = null;                                                 // 3) �ڷ�ƾ �Ϸ� ǥ��
            Return(instance);                                                              // 4) ���� ��ȯ ó�� ����
        }

        #endregion

        /// <summary>
        /// GameObject�� ���� Ǯ�� ��ȯ�ϴ� ���� ó�� �����Դϴ�.
        /// </summary>
        private IPoolInfoReadOnly ProcessReturn(GameObject instance)
        {
            if (instance == null)
                return null;

            if (instance.activeSelf == false)                                              // �̹� ��Ȱ��ȭ�� ��� �ߺ� ��ȯ ����
                return null;

            PooledObject poolObject = instance.GetComponent<PooledObject>();               // 1) Ǯ ������ ���� PooledObject ����
            if (poolObject == null)
                return null;

            PoolInfo info = _autoPool.FindPool(poolObject.PoolInfo.Prefab);               // 2) ���� ������ ���� Ǯ ���� ��ȸ
            if (poolObject.PoolInfo != info)                                              // 3) ������ �ٸ��� �ֽ� PoolInfo�� ����ȭ
            {
                poolObject.PoolInfo = info;
            }

            instance.transform.position = info.Prefab.transform.position;                  // 4) �������� ��ġ/ȸ��/�����Ϸ� ����
            instance.transform.rotation = info.Prefab.transform.rotation;
            instance.transform.localScale = info.Prefab.transform.localScale;
            instance.transform.SetParent(info.Parent);                                     // 5) Ǯ ���� �θ� Ʈ���������� ����

            _autoPool.SleepRigidbody(poolObject);                                          // 6) ���� ���� Sleep ó��

            poolObject.OnReturnToPool();                                                   // 7) IPooledObject ��ȯ �ݹ� ����

            instance.gameObject.SetActive(false);                                          // 8) ������Ʈ ��Ȱ��ȭ
            info.Pool.Push(instance.gameObject);                                           // 9) Ǯ ���ÿ� Ǫ��

            return info;
        }
    }
}