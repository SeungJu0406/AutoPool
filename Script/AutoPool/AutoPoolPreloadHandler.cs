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
        /// 풀을 미리 정의된 개수만큼 생성합니다.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. 컴포넌트를 타입으로 지정할 수 있습니다.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. Resources에 저장된 프리팹을 기준으로 합니다.
        /// Sets the preload count for a specific prefab in the pool using a Resources path.
        /// </summary>
        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return ProcessPreload(info, count);
        }
        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성하는 과정의 메서드입니다
        /// This method processes the preload operation for a given PoolInfo.
        /// </summary>
        private IPoolInfoReadOnly ProcessPreload(PoolInfo info, int count)
        {
            if (info == null)
            {
                Debug.LogError("풀 정보가 유효하지 않습니다.");
                return null;
            }
            // count 수치까지 미리 오브젝트를 생성하고 풀에 추가합니다.
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
        /// 지정된 ID의 풀을 비우고 비활성화 처리합니다.
        /// OnPoolDormant 이벤트가 있다면 호출됩니다.
        /// </summary>
        public void ClearPool(PoolInfo info)
        {
            info.OnPoolDormant?.Invoke();

            info.Pool = new Stack<GameObject>();
            info.IsActive = false;
        }

    }
}