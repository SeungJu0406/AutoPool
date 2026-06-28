using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Base interface for objects that receive callbacks when retrieved from or returned to the pool.
    /// </summary>
    public interface IPooledObject
    {
        /// <summary>
        /// Called when the object is first created or reused from the pool.
        /// </summary>
        void OnCreateFromPool();

        /// <summary>
        /// Called when the object is returned to the pool.
        /// </summary>
        void OnReturnToPool();
    }

    /// <summary>
    /// Interface for objects managed by a generic (non-GameObject) pool.
    /// </summary>
    public interface IPoolGeneric
    {
        /// <summary>
        /// Per-instance generic pool state for this object.
        /// </summary>
        PoolGenericInfo Pool { get; set; }

        /// <summary>
        /// Called when the object is first created or reused from the generic pool.
        /// </summary>
        void OnCreateFromPool();

        /// <summary>
        /// Called when the object is returned to the generic pool.
        /// </summary>
        void OnReturnToPool();
    }

    /// <summary>
    /// Tracks the active state and return event for a single instance in a generic pool.
    /// </summary>
    public class PoolGenericInfo
    {
        /// <summary>
        /// Reference to the shared pool this instance belongs to.
        /// </summary>
        public GenericPoolInfo PoolInfo;

        /// <summary>
        /// Whether this instance is currently in use.
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// Fired when the instance is returned to the pool.
        /// </summary>
        public event System.Action OnReturn;

        /// <summary>
        /// Invokes the return event.
        /// </summary>
        public void Return()
        {
            OnReturn?.Invoke();
        }
    }
}
