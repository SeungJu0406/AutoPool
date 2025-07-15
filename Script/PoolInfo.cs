using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool
{
    /// <summary>
    /// 풀 정보 클래스입니다. 각 프리팹에 대한 풀 상태 및 정보를 저장합니다.
    /// 외부에서는 IPoolInfoReadOnly 인터페이스를 통해 읽기 전용으로 접근합니다.
    /// </summary>
    public class PoolInfo : IPoolInfoReadOnly
    {
        public bool IsMock = false;
        public Stack<GameObject> Pool;
        public GameObject Prefab;
        public Transform Parent;
        public bool IsActive;
        public bool IsUsed = true;
        public UnityAction OnPoolDormant;
        public int PoolCount;
        public int ActiveCount;

        bool IPoolInfoReadOnly.IsMock => IsMock;
        Stack<GameObject> IPoolInfoReadOnly.Pool => Pool;
        GameObject IPoolInfoReadOnly.Prefab => Prefab;
        string IPoolInfoReadOnly.Name => Prefab.name;
        Transform IPoolInfoReadOnly.Parent => Parent;
        bool IPoolInfoReadOnly.IsActive => IsActive;
        bool IPoolInfoReadOnly.IsUsed => IsUsed;
        UnityAction IPoolInfoReadOnly.OnPoolDormant { get => OnPoolDormant; set => OnPoolDormant = value; }
        int IPoolInfoReadOnly.PoolCount => PoolCount;
        int IPoolInfoReadOnly.ActiveCount => ActiveCount;
    }
}