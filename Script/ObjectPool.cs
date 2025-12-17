using UnityEngine;

namespace AutoPool_Tool
{

    public static class ObjectPool
    {
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
        private static MainAutoPool s_objectPool;
        public static bool HasPool => s_objectPool != null && !s_objectPool.Equals(null);
        public static IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }
        public static IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        public static IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        public static IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        public static IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        public static IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        public static GameObject Get(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        public static GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = default)
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }
        public static GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        public static T Get<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        public static T Get<T>(T prefab, Transform transform, bool worldPositionStay = default) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }
        public static T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        public static T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(resouces, pos, rot);
        }

        public static T GenericPool<T>() where T : class, IPoolGeneric, new()
        {
            CreatePool();
            return s_objectPool.GenericPool<T>();
        }
        public static IPoolInfoReadOnly Return(GameObject instance)
        {
            // 1. 풀이 살아있다면 정상 반납
            if (HasPool)
            {
                return s_objectPool.Return(instance);
            }

            // 2. [중요] 풀이 죽었는데 반납 요청이 옴 (Additive 씬 잔존물 등)
            // 재사용이 불가능하므로 과감하게 파괴하여 화면에서 치워버림
            if (instance != null)
            {
                GameObject.Destroy(instance);
            }

            return null;
        }
        public static IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            // 1. 풀이 살아있다면 정상 반납
            if (HasPool)
            {
                return s_objectPool.Return(instance);
            }

            // 2. [중요] 풀이 죽었는데 반납 요청이 옴 (Additive 씬 잔존물 등)
            // 재사용이 불가능하므로 과감하게 파괴하여 화면에서 치워버림
            if (instance != null)
            {
                GameObject.Destroy(instance);
            }

            return null;
        }

        public static void Return(GameObject instance, float delay)
        {
            if (HasPool)
            {
                s_objectPool.Return(instance, delay);
            }
            else
            {
                // 지연 반납의 경우, 코루틴을 돌릴 풀이 없으므로
                // 그냥 딜레이 없이 즉시 파괴하거나, 필요하다면 별도 처리가 필요하지만
                // 보통 풀이 꺼진 상황이면 즉시 정리가 맞습니다.
                if (instance != null) GameObject.Destroy(instance);
            }
        }

        public static void Return<T>(T instance, float delay) where T : Component
        {
            if (HasPool)
            {
                s_objectPool.Return(instance, delay);
            }
            else
            {
                // 지연 반납의 경우, 코루틴을 돌릴 풀이 없으므로
                // 그냥 딜레이 없이 즉시 파괴하거나, 필요하다면 별도 처리가 필요하지만
                // 보통 풀이 꺼진 상황이면 즉시 정리가 맞습니다.
                if (instance != null) GameObject.Destroy(instance);
            }
        }

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
        public static void ReturnGeneric<T>(T instance, float delay) where T : class, IPoolGeneric, new()
        {
            if (HasPool == true)
            {
                s_objectPool.GenericReturn(instance);
                return;
            }
            if (instance != null)
            {
                instance.OnReturnToPool();
            }
        }
        private static void CreatePool()
        {
            if (s_objectPool == null)
            {
                s_objectPool = MainAutoPool.CreatePool();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetRunTime()
        {
            s_objectPool = null;
        }
    }
}