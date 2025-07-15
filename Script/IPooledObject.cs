using UnityEngine;

namespace AutoPool
{

    /// <summary>
    /// IPooledObject 인터페이스는 풀링된 오브젝트가 초기화될 때 호출되는 메서드를 정의합니다.
    /// </summary>
    public interface IPooledObject
    {
        void OnCreateFromPool();

        void OnReturnToPool();
    }

    public interface  IPoolGeneric
    {
        PoolGenericInfo Pool { get; set; }

        void OnCreateFromPool();

        void OnReturnToPool();
    }

    public class PoolGenericInfo
    {
        public GenericPoolInfo PoolInfo;
        public bool IsActive;
        public event System.Action OnReturn;

        public void Return()
        {
            OnReturn?.Invoke();
        }
    }
}