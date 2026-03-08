using System;
using UnityEngine;

namespace AutoPool_Tool
{

    /// <summary>
    /// 풀링된 GameObject에 부착되어 풀 상태, 콜백, 캐시된 컴포넌트를 관리하는 컴포넌트입니다.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        /// <summary>
        /// 이 오브젝트가 속한 풀의 상태 정보입니다.
        /// </summary>
        public PoolInfo PoolInfo;

        /// <summary>
        /// IPooledObject 구현체 캐시입니다. 존재 시 풀 이벤트를 위임합니다.
        /// </summary>
        IPooledObject _poolObject;

        /// <summary>
        /// Rigidbody 구성 요소의 캐시입니다. 자주 접근 시 GetComponent 호출 비용을 줄입니다.
        /// </summary>
        public Rigidbody CachedRb { get; private set; }

        /// <summary>
        /// Rigidbody2D 구성 요소의 캐시입니다.
        /// </summary>
        public Rigidbody2D CachedRb2D { get; private set; }

        /// <summary>
        /// 오브젝트가 풀로 반환될 때(비활성화 시) 호출되는 이벤트입니다.
        /// </summary>
        public event Action OnReturn;

        /// <summary>
        /// 초기화 시 필요한 컴포넌트(IPooledObject, Rigidbody, Rigidbody2D)를 캐시합니다.
        /// </summary>
        private void Awake()
        {
            _poolObject = GetComponent<IPooledObject>(); // 같은 GameObject에서 IPooledObject 구현 찾기
            CachedRb = GetComponent<Rigidbody>();        // 3D 물리용 Rigidbody 캐시
            CachedRb2D = GetComponent<Rigidbody2D>();    // 2D 물리용 Rigidbody2D 캐시
        }

        /// <summary>
        /// 오브젝트가 비활성화될 때 풀의 활성 카운트를 감소시키고 반환 이벤트를 발생시킵니다.
        /// </summary>
        private void OnDisable()
        {
            if (ObjectPool.HasPool == false)             // 풀 인스턴스가 없는 경우(풀 이미 정리됨) 아무 것도 하지 않음
                return;

            PoolInfo.ActiveCount--;                      // 활성 객체 수 감소
            OnReturn?.Invoke();                          // 구독된 반환 콜백 호출 (예: 자동 Return 처리 등)
        }

        /// <summary>
        /// 오브젝트가 파괴될 때 풀 카운트를 정리하고 풀 휴면 이벤트 구독을 해제합니다.
        /// </summary>
        private void OnDestroy()
        {
            if (ObjectPool.HasPool == false)             // 풀 인스턴스가 없는 경우 더 이상 관리하지 않음
                return;

            PoolInfo.PoolCount--;                        // 풀에 속한 총 수량 감소
            PoolInfo.OnPoolDormant -= DestroyObject;     // 휴면 이벤트에서 이 오브젝트 파괴 콜백 제거
        }

        /// <summary>
        /// 풀에서 생성되거나 재사용될 때 연결된 IPooledObject 콜백을 호출합니다.
        /// </summary>
        public void OnCreateFromPool()
        {
            if (_poolObject != null)                     // IPooledObject를 구현한 스크립트가 있으면
            {
                _poolObject.OnCreateFromPool();          // 해당 스크립트의 생성 콜백 호출
            }
        }

        /// <summary>
        /// 풀로 반환될 때 연결된 IPooledObject 콜백을 호출합니다.
        /// </summary>
        public void OnReturnToPool()
        {
            if (_poolObject != null)                     // IPooledObject를 구현한 스크립트가 있으면
            {
                _poolObject.OnReturnToPool();            // 해당 스크립트의 반환 콜백 호출
            }
        }

        /// <summary>
        /// 풀 휴면(더 이상 사용되지 않음) 시 이 오브젝트가 자동으로 파괴되도록 이벤트를 구독합니다.
        /// </summary>
        public void SubscribePoolDeactivateEvent()
        {
            PoolInfo.OnPoolDormant += DestroyObject;     // 풀 휴면 이벤트에 DestroyObject 등록
        }

        /// <summary>
        /// 풀 휴면 시 호출되어, 반환 콜백을 실행하고 GameObject를 파괴합니다.
        /// </summary>
        private void DestroyObject()
        {
            OnReturnToPool();                            // 먼저 IPooledObject 반환 콜백 처리
            Destroy(gameObject);                         // 실제 GameObject 파괴
        }
    }
}
