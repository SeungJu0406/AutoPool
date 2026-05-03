using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoPool_Tool
{
    public class AutoPoolProcessGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        public AutoPoolProcessGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        public GameObject ProcessGet(PoolInfo info)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))
            {
                instance = info.Pool.Pop();
                poolObject = instance.GetComponent<PooledObject>();
                _autoPool.WakeUpRigidBody(poolObject);

                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.SetParent(null);
                instance.gameObject.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }

            poolObject.OnCreateFromPool();
            SetActiveCount(info);
            return instance;
        }

        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))
            {
                instance = info.Pool.Pop();
                poolObject = instance.GetComponent<PooledObject>();
                _autoPool.WakeUpRigidBody(poolObject);
                instance.transform.SetParent(transform, worldPositionStay);
                instance.gameObject.SetActive(true);
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab, transform, worldPositionStay);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }

            poolObject.OnCreateFromPool();
            SetActiveCount(info);
            return instance;
        }

        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))
            {
                instance = info.Pool.Pop();
                poolObject = instance.GetComponent<PooledObject>();
                _autoPool.WakeUpRigidBody(poolObject);
                instance.transform.position = pos;
                instance.transform.rotation = rot;
                instance.transform.SetParent(null);
                instance.gameObject.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab, pos, rot);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }

            poolObject.OnCreateFromPool();
            SetActiveCount(info);
            return instance;
        }

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
            {
                info.PoolCount = info.ActiveCount;
            }
        }

        private void SetActiveCount(GenericPoolInfo info)
        {
            info.ActiveCount++;
            if (info.PoolCount < info.ActiveCount)
            {
                info.PoolCount = info.ActiveCount;
            }
        }
    }
}
