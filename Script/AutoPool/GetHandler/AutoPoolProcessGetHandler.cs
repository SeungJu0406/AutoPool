using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoPool_Tool
{
    /// <summary>
    /// 실제 풀에서 객체를 꺼내거나 새로 생성해 위치/회전/부모 설정까지 처리하는 Get 프로세스 핸들러입니다.
    /// </summary>
    public class AutoPoolProcessGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        /// <summary>
        /// 메인 풀과 상위 Get 핸들러를 주입받아 초기화합니다.
        /// </summary>
        public AutoPoolProcessGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        /// <summary>
        /// 위치/회전 기본값(Zero/Identity)으로 GameObject를 가져옵니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))                            // 1) 풀에 유효한 인스턴스가 있는지 검사
            {
                instance = info.Pool.Pop();                            // 2) 스택에서 하나 꺼냄

                poolObject = instance.GetComponent<PooledObject>();    // 3) PooledObject 참조
                _autoPool.WakeUpRigidBody(poolObject);                 // 4) Rigidbody/2D 깨우기

                instance.transform.position = Vector3.zero;            // 5) 위치 초기화
                instance.transform.rotation = Quaternion.identity;     // 6) 회전 초기화
                instance.transform.SetParent(null);                    // 7) 부모 해제 (루트로 이동)
                instance.gameObject.SetActive(true);                   // 8) 활성화
                SceneManager.MoveGameObjectToScene(                    // 9) 현재 활성 씬으로 이동
                    instance,
                    SceneManager.GetActiveScene());
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab);        // 1) 풀에 없으면 새 인스턴스 생성
                poolObject = _autoPool.AddPoolObjectComponent(instance, info); // 2) 풀 관리용 컴포넌트 연결
            }

            poolObject.OnCreateFromPool();                             // 10) OnCreateFromPool 콜백 실행
            info.ActiveCount++;                                        // 11) 활성 개수 증가
            return instance;
        }

        /// <summary>
        /// 지정된 트랜스폼을 기준으로 GameObject를 가져옵니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))                            // 1) 풀에 유효한 인스턴스가 있는지 검사
            {
                instance = info.Pool.Pop();                            // 2) 스택에서 하나 꺼냄
                poolObject = instance.GetComponent<PooledObject>();    // 3) PooledObject 참조

                _autoPool.WakeUpRigidBody(poolObject);                 // 4) Rigidbody/2D 깨우기
                instance.transform.SetParent(transform);               // 5) 부모 트랜스폼 설정

                if (worldPositionStay == true)                         // 6) worldPositionStay 옵션에 따라
                {
                    instance.transform.position = info.Prefab.transform.position; //   프리팹의 월드 위치/회전 유지
                    instance.transform.rotation = info.Prefab.transform.rotation;
                }
                else
                {
                    instance.transform.position = transform.position;  //   부모 트랜스폼 기준 위치/회전 적용
                    instance.transform.rotation = transform.rotation;
                }

                instance.gameObject.SetActive(true);                   // 7) 활성화
            }
            else
            {
                instance = GameObject.Instantiate(                     // 1) Unity 기본 Instantiate API 사용
                    info.Prefab,
                    transform,
                    worldPositionStay);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info); // 2) 풀 관리용 컴포넌트 연결
            }

            poolObject.OnCreateFromPool();                             // 8) OnCreateFromPool 콜백 실행
            SetActiveCount(info);                                      // 9) 활성/풀 카운트 동기화
            return instance;
        }

        /// <summary>
        /// 지정된 위치와 회전으로 GameObject를 가져옵니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))                            // 1) 풀에 유효한 인스턴스 검사
            {
                instance = info.Pool.Pop();                            // 2) 스택에서 하나 꺼냄
                poolObject = instance.GetComponent<PooledObject>();    // 3) PooledObject 참조

                _autoPool.WakeUpRigidBody(poolObject);                 // 4) Rigidbody/2D 깨우기
                instance.transform.position = pos;                     // 5) 위치 설정
                instance.transform.rotation = rot;                     // 6) 회전 설정
                instance.transform.SetParent(null);                    // 7) 부모 해제
                instance.gameObject.SetActive(true);                   // 8) 활성화
                SceneManager.MoveGameObjectToScene(                    // 9) 현재 활성 씬으로 이동
                    instance,
                    SceneManager.GetActiveScene());
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab, pos, rot);        // 1) 새 인스턴스 생성
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);   // 2) 풀 관리용 컴포넌트 연결
            }

            poolObject.OnCreateFromPool();                             // 10) OnCreateFromPool 콜백 실행
            SetActiveCount(info);                                      // 11) 활성/풀 카운트 동기화
            return instance;
        }

        /// <summary>
        /// 제네릭 풀에서 타입 <typeparamref name="T"/> 인스턴스를 가져오거나 새로 생성합니다.
        /// </summary>
        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new()
        {
            T instance = null;
            IPoolGeneric poolGeneric = null;

            if (_autoPool.FindGeneric<T>(poolInfo))                    // 1) 제네릭 풀에 유효한 인스턴스 있는지 검사
            {
                poolGeneric = poolInfo.Pool.Pop();                     // 2) 스택에서 꺼내고
                instance = (T)poolGeneric;                             // 3) 구체 타입으로 캐스팅
            }
            else
            {
                instance = new T();                                    // 1) 새 인스턴스 생성
                poolGeneric = (IPoolGeneric)instance;                  // 2) 인터페이스로 캐스팅
                poolGeneric.Pool = new PoolGenericInfo();              // 3) 개별 객체용 PoolGenericInfo 생성
                poolGeneric.Pool.PoolInfo = poolInfo;                  // 4) 상위 GenericPoolInfo 연결
                poolInfo.PoolCount++;                                  // 5) 풀에 속한 총 개수 증가
                poolInfo.OnPoolDormant += poolGeneric.OnReturnToPool;  // 6) 풀 휴면 시 객체 반환 콜백 연결
            }

            SetActiveCount(poolInfo);                                  // 7) 활성/풀 카운트 동기화
            poolGeneric.Pool.IsActive = true;                          // 8) 개별 객체 활성 플래그 설정
            poolGeneric.OnCreateFromPool();                            // 9) OnCreateFromPool 콜백 실행
            return instance;
        }

        /// <summary>
        /// GameObject 풀의 활성 개수를 증가시키고 최대 개수를 갱신합니다.
        /// </summary>
        private void SetActiveCount(PoolInfo info)
        {
            info.ActiveCount++;
            if (info.PoolCount < info.ActiveCount)
            {
                info.PoolCount = info.ActiveCount;
            }
        }

        /// <summary>
        /// 제네릭 풀의 활성 개수를 증가시키고 최대 개수를 갱신합니다.
        /// </summary>
        private void SetActiveCount(GenericPoolInfo info)
        {
            info.ActiveCount++;
            if (info.PoolCount < info.ActiveCount)
            {
                info.PoolCount = info.ActiveCount;
            }
        }
    }
}