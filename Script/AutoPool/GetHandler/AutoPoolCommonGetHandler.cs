using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 공통 GameObject/컴포넌트 Get 요청을 처리하는 핸들러로,
    /// 풀 조회와 실제 인스턴스 생성 로직(<see cref="AutoPoolGetHandler"/>)을 중개합니다.
    /// </summary>
    public class AutoPoolCommonGetHandler
    {
        MainAutoPool _autoPool;
        AutoPoolGetHandler _getHandler;

        /// <summary>
        /// 메인 풀과 Get 전용 핸들러를 주입받아 공통 Get 핸들러를 초기화합니다.
        /// </summary>
        public AutoPoolCommonGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _getHandler = getHandler;
        }

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public GameObject Get(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);                // 1) 프리팹 기준 풀 검색/생성
            GameObject instance = _getHandler.ProcessGet(info);       // 2) 풀에서 실제 인스턴스 가져오기
            return instance;                                          // 3) 결과 반환
        }

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = _autoPool.FindPool(prefab);               // 1) 프리팹 기준 풀 검색/생성
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay); // 2) 트랜스폼 위치에 배치하며 Get
            return instance;                                          // 3) 결과 반환
        }

        /// <summary>
        /// 프리팹 기반 GameObject 인스턴스를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = _autoPool.FindPool(prefab);               // 1) 프리팹 기준 풀 검색/생성
            GameObject instance = _getHandler.ProcessGet(info, pos, rot); // 2) 지정된 위치/회전으로 Get
            return instance;                                          // 3) 결과 반환
        }

        /// <summary>
        /// 컴포넌트 프리팹을 기준으로 풀에서 GameObject를 가져온 뒤, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);    // 1) 프리팹 GameObject 기준 풀 검색/생성
            GameObject instance = _getHandler.ProcessGet(info);       // 2) 인스턴스 Get
            T component = instance.GetComponent<T>();                  // 3) 요청된 타입 컴포넌트 획득
            return component;                                         // 4) 컴포넌트 반환
        }

        /// <summary>
        /// 컴포넌트 프리팹을 기준으로 인스턴스를 가져와 지정된 트랜스폼에 배치한 뒤, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);    // 1) 프리팹 GameObject 기준 풀 검색/생성
            GameObject instance = _getHandler.ProcessGet(info, transform, worldPositionStay); // 2) 트랜스폼 위치로 Get
            T component = instance.GetComponent<T>();                  // 3) 요청된 타입 컴포넌트 획득
            return component;                                         // 4) 컴포넌트 반환
        }

        /// <summary>
        /// 컴포넌트 프리팹을 기준으로 인스턴스를 가져와 위치/회전을 설정한 뒤, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);    // 1) 프리팹 GameObject 기준 풀 검색/생성
            GameObject instance = _getHandler.ProcessGet(info, pos, rot); // 2) 지정된 위치/회전으로 Get
            T component = instance.GetComponent<T>();                  // 3) 요청된 타입 컴포넌트 획득
            return component;                                         // 4) 컴포넌트 반환
        }
    }
}