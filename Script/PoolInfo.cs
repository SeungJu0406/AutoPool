using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool_Tool
{
    /// <summary>
    /// Holds the runtime state and metadata for a single GameObject pool entry.
    /// </summary>
    public class PoolInfo : IPoolInfoReadOnly
    {
        /// <summary>
        /// When true this pool is a test stub and performs no real pooling.
        /// </summary>
        public bool IsMock = false;

        /// <summary>
        /// Stack of inactive instances waiting to be reused.
        /// </summary>
        public Stack<GameObject> Pool;

        /// <summary>
        /// Source prefab used to instantiate new instances.
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// Parent transform under which inactive instances are parked.
        /// </summary>
        public Transform Parent;

        /// <summary>
        /// Whether this pool has been used at least once.
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// Whether this pool is currently in use (false means it may be cleared).
        /// </summary>
        public bool IsUsed = true;

        /// <summary>
        /// Fired when the pool goes dormant (all instances cleared).
        /// </summary>
        public UnityAction OnPoolDormant;

        /// <summary>
        /// Total number of instances ever created for this pool (active + pooled).
        /// </summary>
        public int PoolCount;

        /// <summary>
        /// Number of instances currently checked out and active in the scene.
        /// </summary>
        public int ActiveCount;

        bool IPoolInfoReadOnly.IsMock => IsMock;
        Stack<GameObject> IPoolInfoReadOnly.Pool => Pool;
        GameObject IPoolInfoReadOnly.Prefab => Prefab;
        string IPoolInfoReadOnly.Name => Prefab.name;
        Transform IPoolInfoReadOnly.Parent => Parent;
        bool IPoolInfoReadOnly.IsActive => IsActive;
        bool IPoolInfoReadOnly.IsUsed => IsUsed;
        UnityAction IPoolInfoReadOnly.OnPoolDormant
        {
            get => OnPoolDormant;
            set => OnPoolDormant = value;
        }
        int IPoolInfoReadOnly.PoolCount => PoolCount;
        int IPoolInfoReadOnly.ActiveCount => ActiveCount;
    }
}
