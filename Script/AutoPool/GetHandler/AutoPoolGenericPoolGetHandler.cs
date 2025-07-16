using System;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolGenericPoolGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        public AutoPoolGenericPoolGetHandler(AutoPoolGetHandler getHandler,MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _getHandler = getHandler;
        }

        public T Get<T>() where T : class, IPoolGeneric, new()
        {
            // Ǯ�� ã��
            GenericPoolInfo poolInfo = _autoPool.FindGenericPool<T>();

            // ���μ��� ��
            T instance = _getHandler.ProcessGenericGet<T>(poolInfo);

            return instance;
        }
    }
}