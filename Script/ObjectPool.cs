using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// �������� ���� ������ ���� ��ü Ǯ ����������, Unity ��ü �� ���׸� Ǯ�� ���� ���� API�� �����մϴ�.
    /// </summary>
    public static class ObjectPool
    {
        /// <summary>
        /// ���ο��� ����ϴ� ���� Ǯ �ν��Ͻ��Դϴ�. �ʿ� �� �ڵ����� �����˴ϴ�.
        /// </summary>
        public static MainAutoPool Instance
        {
            get
            {
                if (s_objectPool == null)
                {
                    CreatePool();
                }
                return s_objectPool;
            }
        }

        /// <summary>
        /// ���� Ǯ �ν��Ͻ� �����Դϴ�.
        /// </summary>
        private static MainAutoPool s_objectPool;

        /// <summary>
        /// ���� Ǯ�� ��ȿ�ϰ� �����ϴ��� �����Դϴ�.
        /// </summary>
        public static bool HasPool => s_objectPool != null && !s_objectPool.Equals(null);

        /// <summary>
        /// �����տ� ���� Ǯ ������ �����ɴϴ�.
        /// </summary>
        public static IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        /// <summary>
        /// ������Ʈ �����տ� ���� Ǯ ������ �����ɴϴ�.
        /// </summary>
        public static IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        /// <summary>
        /// �������� ���� ���� ������ �����մϴ�.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>
        /// ������Ʈ �������� ���� ���� ������ �����մϴ�.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>
        /// �����հ� ����� Ǯ�� ���ϴ�.
        /// </summary>
        public static IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>
        /// ������Ʈ �����հ� ����� Ǯ�� ���ϴ�.
        /// </summary>
        public static IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>
        /// ������ ��� GameObject�� Ǯ���� �����ɴϴ�.
        /// </summary>
        public static GameObject Get(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>
        /// ������ ��� GameObject�� ������ ������ Ʈ�������� ��ġ�մϴ�.
        /// </summary>
        public static GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = default)
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }

        /// <summary>
        /// ������ ��� GameObject�� ������ ��ġ�� ȸ���� �����մϴ�.
        /// </summary>
        public static GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        /// <summary>
        /// ������Ʈ ������ �ν��Ͻ��� Ǯ���� �����ɴϴ�.
        /// </summary>
        public static T Get<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>
        /// ������Ʈ ������ �ν��Ͻ��� ������ ������ Ʈ�������� ��ġ�մϴ�.
        /// </summary>
        public static T Get<T>(T prefab, Transform transform, bool worldPositionStay = default) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }

        /// <summary>
        /// ������Ʈ ������ �ν��Ͻ��� ������ ��ġ�� ȸ���� �����մϴ�.
        /// </summary>
        public static T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        /// <summary>
        /// Resources ��� ��� ������Ʈ �ν��Ͻ��� ������ ��ġ�� ȸ���� �����մϴ�.
        /// </summary>
        public static T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(resouces, pos, rot);
        }

        /// <summary>
        /// ���׸� Ǯ���� Ÿ�� <typeparamref name="T"/> �ν��Ͻ��� �����ɴϴ�.
        /// </summary>
        public static T GenericPool<T>() where T : class, IPoolGeneric, new()
        {
            CreatePool();
            return s_objectPool.GenericPool<T>();
        }

        /// <summary>
        /// GameObject �ν��Ͻ��� Ǯ�� ��ȯ�ϰų�, Ǯ�� ������ �ı��մϴ�.
        /// </summary>
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            // 1. Ǯ�� ����ִٸ� ���� �ݳ�
            if (HasPool)
            {
                return s_objectPool.Return(instance);
            }

            // 2. [�߿�] Ǯ�� �׾��µ� �ݳ� ��û�� �� (Additive �� ������ ��)
            // ������ �Ұ����ϹǷ� �����ϰ� �ı��Ͽ� ȭ�鿡�� ġ������
            if (instance != null)
            {
                GameObject.Destroy(instance);
            }

            return null;
        }

        /// <summary>
        /// ������Ʈ �ν��Ͻ��� Ǯ�� ��ȯ�ϰų�, Ǯ�� ������ �ı��մϴ�.
        /// </summary>
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            // 1. Ǯ�� ����ִٸ� ���� �ݳ�
            if (HasPool)
            {
                return s_objectPool.Return(instance);
            }

            // 2. [�߿�] Ǯ�� �׾��µ� �ݳ� ��û�� �� (Additive �� ������ ��)
            // ������ �Ұ����ϹǷ� �����ϰ� �ı��Ͽ� ȭ�鿡�� ġ������
            if (instance != null)
            {
                GameObject.Destroy(instance);
            }

            return null;
        }

        /// <summary>
        /// ���� �ð� �� GameObject �ν��Ͻ��� Ǯ�� ��ȯ�ϰų�, Ǯ�� ������ ��� �ı��մϴ�.
        /// </summary>
        public static void Return(GameObject instance, float delay)
        {
            if (HasPool)
            {
                s_objectPool.Return(instance, delay);
            }
            else
            {
                // ���� �ݳ��� ���, �ڷ�ƾ�� ���� Ǯ�� �����Ƿ�
                // �׳� ������ ���� ��� �ı��ϰų�, �ʿ��ϴٸ� ���� ó���� �ʿ�������
                // ���� Ǯ�� ���� ��Ȳ�̸� ��� ������ �½��ϴ�.
                if (instance != null) GameObject.Destroy(instance);
            }
        }

        /// <summary>
        /// ���� �ð� �� ������Ʈ �ν��Ͻ��� Ǯ�� ��ȯ�ϰų�, Ǯ�� ������ ��� �ı��մϴ�.
        /// </summary>
        public static void Return<T>(T instance, float delay) where T : Component
        {
            if (HasPool)
            {
                s_objectPool.Return(instance, delay);
            }
            else
            {
                // ���� �ݳ��� ���, �ڷ�ƾ�� ���� Ǯ�� �����Ƿ�
                // �׳� ������ ���� ��� �ı��ϰų�, �ʿ��ϴٸ� ���� ó���� �ʿ�������
                // ���� Ǯ�� ���� ��Ȳ�̸� ��� ������ �½��ϴ�.
                if (instance != null) GameObject.Destroy(instance);
            }
        }

        /// <summary>
        /// ���׸� Ǯ �ν��Ͻ��� ��ȯ�ϰ�, Ǯ�� ������ �ݹ鸸 ȣ���մϴ�.
        /// </summary>
        public static IGenericPoolInfoReadOnly ReturnGeneric<T>(T instance) where T : class, IPoolGeneric, new()
        {
            if (HasPool == true)
            {
                return s_objectPool.GenericReturn(instance);
            }

            if (instance != null)
            {
                instance.OnReturnToPool();
            }
            return null;
        }

        /// <summary>
        /// ���� �ð� �� ���׸� Ǯ �ν��Ͻ��� ��ȯ�ϰų�, Ǯ�� ������ �ݹ鸸 ȣ���մϴ�.
        /// </summary>
        public static void ReturnGeneric<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            if (HasPool == true)
            {
                s_objectPool.GenericReturn(instance, delay);
                return;
            }
            if (instance != null)
            {
                instance.OnReturnToPool();
            }
        }

        /// <summary>
        /// ���� Ǯ �ν��Ͻ��� �����մϴ�. �̹� �����ϸ� �ƹ� ���۵� ���� �ʽ��ϴ�.
        /// </summary>
        private static void CreatePool()
        {
            if (s_objectPool == null)
            {
                s_objectPool = MainAutoPool.CreatePool();
            }
        }

        /// <summary>
        /// �� �ε� �� ��Ÿ�� �ʱ�ȭ �� Ǯ �ν��Ͻ��� �ʱ� ���·� �����մϴ�.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetRunTime()
        {
            s_objectPool = null;
        }
    }
}