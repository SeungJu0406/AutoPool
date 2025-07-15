using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool
{
    public class GenericPoolInfo : IGenericPoolInfoReadOnly
    {
        public bool IsMock = false;
        public Stack<IPoolGeneric> Pool;
        public Type Type;
        public bool IsActive;
        public bool IsUsed = true;
        public UnityAction OnPoolDormant;
        public int PoolCount;
        public int ActiveCount;

        bool IGenericPoolInfoReadOnly.IsMock => IsMock;
        Stack<IPoolGeneric> IGenericPoolInfoReadOnly.Pool => Pool;
        Type IGenericPoolInfoReadOnly.Type => Type;
        bool IGenericPoolInfoReadOnly.IsActive => IsActive;
        bool IGenericPoolInfoReadOnly.IsUsed => IsUsed;
        UnityAction IGenericPoolInfoReadOnly.OnPoolDormant { get => OnPoolDormant; set => OnPoolDormant = value; }
        int IGenericPoolInfoReadOnly.PoolCount => PoolCount;
        int IGenericPoolInfoReadOnly.ActiveCount => ActiveCount;
    }
}