using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Implement this interface on a MonoBehaviour to receive pool lifecycle callbacks.
    /// </summary>
    public interface IPooledObject
    {
        /// <summary>
        /// Called when the object is retrieved from the pool.
        /// </summary>
        void OnCreateFromPool();

        /// <summary>
        /// Called when the object is returned to the pool.
        /// </summary>
        void OnReturnToPool();
    }

    /// <summary>
    /// Implement this interface on a plain C# class to make it compatible with the generic pool.
    /// </summary>
    public interface IPoolGeneric
    {
        /// <summary>
        /// Per-instance generic pool state managed by the pool system.
        /// </summary>
        PoolGenericInfo Pool { get; set; }

        /// <summary>
        /// Called when the object is retrieved from the generic pool.
        /// </summary>
        void OnCreateFromPool();

        /// <summary>
        /// Called when the object is returned to the generic pool.
        /// </summary>
        void OnReturnToPool();
    }

    /// <summary>
    /// Per-instance state container for objects managed by the generic pool.
    /// </summary>
    public class PoolGenericInfo
    {
        /// <summary>
        /// Reference to the shared pool this instance belongs to.
        /// </summary>
        public GenericPoolInfo PoolInfo;

        /// <summary>
        /// Whether this instance is currently active (in use).
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// Fired when this instance is returned to the pool.
        /// </summary>
        public event System.Action OnReturn;

        /// <summary>
        /// Invokes the OnReturn event.
        /// </summary>
        public void Return()
        {
            OnReturn?.Invoke();
        }
    }
}
