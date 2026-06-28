using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool_Tool
{
    /// <summary>
    /// Read-only view of a GameObject pool's state and metadata.
    /// </summary>
    public interface IPoolInfoReadOnly
    {
        /// <summary>
        /// True if this is a mock pool used only for testing, with no real behaviour.
        /// </summary>
        public bool IsMock { get; }

        /// <summary>
        /// Stack of inactive PooledObject instances waiting for reuse.
        /// </summary>
        public Stack<PooledObject> Pool { get; }

        /// <summary>
        /// The source prefab this pool was created from.
        /// </summary>
        public GameObject Prefab { get; }

        /// <summary>
        /// Display name for this pool (mirrors the prefab name).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Parent transform under which pooled objects are parked when inactive.
        /// </summary>
        public Transform Parent { get; }

        /// <summary>
        /// Whether the pool is currently active (has ever been used).
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Whether the pool is in use or scheduled for cleanup.
        /// </summary>
        public bool IsUsed { get; }

        /// <summary>
        /// Callback fired when the pool enters a dormant state (all objects cleared).
        /// </summary>
        public UnityAction OnPoolDormant { get; set; }

        /// <summary>
        /// Total number of instances ever created for this pool (active + pooled).
        /// </summary>
        public int PoolCount { get; }

        /// <summary>
        /// Number of instances currently checked out and active in the scene.
        /// </summary>
        public int ActiveCount { get; }
    }

    /// <summary>
    /// Read-only view of a generic (non-GameObject) pool's state and metadata.
    /// </summary>
    public interface IGenericPoolInfoReadOnly
    {
        /// <summary>
        /// True if this is a mock pool used only for testing.
        /// </summary>
        public bool IsMock { get; }

        /// <summary>
        /// Stack of inactive generic instances waiting for reuse.
        /// </summary>
        public Stack<IPoolGeneric> Pool { get; }

        /// <summary>
        /// Runtime type of objects managed by this pool.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Whether the pool is currently active.
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// Whether the pool is in use or scheduled for cleanup.
        /// </summary>
        public bool IsUsed { get; }

        /// <summary>
        /// Callback fired when the pool enters a dormant state.
        /// </summary>
        public UnityAction OnPoolDormant { get; set; }

        /// <summary>
        /// Total number of instances ever created for this pool.
        /// </summary>
        public int PoolCount { get; }

        /// <summary>
        /// Number of instances currently in use.
        /// </summary>
        public int ActiveCount { get; }
    }
}
