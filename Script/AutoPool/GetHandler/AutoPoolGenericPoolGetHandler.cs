using System;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolGenericPoolGetHandler
    {
        AutoPoolGetHandler _getHandler;
        AutoPool _autoPool;

        public AutoPoolGenericPoolGetHandler(AutoPoolGetHandler getHandler,AutoPool autoPool)
        {
            _autoPool = autoPool;
            _getHandler = getHandler;
        }

        public T Get<T>() where T : class, IPoolGeneric, new()
        {
            // 풀을 찾고
            GenericPoolInfo poolInfo = _autoPool.FindGenericPool<T>();

            // 프로세스 겟
            T instance = _getHandler.ProcessGenericGet<T>(poolInfo);

            return instance;
        }
    }
}