using System.Collections.Generic;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolFindPoolHandler
    {
        AutoPool _autoPool;

        public AutoPoolFindPoolHandler(AutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// 해당 프리팹에 대한 풀 정보를 찾거나, 없으면 새로 생성합니다.
        /// 프리팹의 인스턴스 ID를 기준으로 Dictionary에서 관리됩니다.
        /// </summary>
        public PoolInfo FindPool(GameObject poolPrefab)
        {
            if (poolPrefab == null)
            {
                Debug.LogError($"{poolPrefab}가 참조되어 있지 않습니다");
                return null;
            }

            int prefabID = poolPrefab.GetInstanceID();

            PoolInfo pool = default;
            if (_autoPool.PoolDic.ContainsKey(prefabID) == false)
            {
                _autoPool.RegisterPool(poolPrefab, prefabID);
            }
            pool = _autoPool.PoolDic[prefabID];
            pool.IsUsed = true;
            _autoPool.PoolDic[prefabID] = pool;
            return pool;
        }

        /// <summary>
        /// 해당 프리팹에 대한 풀 정보를 찾거나, 없으면 새로 생성합니다.
        /// 프리팹의 인스턴스 ID를 기준으로 Dictionary에서 관리됩니다.
        /// </summary>
        public PoolInfo FindResourcesPool(string resources)
        {
            Dictionary<string, int> resourcePool = _autoPool.ResourcesPoolDic;
            PoolInfo pool = default;
            if (resourcePool.ContainsKey(resources) == false)
            {
                // 리소시스 프리팹 로드
                GameObject prefab = Resources.Load<GameObject>(resources);
                if (prefab == null)
                {
                    Debug.LogError($"Resources에 {resources}와 일치하는 리소스가 없습니다");
                    return null;
                }
                // 프리팹 instanceID값 캐싱
                int prefabID = prefab.GetInstanceID();
                // 풀에 등록
                _autoPool.RegisterPool(prefab, prefabID);
                // 리소시스 풀에 등록
                resourcePool.Add(resources, prefabID);
            }

            pool = _autoPool.PoolDic[resourcePool[resources]];
            pool.IsUsed = true;
            _autoPool.PoolDic[resourcePool[resources]] = pool;
            return pool;
        }

        /// <summary>
        /// 현재 풀에 재사용 가능한 오브젝트가 존재하는지 확인합니다.
        /// null 오브젝트가 껴 있으면 제거합니다.
        /// </summary>
        public bool FindObject(PoolInfo info)
        {
            if (info == null) return false;

            GameObject instance = null;
            while (true)
            {
                if (info.Pool.Count <= 0)
                    return false;

                instance = info.Pool.Peek();
                if (instance != null)
                    break;

                // null 제거
                info.Pool.Pop();
            }

            return true;

        }
    }
}