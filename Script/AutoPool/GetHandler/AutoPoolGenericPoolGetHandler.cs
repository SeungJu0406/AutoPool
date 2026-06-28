using System;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Handles Get requests for generic (non-GameObject) pools.
    /// </summary>
    public class AutoPoolGenericPoolGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        public AutoPoolGenericPoolGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _getHandler = getHandler;
        }

        /// <summary>
        /// Retrieves or creates an instance of <typeparamref name="T"/> from the generic pool.
        /// </summary>
        public T Get<T>() where T : class, IPoolGeneric, new()
        {
            GenericPoolInfo poolInfo = _autoPool.FindGenericPool<T>();
            T instance = _getHandler.ProcessGenericGet<T>(poolInfo);
            return instance;
        }
    }
}
