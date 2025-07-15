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
        /// ������Ʈ�� Ǯ���� �������� ���� ó�� �����Դϴ�. ������Ʈ�� ���������� �����ϰ�, ������ ���� �����մϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info)
        {
            GameObject instance = null;
            PooledObject poolObject = null;
            if (_autoPool.FindObject(info))
            {
                // ���� Ǯ���� ����
                instance = info.Pool.Pop();

                poolObject = instance.GetComponent<PooledObject>();
                // Rigidbody �ʱ�ȭ
                _autoPool.WakeUpRigidBody(poolObject);

                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.SetParent(null);
                instance.gameObject.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
            }
            else
            {
                // ���� ����
                instance = GameObject.Instantiate(info.Prefab);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }
            poolObject.OnCreateFromPool();
            info.ActiveCount++;
            return instance;
        }
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ Transform�� ��ġ��Ű��, ���� �������� �������� ���θ� �����մϴ�. �ش� �����տ� ���� Ǯ ������ ã��, Ȱ��ȭ�� ������Ʈ�� ������ ��ȯ�ϰ�, ������ ���� �����մϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false)
        {
            GameObject instance = null;
            PooledObject poolObject = null;
            if (_autoPool.FindObject(info))
            {
                // ���� Ǯ���� ����
                instance = info.Pool.Pop();
                poolObject = instance.GetComponent<PooledObject>();
                // Rigidbody �ʱ�ȭ
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
                // ���� ����
                instance = GameObject.Instantiate(info.Prefab, transform, worldPositionStay);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);
            }
            poolObject.OnCreateFromPool();
            info.ActiveCount++;
            return instance;
        }
        /// <summary>
        /// Ǯ���� ������Ʈ�� ��������, ������ ��ġ�� ȸ���� �����մϴ�. �ش� �����տ� ���� Ǯ ������ ã��, Ȱ��ȭ�� ������Ʈ�� ������ ��ȯ�ϰ�, ������ ���� �����մϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot)
        {
            GameObject instance = null;

            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))
            {
                // ���� Ǯ���� ����
                instance = info.Pool.Pop();
                poolObject = instance.GetComponent<PooledObject>();
                // Rigidbody �ʱ�ȭ
                _autoPool.WakeUpRigidBody(poolObject);
                instance.transform.position = pos;
                instance.transform.rotation = rot;
                instance.transform.SetParent(null);
                instance.gameObject.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());

            }
            else
            {
                // ���� ����
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

                // Ǯ ������ �װ� �־�ߵ�
                Debug.Log($"Generic Pool Create : {instance}");
            }
            poolGeneric.OnCreateFromPool();
            poolInfo.ActiveCount++;
            poolGeneric.Pool.IsActive = true;
            return instance;
        }
    }
}