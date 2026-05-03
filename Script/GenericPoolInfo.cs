using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool_Tool
{
    /// <summary>
    /// 제네릭 객체 풀의 현재 상태와 메타데이터를 관리하고 노출하는 정보 클래스입니다.
    /// </summary>
    public class GenericPoolInfo : IGenericPoolInfoReadOnly
    {
        /// <summary>
        /// 실제 풀 동작 없이 테스트용 더미 풀인지 여부를 나타냅니다.
        /// </summary>
        public bool IsMock = false;

        /// <summary>
        /// 재사용 대기 중인 풀링 객체들이 저장된 스택 컬렉션입니다.
        /// </summary>
        public Stack<IPoolGeneric> Pool;

        /// <summary>
        /// 이 풀에서 관리하는 객체의 실제 런타임 타입입니다.
        /// </summary>
        public Type Type;

        /// <summary>
        /// 현재 풀이 활성 상태인지 여부입니다.
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// 풀이 사용 중인지 여부를 나타내며, 정리 대상인지 판단하는 데 사용될 수 있습니다.
        /// </summary>
        public bool IsUsed = true;

        /// <summary>
        /// 풀이 휴면 상태(활성 객체가 없고 더 이상 사용되지 않음)가 되었을 때 호출되는 콜백입니다.
        /// </summary>
        public UnityAction OnPoolDormant;

        /// <summary>
        /// 풀에 보관 중인 비활성 객체의 수입니다.
        /// </summary>
        public int PoolCount;

        /// <summary>
        /// 현재 활성화되어 사용 중인 객체의 수입니다.
        /// </summary>
        public int ActiveCount;

        /// <summary>
        /// 읽기 전용 인터페이스에서 풀의 모의 여부를 노출합니다.
        /// </summary>
        bool IGenericPoolInfoReadOnly.IsMock => IsMock;

        /// <summary>
        /// 읽기 전용 인터페이스에서 내부 풀 스택을 노출합니다.
        /// </summary>
        Stack<IPoolGeneric> IGenericPoolInfoReadOnly.Pool => Pool;

        /// <summary>
        /// 읽기 전용 인터페이스에서 관리 대상 타입 정보를 노출합니다.
        /// </summary>
        Type IGenericPoolInfoReadOnly.Type => Type;

        /// <summary>
        /// 읽기 전용 인터페이스에서 풀의 활성 상태를 노출합니다.
        /// </summary>
        bool IGenericPoolInfoReadOnly.IsActive => IsActive;

        /// <summary>
        /// 읽기 전용 인터페이스에서 풀의 사용 여부를 노출합니다.
        /// </summary>
        bool IGenericPoolInfoReadOnly.IsUsed => IsUsed;

        /// <summary>
        /// 읽기 전용 인터페이스에서 풀 휴면 시 호출되는 콜백을 노출 및 설정합니다.
        /// </summary>
        UnityAction IGenericPoolInfoReadOnly.OnPoolDormant
        {
            get => OnPoolDormant;
            set => OnPoolDormant = value;
        }

        /// <summary>
        /// 읽기 전용 인터페이스에서 풀에 대기 중인 객체 수를 노출합니다.
        /// </summary>
        int IGenericPoolInfoReadOnly.PoolCount => PoolCount;

        /// <summary>
        /// 읽기 전용 인터페이스에서 활성 객체 수를 노출합니다.
        /// </summary>
        int IGenericPoolInfoReadOnly.ActiveCount => ActiveCount;
    }
}