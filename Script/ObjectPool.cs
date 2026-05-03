using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 전역에서 접근 가능한 정적 객체 풀 진입점으로, Unity 객체 및 제네릭 풀에 대한 래핑 API를 제공합니다.
    /// </summary>
    public static class ObjectPool
    {
        /// <summary>
        /// 내부에서 사용하는 메인 풀 인스턴스입니다. 필요 시 자동으로 생성됩니다.
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
        /// 메인 풀 인스턴스 참조입니다.
        /// </summary>
        private static MainAutoPool s_objectPool;

        /// <summary>
        /// 메인 풀이 유효하게 존재하는지 여부입니다.
        /// </summary>
        public static bool HasPool => s_objectPool != null && !s_objectPool.Equals(null);

        /// <summary>
        /// 프리팹에 대한 풀 정보를 가져옵니다.
        /// </summary>
        public static IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        /// <summary>
        /// 컴포넌트 프리팹에 대한 풀 정보를 가져옵니다.
        /// </summary>
        public static IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.GetInfo(prefab);
        }

        /// <summary>
        /// 프리팹의 사전 생성 개수를 설정합니다.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>
        /// 컴포넌트 프리팹의 사전 생성 개수를 설정합니다.
        /// </summary>
        public static IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            CreatePool();
            return s_objectPool.SetPreload(prefab, count);
        }

        /// <summary>
        /// 프리팹과 연결된 풀을 비웁니다.
        /// </summary>
        public static IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>
        /// 컴포넌트 프리팹과 연결된 풀을 비웁니다.
        /// </summary>
        public static IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.ClearPool(prefab);
        }

        /// <summary>
        /// 프리팹 기반 GameObject를 풀에서 가져옵니다.
        /// </summary>
        public static GameObject Get(GameObject prefab)
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>
        /// 프리팹 기반 GameObject를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public static GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = default)
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }

        /// <summary>
        /// 프리팹 기반 GameObject를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public static GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        /// <summary>
        /// 컴포넌트 프리팹 인스턴스를 풀에서 가져옵니다.
        /// </summary>
        public static T Get<T>(T prefab) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab);
        }

        /// <summary>
        /// 컴포넌트 프리팹 인스턴스를 가져와 지정된 트랜스폼에 배치합니다.
        /// </summary>
        public static T Get<T>(T prefab, Transform transform, bool worldPositionStay = default) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, transform, worldPositionStay);
        }

        /// <summary>
        /// 컴포넌트 프리팹 인스턴스를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public static T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.Get(prefab, pos, rot);
        }

        /// <summary>
        /// Resources 경로 기반 컴포넌트 인스턴스를 가져와 위치와 회전을 설정합니다.
        /// </summary>
        public static T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component
        {
            CreatePool();
            return s_objectPool.ResourcesGet<T>(resouces, pos, rot);
        }

        /// <summary>
        /// 제네릭 풀에서 타입 <typeparamref name="T"/> 인스턴스를 가져옵니다.
        /// </summary>
        public static T GenericPool<T>() where T : class, IPoolGeneric, new()
        {
            CreatePool();
            return s_objectPool.GenericPool<T>();
        }

        /// <summary>
        /// GameObject 인스턴스를 풀에 반환하거나, 풀이 없으면 파괴합니다.
        /// </summary>
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

        /// <summary>
        /// 컴포넌트 인스턴스를 풀에 반환하거나, 풀이 없으면 파괴합니다.
        /// </summary>
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

        /// <summary>
        /// 지연 시간 후 GameObject 인스턴스를 풀에 반환하거나, 풀이 없으면 즉시 파괴합니다.
        /// </summary>
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

        /// <summary>
        /// 지연 시간 후 컴포넌트 인스턴스를 풀에 반환하거나, 풀이 없으면 즉시 파괴합니다.
        /// </summary>
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

        /// <summary>
        /// 제네릭 풀 인스턴스를 반환하고, 풀이 없으면 콜백만 호출합니다.
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
        /// 지연 시간 후 제네릭 풀 인스턴스를 반환하거나, 풀이 없으면 콜백만 호출합니다.
        /// </summary>
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

        /// <summary>
        /// 메인 풀 인스턴스를 생성합니다. 이미 존재하면 아무 동작도 하지 않습니다.
        /// </summary>
        private static void CreatePool()
        {
            if (s_objectPool == null)
            {
                s_objectPool = MainAutoPool.CreatePool();
            }
        }

        /// <summary>
        /// 씬 로드 전 런타임 초기화 시 풀 인스턴스를 초기 상태로 설정합니다.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void SetRunTime()
        {
            s_objectPool = null;
        }
    }
}