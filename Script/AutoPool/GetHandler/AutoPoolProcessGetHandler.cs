using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoPool_Tool
{
    /// <summary>
    /// Low-level handler that pops an instance from the pool stack (or instantiates a new one),
    /// then applies position, rotation, parent, and activation.
    /// </summary>
    public class AutoPoolProcessGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        public AutoPoolProcessGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        /// <summary>
        /// Pops a pooled instance and activates it at world origin, or instantiates a new one
        /// if the pool is empty.
        /// </summary>
        public PooledObject ProcessGet(PoolInfo info)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))
            {
                poolObject = info.Pool.Pop();
                instance = poolObject.gameObject;
                _autoPool.WakeUpRigidBody(poolObject);

                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.localScale = info.Prefab.transform.localScale;
                instance.transform.SetParent(null);
                instance.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }

            poolObject.OnCreateFromPool();
            SetActiveCount(info);
            return poolObject;
        }

        /// <summary>
        /// Pops a pooled instance and places it under <paramref name="transform"/>,
        /// or instantiates a new one via Unity's Instantiate overload.
        /// </summary>
        public PooledObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))
            {
                poolObject = info.Pool.Pop();
                instance = poolObject.gameObject;
                _autoPool.WakeUpRigidBody(poolObject);

                instance.transform.SetParent(transform, worldPositionStay);
                if (!worldPositionStay)
                    instance.transform.localScale = info.Prefab.transform.localScale;

                instance.SetActive(true);
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab, transform, worldPositionStay);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }

            poolObject.OnCreateFromPool();
            SetActiveCount(info);
            return poolObject;
        }

        /// <summary>
        /// Pops a pooled instance and places it at the given position and rotation,
        /// or instantiates a new one.
        /// </summary>
        public PooledObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))
            {
                poolObject = info.Pool.Pop();
                instance = poolObject.gameObject;
                _autoPool.WakeUpRigidBody(poolObject);

                instance.transform.position = pos;
                instance.transform.rotation = rot;
                instance.transform.localScale = info.Prefab.transform.localScale;
                instance.transform.SetParent(null);
                instance.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab, pos, rot);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }

            poolObject.OnCreateFromPool();
            SetActiveCount(info);
            return poolObject;
        }

        /// <summary>
        /// Pops or creates a generic instance of type <typeparamref name="T"/>
        /// and marks it active in the pool.
        /// </summary>
        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new()
        {
            T instance = null;
            IPoolGeneric poolGeneric = null;

            if (_autoPool.FindGeneric<T>(poolInfo))
            {
                poolGeneric = poolInfo.Pool.Pop();
                instance = (T)poolGeneric;
            }
            else
            {
                instance = new T();
                poolGeneric = (IPoolGeneric)instance;
                poolGeneric.Pool = new PoolGenericInfo();
                poolGeneric.Pool.PoolInfo = poolInfo;
                poolInfo.PoolCount++;
                poolInfo.OnPoolDormant += poolGeneric.OnReturnToPool;
            }

            SetActiveCount(poolInfo);
            poolGeneric.Pool.IsActive = true;
            poolGeneric.OnCreateFromPool();
            return instance;
        }

        private void SetActiveCount(PoolInfo info)
        {
            info.ActiveCount++;
            if (info.PoolCount < info.ActiveCount)
                info.PoolCount = info.ActiveCount;
        }

        private void SetActiveCount(GenericPoolInfo info)
        {
            info.ActiveCount++;
            if (info.PoolCount < info.ActiveCount)
                info.PoolCount = info.ActiveCount;
        }
    }
}
