using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Ǯ �� ���׸� Ǯ�� ���ε�(Preload)�� ����(Clear)�� ����ϴ� �ڵ鷯�Դϴ�.
    /// </summary>
    public class AutoPoolPreloadHandler
    {
        /// <summary>
        /// ���� Ǯ ��ųʸ��� ��ƿ��Ƽ�� ������ ���� Ǯ �ν��Ͻ��Դϴ�.
        /// </summary>
        MainAutoPool _autoPool;

        /// <summary>
        /// ������ ���� Ǯ �ν��Ͻ��� �����ε� �ڵ鷯�� �ʱ�ȭ�մϴ�.
        /// </summary>
        public AutoPoolPreloadHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// ������ ���� Ǯ�� ã��, ���� ������ŭ ���ε��� �����մϴ�.
        /// </summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// ������Ʈ ������ ���� Ǯ�� ã��, ���� ������ŭ ���ε��� �����մϴ�.
        /// </summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// Resources ��� ���� Ǯ�� ã��, ���� ������ŭ ���ε��� �����մϴ�.
        /// </summary>
        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// ���� �����ε� ó�� ��������, ������ ������ ������ ������ �ν��Ͻ��� �����Ͽ� Ǯ�� ä��ϴ�.
        /// </summary>
        private IPoolInfoReadOnly ProcessPreload(PoolInfo info, int count)
        {
            if (info == null)
            {
                Debug.LogError("The pool information is invalid.");
                return null;
            }

            // ��ǥ ����(count)�� ������ ������ ������ �ν��Ͻ��� ����
            while (info.PoolCount < count)
            {
                GameObject instance = GameObject.Instantiate(info.Prefab);       // 1) ���������� �� �ν��Ͻ� ����
                PooledObject poolObject = _autoPool.AddPoolObjectComponent(instance, info); // 2) PooledObject ���� �� PoolInfo ����
                instance.transform.SetParent(info.Parent);                       // 3) Ǯ ���� �θ� Ʈ������ �Ʒ��� ����
                info.Pool.Push(instance);                                        // 4) Ǯ ���ÿ� Ǫ��
                if (instance.gameObject.activeSelf)                              // 5) Ȱ��ȭ ���¿� ������ ��ȯ ó��
                {
                    info.ActiveCount++;                                          //    OnDisable ȣ�� �غ澸 ī��Ʈ ����
                    instance.gameObject.SetActive(false);                        //    ��Ȱ��ȭ → OnDisable → ActiveCount-- (��Ʈ ����)
                }
                else
                {
                    instance.gameObject.SetActive(false);                        //    �̹̺��Ȱ�� ������ �̺�Ʈ ¾���
                }
            }

            return info;
        }

        /// <summary>
        /// ������ ���� Ǯ�� ã�� ���� ��ü���� ����(�ʱ�ȭ)�մϴ�.
        /// </summary>
        public IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            ClearPool(info);
            return info;
        }

        /// <summary>
        /// ������Ʈ ������ ���� Ǯ�� ã�� ���� ��ü���� ����(�ʱ�ȭ)�մϴ�.
        /// </summary>
        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            ClearPool(info);
            return info;
        }

        /// <summary>
        /// Resources ��� ���� Ǯ�� ã�� ���� ��ü���� ����(�ʱ�ȭ)�մϴ�.
        /// </summary>
        public IPoolInfoReadOnly ClearResourcesPool(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            ClearPool(info);
            return info;
        }

        /// <summary>
        /// ���׸� Ÿ�� <typeparamref name="T"/> �� ���� ���׸� Ǯ�� ã�� ���� ��ü���� �����մϴ�.
        /// </summary>
        public IGenericPoolInfoReadOnly ClearGenericPool<T>() where T : class, IPoolGeneric, new()
        {
            GenericPoolInfo info = _autoPool.FindGenericPool<T>();
            ClearGenericPool(info);
            return info;
        }

        /// <summary>
        /// GameObject Ǯ�� ��� ��ü�� ���� ��Ȱ�� ���·� ǥ���մϴ�.
        /// </summary>
        public void ClearPool(PoolInfo info)
        {
            info.OnPoolDormant?.Invoke();                       // 1) Ǯ �޸� �ݹ� ȣ�� (������ ������Ʈ ���� ��)

            info.Pool = new Stack<GameObject>();                // 2) �� �������� ��ü�Ͽ� ���� ���۷��� ����
            info.IsActive = false;                              // 3) Ǯ ��Ȱ�� ���� �÷��� ����
        }

        /// <summary>
        /// ���׸� Ǯ�� ��� ��ü�� ���� ��Ȱ�� ���·� ǥ���մϴ�.
        /// </summary>
        public void ClearGenericPool(GenericPoolInfo info)
        {
            info.OnPoolDormant?.Invoke();                       // 1) ���׸� Ǯ �޸� �ݹ� ȣ��
            info.Pool = new Stack<IPoolGeneric>();              // 2) �� �������� ��ü�Ͽ� ���� ���۷��� ����
            info.IsActive = false;                              // 3) Ǯ ��Ȱ�� ���� �÷��� ����
        }
    }
}