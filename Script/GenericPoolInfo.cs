using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool_Tool
{
    /// <summary>
    /// Holds the runtime state and metadata for a single generic (non-GameObject) pool entry.
    /// </summary>
    public class GenericPoolInfo : IGenericPoolInfoReadOnly
    {
        /// <summary>
        /// When true this pool is a test stub and performs no real pooling.
        /// </summary>
        public bool IsMock = false;

        /// <summary>
        /// Stack of inactive instances waiting to be reused.
        /// </summary>
        public Stack<IPoolGeneric> Pool;

        /// <summary>
        /// Runtime type of objects managed by this pool.
        /// </summary>
        public Type Type;

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
        /// Total number of instances ever created for this pool.
        /// </summary>
        public int PoolCount;

        /// <summary>
        /// Number of instances currently in use.
        /// </summary>
        public int ActiveCount;

        bool IGenericPoolInfoReadOnly.IsMock => IsMock;
        Stack<IPoolGeneric> IGenericPoolInfoReadOnly.Pool => Pool;
        Type IGenericPoolInfoReadOnly.Type => Type;
        bool IGenericPoolInfoReadOnly.IsActive => IsActive;
        bool IGenericPoolInfoReadOnly.IsUsed => IsUsed;
        UnityAction IGenericPoolInfoReadOnly.OnPoolDormant
        {
            get => OnPoolDormant;
            set => OnPoolDormant = value;
        }
        int IGenericPoolInfoReadOnly.PoolCount => PoolCount;
        int IGenericPoolInfoReadOnly.ActiveCount => ActiveCount;
    }
}
