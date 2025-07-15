using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool
{
    /// <summary>
    /// Ǯ ���� Ŭ�����Դϴ�. �� �����տ� ���� Ǯ ���� �� ������ �����մϴ�.
    /// �ܺο����� IPoolInfoReadOnly �������̽��� ���� �б� �������� �����մϴ�.
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