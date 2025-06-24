using System;
using UnityEngine;

namespace AutoPool
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // © 2025 NSJ. All rights reserved.
    public class PooledObject : MonoBehaviour
    {
        public IPoolInfoReadOnly PoolInfo;

        IPooledObject _poolObject;

        public Rigidbody CachedRb {  get; private set; }
        public Rigidbody2D CachedRb2D {  get; private set; }

        public event Action OnReturn;

        private void Awake()
        {
            _poolObject = GetComponent<IPooledObject>();
            CachedRb = GetComponent<Rigidbody>();
            CachedRb2D = GetComponent<Rigidbody2D>();
        }

        private void OnDisable()
        {
            if (AutoPool.Instance == null)
                return;

            OnReturn?.Invoke();
        }

        /// <summary>
        /// 풀링된 오브젝트가 활성화될 때 호출됩니다.
        /// 풀링된 오브젝트가 활성화될 때 초기화 작업을 수행합니다.
        /// </summary>
        public void OnCreateFromPool()
        {
            if (_poolObject != null)
            {
                _poolObject.OnCreateFromPool();
            }
        }

        public void OnReturnToPool()
        {
            if (_poolObject != null)
            {
                _poolObject.OnReturnToPool();
            }
        }

        /// <summary>
        /// 풀이 비활성화될 때 호출되는 이벤트를 구독합니다.
        /// </summary>
        public void SubscribePoolDeactivateEvent()
        {
            PoolInfo.OnPoolDormant += DestroyObject;
        }
        /// <summary>
        /// 풀이 비활성화될 때 호출됩니다.
        /// </summary>
        private void DestroyObject()
        {
            Destroy(gameObject);
            PoolInfo.OnPoolDormant -= DestroyObject;
        }
    }

    /// <summary>
    /// IPooledObject 인터페이스는 풀링된 오브젝트가 초기화될 때 호출되는 메서드를 정의합니다.
    /// </summary>
    public interface IPooledObject
    {
        void OnCreateFromPool();

        void OnReturnToPool();
    }
}
