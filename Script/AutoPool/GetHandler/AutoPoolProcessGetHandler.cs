using UnityEngine;
using UnityEngine.SceneManagement;

namespace AutoPool_Tool
{
    /// <summary>
    /// ���� Ǯ���� ��ü�� �����ų� ���� ������ ��ġ/ȸ��/�θ� �������� ó���ϴ� Get ���μ��� �ڵ鷯�Դϴ�.
    /// </summary>
    public class AutoPoolProcessGetHandler
    {
        AutoPoolGetHandler _getHandler;
        MainAutoPool _autoPool;

        /// <summary>
        /// ���� Ǯ�� ���� Get �ڵ鷯�� ���Թ޾� �ʱ�ȭ�մϴ�.
        /// </summary>
        public AutoPoolProcessGetHandler(AutoPoolGetHandler getHandler, MainAutoPool autoPool)
        {
            _getHandler = getHandler;
            _autoPool = autoPool;
        }

        /// <summary>
        /// ��ġ/ȸ�� �⺻��(Zero/Identity)���� GameObject�� �����ɴϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))                            // 1) Ǯ�� ��ȿ�� �ν��Ͻ��� �ִ��� �˻�
            {
                instance = info.Pool.Pop();                            // 2) ���ÿ��� �ϳ� ����

                poolObject = instance.GetComponent<PooledObject>();    // 3) PooledObject ����
                _autoPool.WakeUpRigidBody(poolObject);                 // 4) Rigidbody/2D �����

                instance.transform.position = Vector3.zero;            // 5) ��ġ �ʱ�ȭ
                instance.transform.rotation = Quaternion.identity;     // 6) ȸ�� �ʱ�ȭ
                instance.transform.localScale = info.Prefab.transform.localScale; // 7) ��ĸ�� �ʱ�ȭ
                instance.transform.SetParent(null);                    // 8) �θ� ���� (��Ʈ�� �̵�)
                instance.gameObject.SetActive(true);                   // 8) Ȱ��ȭ
                SceneManager.MoveGameObjectToScene(                    // 9) ���� Ȱ�� ������ �̵�
                    instance,
                    SceneManager.GetActiveScene());
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab);        // 1) Ǯ�� ������ �� �ν��Ͻ� ����
                poolObject = _autoPool.AddPoolObjectComponent(instance, info); // 2) Ǯ ������ ������Ʈ ����
            }

