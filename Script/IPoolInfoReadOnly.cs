using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool_Tool
{
    /// <summary>
    /// Read-only view of a GameObject pool's runtime state.
    /// </summary>
    public interface IPoolInfoReadOnly
    {
        /// <summary>Whether this is a mock pool used for testing.</summary>
        public bool IsMock { get; }

        /// <summary>Stack of inactive instances waiting to be reused.</summary>
        public Stack<GameObject> Pool { get; }

        /// <summary>The source prefab this pool was created from.</summary>
        public GameObject Prefab { get; }

        /// <summary>Name identifier for this pool (prefab name).</summary>
        public string Name { get; }

        /// <summary>Parent transform that holds inactive pooled instances.</summary>
        public Transform Parent { get; }

        /// <summary>Whether this pool is currently active.</summary>
        public bool IsActive { get; }

        /// <summary>Whether this pool is in use or pending cleanup.</summary>
        public bool IsUsed { get; }

        /// <summary>Callback invoked when the pool enters a dormant state.</summary>
        public UnityAction OnPoolDormant { get; set; }

        /// <summary>Total number of instances managed by this pool.</summary>
        public int PoolCount { get; }

        /// <summary>Number of instances currently active (in use).</summary>
        public int ActiveCount { get; }
    }

    /// <summary>
    /// Read-only view of a generic pool's runtime state.
    /// </summary>
    public interface IGenericPoolInfoReadOnly
    {
        /// <summary>Whether this is a mock pool used for testing.</summary>
        public bool IsMock { get; }

        /// <summary>Stack of inactive instances waiting to be reused.</summary>
        public Stack<IPoolGeneric> Pool { get; }

        /// <summary>The runtime type of objects managed by this pool.</summary>
        public Type Type { get; }

        /// <summary>Whether this pool is currently active.</summary>
        public bool IsActive { get; }

        /// <summary>Whether this pool is in use or pending cleanup.</summary>
        public bool IsUsed { get; }

        /// <summary>Callback invoked when the pool enters a dormant state.</summary>
        public UnityAction OnPoolDormant { get; set; }

        /// <summary>Total number of instances managed by this pool.</summary>
        public int PoolCount { get; }

        /// <summary>Number of instances currently active (in use).</summary>
        public int ActiveCount { get; }
    }
}
