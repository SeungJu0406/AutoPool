using System;
using UnityEngine;

namespace AutoPool_Tool
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // © 2025 NSJ. All rights reserved.
    public class PooledObject : MonoBehaviour
    {
        public PoolInfo PoolInfo;

        IPooledObject _poolObject;

        public Rigidbody CachedRb { get; private set; }
        public Rigidbody2D CachedRb2D { get; private set; }

        public event Action OnReturn;

        private void Awake()
        {
            _poolObject = GetComponent<IPooledObject>();
            CachedRb = GetComponent<Rigidbody>();
            CachedRb2D = GetComponent<Rigidbody2D>();
        }

        private void OnDisable()
        {
            if (ObjectPool.HasPool == false)
                return;
            PoolInfo.ActiveCount--;
            OnReturn?.Invoke();
        }
        private void OnDestroy()
        {
            if (ObjectPool.HasPool == false)
                return;

            PoolInfo.PoolCount--;
            PoolInfo.OnPoolDormant -= DestroyObject;
        }
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

        public void SubscribePoolDeactivateEvent()
        {
            PoolInfo.OnPoolDormant += DestroyObject;
        }

        private void DestroyObject()
        {
            OnReturnToPool();
            Destroy(gameObject);
        }


    }


}
