using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 제네릭 객체 풀에 대한 간단한 접근을 제공하는 정적 클래스입니다.
    /// </summary>
    public static class GenericPool
    {
        /// <summary>
        /// 제네릭 풀에서 타입 <typeparamref name="T"/> 인스턴스를 가져옵니다.
        /// </summary>
        public static T Get<T>() where T : class, IPoolGeneric, new()
        {
            return ObjectPool.GenericPool<T>();
        }

        /// <summary>
        /// 인스턴스를 제네릭 풀에 반환하고 풀 정보를 반환합니다.
        /// </summary>
        public static IGenericPoolInfoReadOnly Return<T>(T instance) where T : class, IPoolGeneric, new()
        {
            return ObjectPool.ReturnGeneric(instance);
        }
    }
}