using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool_Tool
{

    /// <summary>
    /// GameObject 풀의 읽기 전용 상태 정보를 제공하는 인터페이스입니다.
    /// </summary>
    public interface IPoolInfoReadOnly
    {
        /// <summary>
        /// 실제 동작 없이 테스트용으로만 사용되는 모의 풀인지 여부입니다.
        /// </summary>
        public bool IsMock { get; }

        /// <summary>
        /// 재사용 대기 중인 GameObject 인스턴스가 저장된 스택입니다.
        /// </summary>
        public Stack<GameObject> Pool { get; }

        /// <summary>
        /// 이 풀에서 관리하는 기준 프리팹입니다.
        /// </summary>
        public GameObject Prefab { get; }

        /// <summary>
        /// 풀의 이름 또는 식별자입니다.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 풀에서 생성된 객체들이 할당되는 부모 트랜스폼입니다.
        /// </summary>
        public Transform Parent { get; }

        /// <summary>
        /// 현재 풀이 활성 상태인지 여부입니다.
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// 풀이 사용 중인지, 혹은 정리 대상인지 여부입니다.
        /// </summary>
        public bool IsUsed { get; }

        /// <summary>
        /// 풀이 휴면 상태가 되었을 때 호출되는 콜백입니다.
        /// </summary>
        public UnityAction OnPoolDormant { get; set; }

        /// <summary>
        /// 풀에 대기 중인 비활성 객체의 개수입니다.
        /// </summary>
        public int PoolCount { get; }

        /// <summary>
        /// 현재 활성 상태로 사용 중인 객체의 개수입니다.
        /// </summary>
        public int ActiveCount { get; }
    }

    /// <summary>
    /// 제네릭 풀의 읽기 전용 상태 정보를 제공하는 인터페이스입니다.
    /// </summary>
    public interface IGenericPoolInfoReadOnly
    {
        /// <summary>
        /// 실제 동작 없이 테스트용으로만 사용되는 모의 풀인지 여부입니다.
        /// </summary>
        public bool IsMock { get; }

        /// <summary>
        /// 재사용 대기 중인 제네릭 풀 객체들이 저장된 스택입니다.
        /// </summary>
        public Stack<IPoolGeneric> Pool { get; }

        /// <summary>
        /// 이 풀에서 관리하는 객체의 런타임 타입입니다.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 현재 제네릭 풀이 활성 상태인지 여부입니다.
        /// </summary>
        public bool IsActive { get; }

        /// <summary>
        /// 풀이 사용 중인지, 혹은 정리 대상인지 여부입니다.
        /// </summary>
        public bool IsUsed { get; }

        /// <summary>
        /// 제네릭 풀이 휴면 상태가 되었을 때 호출되는 콜백입니다.
        /// </summary>
        public UnityAction OnPoolDormant { get; set; }

        /// <summary>
        /// 풀에 대기 중인 객체의 개수입니다.
        /// </summary>
        public int PoolCount { get; }

        /// <summary>
        /// 현재 활성 상태로 사용 중인 객체의 개수입니다.
        /// </summary>
        public int ActiveCount { get; }
    }
}