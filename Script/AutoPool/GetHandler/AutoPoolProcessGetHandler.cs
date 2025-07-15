using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoPool
{
    public class AutoPoolProcessGetHandler
    {
        AutoPoolGetHandler _getHandler;
        AutoPool _autoPool;

        public AutoPoolProcessGetHandler(AutoPoolGetHandler getHandler, AutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        /// <summary>
        /// 오브젝트를 풀에서 가져오는 실제 처리 로직입니다. 오브젝트가 남아있으면 재사용하고, 없으면 새로 생성합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info)
        {
            GameObject instance = null;
            PooledObject poolObject = null;
            if (_autoPool.FindObject(info))
            {
                // 기존 풀에서 꺼냄
                instance = info.Pool.Pop();

                poolObject = instance.GetComponent<PooledObject>();
                // Rigidbody 초기화
                _autoPool.WakeUpRigidBody(poolObject);

                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.SetParent(null);
                instance.gameObject.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
            }
            else
            {
                // 새로 생성
                instance = GameObject.Instantiate(info.Prefab);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }
            poolObject.OnCreateFromPool();
            info.ActiveCount++;
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다. 해당 프리팹에 대한 풀 정보를 찾고, 활성화된 오브젝트가 있으면 반환하고, 없으면 새로 생성합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false)
        {
            GameObject instance = null;
            PooledObject poolObject = null;
            if (_autoPool.FindObject(info))
            {
                // 기존 풀에서 꺼냄
                instance = info.Pool.Pop();
                poolObject = instance.GetComponent<PooledObject>();
                // Rigidbody 초기화
                _autoPool.WakeUpRigidBody(poolObject);
                instance.transform.SetParent(transform);
                if (worldPositionStay == true)
                {
                    instance.transform.position = info.Prefab.transform.position;
                    instance.transform.rotation = info.Prefab.transform.rotation;
                }
                else
                {
                    instance.transform.position = transform.position;
                    instance.transform.rotation = transform.rotation;
                }
                instance.gameObject.SetActive(true);
            }
            else
            {
                // 새로 생성
                instance = GameObject.Instantiate(info.Prefab, transform, worldPositionStay);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }
            poolObject.OnCreateFromPool();
            info.ActiveCount++;
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다. 해당 프리팹에 대한 풀 정보를 찾고, 활성화된 오브젝트가 있으면 반환하고, 없으면 새로 생성합니다.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot)
        {
            GameObject instance = null;

            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))
            {
                // 기존 풀에서 꺼냄
                instance = info.Pool.Pop();
                poolObject = instance.GetComponent<PooledObject>();
                // Rigidbody 초기화
                _autoPool.WakeUpRigidBody(poolObject);
                instance.transform.position = pos;
                instance.transform.rotation = rot;
                instance.transform.SetParent(null);
                instance.gameObject.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());

            }
            else
            {
                // 새로 생성
                instance = GameObject.Instantiate(info.Prefab, pos, rot);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }
            poolObject.OnCreateFromPool();
            info.ActiveCount++;
            return instance;
        }

        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new()
        {
            T instance = null;
            IPoolGeneric poolGeneric = null;
            if(_autoPool.FindGeneric<T>(poolInfo))
            {
                poolGeneric = poolInfo.Pool.Pop();
                instance = (T)poolGeneric;

                Debug.Log($"Generic Pool Get : {typeof(T)}");
            }
            else
            {
                instance = new T();
                poolGeneric = (IPoolGeneric)instance;
                poolGeneric.Pool = new PoolGenericInfo();
                poolGeneric.Pool.PoolInfo = poolInfo;
                poolInfo.PoolCount++;

                // 풀 라이프 그거 넣어야됨
                Debug.Log($"Generic Pool Create : {instance}");
            }
            poolGeneric.OnCreateFromPool();
            poolInfo.ActiveCount++;
            poolGeneric.Pool.IsActive = true;
            return instance;
        }
    }
}