using System;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 제네릭 풀 타입에 대한 Get 요청을 처리하는 핸들러로,
    /// 메인 풀에서 제네릭 풀을 찾고 인스턴스를 생성/재사용합니다.
    /// </summary>
    public class AutoPoolGenericPoolGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        /// <summary>
        /// 제네릭 Get 처리를 위해 메인 풀 및 공통 Get 핸들러를 주입합니다.
        /// </summary>
        public AutoPoolGenericPoolGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _getHandler = getHandler;
        }

        /// <summary>
        /// 제네릭 풀에서 타입 <typeparamref name="T"/> 인스턴스를 가져옵니다.
        /// 풀이 없으면 자동으로 생성한 뒤 사용합니다.
        /// </summary>
        public T Get<T>() where T : class, IPoolGeneric, new()
        {
            GenericPoolInfo poolInfo = _autoPool.FindGenericPool<T>();      // 1) 타입 T 기준 제네릭 풀 찾기/생성
            T instance = _getHandler.ProcessGenericGet<T>(poolInfo);        // 2) 풀에서 인스턴스 가져오기
            return instance;                                                // 3) 결과 반환
        }
    }
}