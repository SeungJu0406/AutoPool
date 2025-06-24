using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NSJ_EasyPoolKit
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // © 2025 NSJ. All rights reserved.
    public interface IPoolInfoReadOnly
    {
        public bool IsMock { get; }
        public Stack<GameObject> Pool { get; }
        public GameObject Prefab { get; }
        public string Name { get; }
        public Transform Parent { get; }
        public bool IsActive { get; }
        public bool IsUsed { get; }
        public UnityAction OnPoolDormant { get; set; }
        public int PoolCount { get; }
        public int ActiveCount { get; }
    }
}