            poolObject.OnCreateFromPool();                             // 10) OnCreateFromPool �ݹ� ����
            SetActiveCount(info);                                      // 11) Ȱ��/Ǯ ī��Ʈ ����ȭ
            return instance;
        }

        /// <summary>
        /// ������ Ʈ�������� �������� GameObject�� �����ɴϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))                            // 1) Ǯ�� ��ȿ�� �ν��Ͻ��� �ִ��� �˻�
            {
                instance = info.Pool.Pop();                            // 2) ���ÿ��� �ϳ� ����
                poolObject = instance.GetComponent<PooledObject>();    // 3) PooledObject ����

                _autoPool.WakeUpRigidBody(poolObject);                 // 4) Rigidbody/2D �����
                instance.transform.SetParent(transform, worldPositionStay); // 5) worldPositionStay ����踦 Unity ǥ�� ���ǿ� ����
                if (!worldPositionStay)
                    instance.transform.localScale = info.Prefab.transform.localScale; // 6) ��ĸ�� �ʱ�ȭ

                instance.gameObject.SetActive(true);                   // 7) Ȱ��ȭ
            }
            else
            {
                instance = GameObject.Instantiate(                     // 1) Unity �⺻ Instantiate API ���
                    info.Prefab,
                    transform,
                    worldPositionStay);
                poolObject = _autoPool.AddPoolObjectComponent(instance, info); // 2) Ǯ ������ ������Ʈ ����
            }

            poolObject.OnCreateFromPool();                             // 8) OnCreateFromPool �ݹ� ����
            SetActiveCount(info);                                      // 9) Ȱ��/Ǯ ī��Ʈ ����ȭ
            return instance;
        }

        /// <summary>
        /// ������ ��ġ�� ȸ������ GameObject�� �����ɴϴ�.
        /// </summary>
        public GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot)
        {
            GameObject instance = null;
            PooledObject poolObject = null;

            if (_autoPool.FindObject(info))                            // 1) Ǯ�� ��ȿ�� �ν��Ͻ� �˻�
            {
                instance = info.Pool.Pop();                            // 2) ���ÿ��� �ϳ� ����
                poolObject = instance.GetComponent<PooledObject>();    // 3) PooledObject ����

                _autoPool.WakeUpRigidBody(poolObject);                 // 4) Rigidbody/2D �����
                instance.transform.position = pos;                     // 5) ��ġ ����
                instance.transform.rotation = rot;     
                instance.transform.localScale = info.Prefab.transform.localScale;                // 6) ȸ�� ����
                instance.transform.SetParent(null);                    // 7) �θ� ����
                instance.gameObject.SetActive(true);                   // 8) Ȱ��ȭ
                SceneManager.MoveGameObjectToScene(                    // 9) ���� Ȱ�� ������ �̵�
                    instance,
                    SceneManager.GetActiveScene());
            }
            else
            {
                instance = GameObject.Instantiate(info.Prefab, pos, rot);        // 1) �� �ν��Ͻ� ����
                poolObject = _autoPool.AddPoolObjectComponent(instance, info);   // 2) Ǯ ������ ������Ʈ ����
            }

            poolObject.OnCreateFromPool();                             // 10) OnCreateFromPool �ݹ� ����
            SetActiveCount(info);                                      // 11) Ȱ��/Ǯ ī��Ʈ ����ȭ
            return instance;
        }

        /// <summary>
        /// ���׸� Ǯ���� Ÿ�� <typeparamref name="T"/> �ν��Ͻ��� �������ų� ���� �����մϴ�.
        /// </summary>
        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new()
        {
            T instance = null;
            IPoolGeneric poolGeneric = null;

            if (_autoPool.FindGeneric<T>(poolInfo))                    // 1) ���׸� Ǯ�� ��ȿ�� �ν��Ͻ� �ִ��� �˻�
            {
                poolGeneric = poolInfo.Pool.Pop();                     // 2) ���ÿ��� ������
                instance = (T)poolGeneric;                             // 3) ��ü Ÿ������ ĳ����
            }
            else
            {
                instance = new T();                                    // 1) �� �ν��Ͻ� ����
                poolGeneric = (IPoolGeneric)instance;                  // 2) �������̽��� ĳ����
                poolGeneric.Pool = new PoolGenericInfo();              // 3) ���� ��ü�� PoolGenericInfo ����
                poolGeneric.Pool.PoolInfo = poolInfo;                  // 4) ���� GenericPoolInfo ����
                poolInfo.PoolCount++;                                  // 5) Ǯ�� ���� �� ���� ����
                poolInfo.OnPoolDormant += poolGeneric.OnReturnToPool;  // 6) Ǯ �޸� �� ��ü ��ȯ �ݹ� ����
            }

            SetActiveCount(poolInfo);                                  // 7) Ȱ��/Ǯ ī��Ʈ ����ȭ
            poolGeneric.Pool.IsActive = true;                          // 8) ���� ��ü Ȱ�� �÷��� ����
            poolGeneric.OnCreateFromPool();                            // 9) OnCreateFromPool �ݹ� ����
            return instance;
        }

        /// <summary>
        /// GameObject Ǯ�� Ȱ�� ������ ������Ű�� �ִ� ������ �����մϴ�.
        /// </summary>
        private void SetActiveCount(PoolInfo info)
        {
            info.ActiveCount++;
            if (info.PoolCount < info.ActiveCount)
            {
                info.PoolCount = info.ActiveCount;
            }
        }

        /// <summary>
        /// ���׸� Ǯ�� Ȱ�� ������ ������Ű�� �ִ� ������ �����մϴ�.
        /// </summary>
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