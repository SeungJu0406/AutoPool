using System.Collections.Generic;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolPreloadHandler
    {
        AutoPool _autoPool;

        public AutoPoolPreloadHandler(AutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// Ǯ�� �̸� ���ǵ� ������ŭ �����մϴ�.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// Ǯ�� �̸� ���ǵ� ������ŭ �����մϴ�. ������Ʈ�� Ÿ������ ������ �� �ֽ��ϴ�.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// Ǯ�� �̸� ���ǵ� ������ŭ �����մϴ�. Resources�� ����� �������� �������� �մϴ�.
        /// Sets the preload count for a specific prefab in the pool using a Resources path.
        /// </summary>
        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return ProcessPreload(info, count);
        }
        /// <summary>
        /// Ǯ�� �̸� ���ǵ� ������ŭ �����ϴ� ������ �޼����Դϴ�
        /// This method processes the preload operation for a given PoolInfo.
        /// </summary>
        private IPoolInfoReadOnly ProcessPreload(PoolInfo info, int count)
        {
            if (info == null)
            {
                Debug.LogError("Ǯ ������ ��ȿ���� �ʽ��ϴ�.");
                return null;
            }
            // count ��ġ���� �̸� ������Ʈ�� �����ϰ� Ǯ�� �߰��մϴ�.
            while (info.PoolCount < count)
            {
                GameObject instance = GameObject.Instantiate(info.Prefab);
                PooledObject poolObject = _autoPool.AddPoolObjectComponent(instance, info);
                instance.transform.SetParent(info.Parent);
                info.Pool.Push(instance);
                info.ActiveCount++;
                instance.gameObject.SetActive(false);
            }
            return info;
        }

        public IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            ClearPool(info);
            return info;
        }

        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            ClearPool(info);
            return info;
        }

        public IPoolInfoReadOnly ClearResourcesPool(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            ClearPool(info);
            return info;
        }

        /// <summary>
        /// ������ ID�� Ǯ�� ���� ��Ȱ��ȭ ó���մϴ�.
        /// OnPoolDormant �̺�Ʈ�� �ִٸ� ȣ��˴ϴ�.
        /// </summary>
        public void ClearPool(PoolInfo info)
        {
            info.OnPoolDormant?.Invoke();

            info.Pool = new Stack<GameObject>();
            info.IsActive = false;
        }

    }
}