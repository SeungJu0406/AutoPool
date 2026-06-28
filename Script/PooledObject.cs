using System;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Attached to every pooled GameObject to track pool state, fire lifecycle callbacks,
    /// and cache frequently accessed components.
    /// </summary>
    public class PooledObject : MonoBehaviour
    {
        /// <summary>
        /// Pool metadata for the pool this object belongs to.
        /// </summary>
        public PoolInfo PoolInfo;

        /// <summary>
        /// Cached reference to an IPooledObject implementation on this GameObject, if any.
        /// </summary>
        IPooledObject _poolObject;

        /// <summary>
        /// Cached Rigidbody to avoid repeated GetComponent calls on sleep/wake.
        /// </summary>
        public Rigidbody CachedRb { get; private set; }

        /// <summary>
        /// Cached Rigidbody2D to avoid repeated GetComponent calls on sleep/wake.
        /// </summary>
        public Rigidbody2D CachedRb2D { get; private set; }

        /// <summary>
        /// Fired when the object is deactivated (returned to the pool).
        /// Used internally to cancel pending delayed-return coroutines.
        /// </summary>
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

        /// <summary>
        /// Forwards the create-from-pool event to the IPooledObject implementation, if present.
        /// </summary>
        public void OnCreateFromPool()
        {
            if (_poolObject != null)
                _poolObject.OnCreateFromPool();
        }

        /// <summary>
        /// Forwards the return-to-pool event to the IPooledObject implementation, if present.
        /// </summary>
        public void OnReturnToPool()
        {
            if (_poolObject != null)
                _poolObject.OnReturnToPool();
        }

        /// <summary>
        /// Registers DestroyObject to be called when the pool goes dormant,
        /// so preloaded objects are cleaned up automatically.
        /// </summary>
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
