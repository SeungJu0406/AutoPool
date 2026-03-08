        using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool_Tool
{
    /// <summary>
    /// 각 프리팹에 대한 풀 상태와 메타데이터를 저장하는 정보 클래스입니다.
    /// </summary>
    public class PoolInfo : IPoolInfoReadOnly
    {
        /// <summary>
        /// 실제 풀 동작 없이 테스트용으로만 사용하는 모의 풀인지 여부입니다.
        /// </summary>
        public bool IsMock = false;

        /// <summary>
        /// 재사용 대기 중인 GameObject 인스턴스가 저장된 스택입니다.
        /// </summary>
        public Stack<GameObject> Pool;

        /// <summary>
        /// 이 풀에서 관리하는 기준 프리팹입니다.
        /// </summary>
        public GameObject Prefab;

        /// <summary>
        /// 풀에서 생성된 객체들이 할당되는 부모 트랜스폼입니다.
        /// </summary>
        public Transform Parent;

        /// <summary>
        /// 현재 풀이 활성 상태인지 여부입니다.
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// 풀이 사용 중인지, 정리 대상인지 여부입니다.
        /// </summary>
        public bool IsUsed = true;

        /// <summary>
        /// 풀이 휴면 상태(더 이상 사용되지 않음)가 되었을 때 호출되는 콜백입니다.
        /// </summary>
        public UnityAction OnPoolDormant;

        /// <summary>
        /// 풀에 대기 중인 비활성 객체의 개수입니다.
        /// </summary>
        public int PoolCount;

        /// <summary>
        /// 현재 활성 상태로 사용 중인 객체의 개수입니다.
        /// </summary>
        public int ActiveCount;

        /// <summary>
        /// 읽기 전용 인터페이스에서 모의 풀 여부를 제공합니다.
        /// </summary>
        bool IPoolInfoReadOnly.IsMock => IsMock;

        /// <summary>
        /// 읽기 전용 인터페이스에서 내부 스택을 제공합니다.
        /// </summary>
        Stack<GameObject> IPoolInfoReadOnly.Pool => Pool;

        /// <summary>
        /// 읽기 전용 인터페이스에서 기준 프리팹을 제공합니다.
        /// </summary>
        GameObject IPoolInfoReadOnly.Prefab => Prefab;

        /// <summary>
        /// 읽기 전용 인터페이스에서 풀 이름(프리팹 이름)을 제공합니다.
        /// </summary>
        string IPoolInfoReadOnly.Name => Prefab.name;

        /// <summary>
        /// 읽기 전용 인터페이스에서 부모 트랜스폼을 제공합니다.
        /// </summary>
        Transform IPoolInfoReadOnly.Parent => Parent;

        /// <summary>
        /// 읽기 전용 인터페이스에서 활성 상태를 제공합니다.
        /// </summary>
        bool IPoolInfoReadOnly.IsActive => IsActive;

        /// <summary>
        /// 읽기 전용 인터페이스에서 사용 여부를 제공합니다.
        /// </summary>
        bool IPoolInfoReadOnly.IsUsed => IsUsed;

        /// <summary>
        /// 읽기 전용 인터페이스에서 풀 휴면 콜백을 가져오거나 설정합니다.
        /// </summary>
        UnityAction IPoolInfoReadOnly.OnPoolDormant
        {
            get => OnPoolDormant;
            set => OnPoolDormant = value;
        }

        /// <summary>
        /// 읽기 전용 인터페이스에서 대기 중인 객체 수를 제공합니다.
        /// </summary>
        int IPoolInfoReadOnly.PoolCount => PoolCount;

        /// <summary>
        /// 읽기 전용 인터페이스에서 활성 객체 수를 제공합니다.
        /// </summary>
        int IPoolInfoReadOnly.ActiveCount => ActiveCount;
    }
}