using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 풀에서 생성 및 반환될 때 콜백을 제공하는 기본 풀 오브젝트 인터페이스입니다.
    /// </summary>
    public interface IPooledObject
    {
        /// <summary>
        /// 객체가 풀에서 처음 생성되거나 다시 가져올 때 호출됩니다.
        /// </summary>
        void OnCreateFromPool();

        /// <summary>
        /// 객체가 풀로 반환될 때 호출됩니다.
        /// </summary>
        void OnReturnToPool();
    }

    /// <summary>
    /// 제네릭 풀 관리 정보를 포함하는 풀 오브젝트 인터페이스입니다.
    /// </summary>
    public interface IPoolGeneric
    {
        /// <summary>
        /// 이 객체가 속한 제네릭 풀 관련 정보입니다.
        /// </summary>
        PoolGenericInfo Pool { get; set; }

        /// <summary>
        /// 객체가 제네릭 풀에서 생성되거나 다시 가져올 때 호출됩니다.
        /// </summary>
        void OnCreateFromPool();

        /// <summary>
        /// 객체가 제네릭 풀로 반환될 때 호출됩니다.
        /// </summary>
        void OnReturnToPool();
    }

    /// <summary>
    /// 제네릭 풀에 속한 개별 객체의 상태와 반환 동작을 관리하는 정보 클래스입니다.
    /// </summary>
    public class PoolGenericInfo
    {
        /// <summary>
        /// 이 객체가 속한 제네릭 풀의 정보입니다.
        /// </summary>
        public GenericPoolInfo PoolInfo;

        /// <summary>
        /// 현재 객체가 활성 상태인지 여부입니다.
        /// </summary>
        public bool IsActive;

        /// <summary>
        /// 객체가 풀로 반환될 때 호출되는 이벤트입니다.
        /// </summary>
        public event System.Action OnReturn;

        /// <summary>
        /// 객체를 풀로 반환할 때 호출되어 관련 콜백을 실행합니다.
        /// </summary>
        public void Return()
        {
            OnReturn?.Invoke();
        }
    }
}