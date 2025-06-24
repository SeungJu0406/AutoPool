using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace NSJ_EasyPoolKit
{
    // This script is part of a Unity Asset Store package.
    // Unauthorized copying, modification, or redistribution of this code is strictly prohibited.
    // © 2025 NSJ. All rights reserved.

    /// <summary>
    /// 오브젝트 풀링을 위한 메인 클래스입니다.
    /// 자동 반환, 딜레이 반환, 재사용 가능한 풀 구조를 제공합니다.
    /// </summary>
    public class EasyObjectPool : MonoBehaviour, IObjectPool
    {
        /// <summary>
        /// 풀 오브젝트 최대 유지 시간입니다. 시간이 지나면 자동 비활성화됩니다.
        /// </summary>

        [HideInInspector] public float MaxTimer = 600f;

        private static bool s_isApplicationQuit = false;

        private static EasyObjectPool _instance;
        /// <summary>
        /// EasyObjectPool 싱글톤 인스턴스입니다. 자동 생성됩니다.
        /// </summary>
        public static EasyObjectPool Instance
        {
            get
            {
                return CreatePool();
            }
            set
            {
                _instance = value;
            }
        }

        public static EasyObjectPool CreatePool()
        {
            if (_instance != null)
            {
                return _instance;
            }
            else
            {
                if (s_isApplicationQuit == true)
                    return null;
                // 새로운 EasyObjectPool GameObject 생성 및 할당
                GameObject newPool = new GameObject("EasyObjectPool");
                EasyObjectPool pool = newPool.AddComponent<EasyObjectPool>();
                return pool;
            }
        }
        /// <summary>
        /// 풀 정보 클래스입니다. 각 프리팹에 대한 풀 상태 및 정보를 저장합니다.
        /// 외부에서는 IPoolInfoReadOnly 인터페이스를 통해 읽기 전용으로 접근합니다.
        /// </summary>
        public class PoolInfo : IPoolInfoReadOnly
        {
            public bool IsMock = false;
            public Stack<GameObject> Pool;
            public GameObject Prefab;
            public Transform Parent;
            public bool IsActive;
            public bool IsUsed = true;
            public UnityAction OnPoolDormant;
            public int PoolCount;
            public int ActiveCount;

            bool IPoolInfoReadOnly.IsMock => IsMock;
            Stack<GameObject> IPoolInfoReadOnly.Pool => Pool;
            GameObject IPoolInfoReadOnly.Prefab => Prefab;
            string IPoolInfoReadOnly.Name => Prefab.name;
            Transform IPoolInfoReadOnly.Parent => Parent;
            bool IPoolInfoReadOnly.IsActive => IsActive;
            bool IPoolInfoReadOnly.IsUsed => IsUsed;

            UnityAction IPoolInfoReadOnly.OnPoolDormant { get => OnPoolDormant; set => OnPoolDormant = value; }

            int IPoolInfoReadOnly.PoolCount => PoolCount;
            int IPoolInfoReadOnly.ActiveCount => ActiveCount;


        }

        public class CoroutineRef
        {
            public Coroutine coroutine;
        }

        /// <summary>
        /// 프리팹 ID를 기준으로 풀 정보를 저장하는 딕셔너리입니다.
        /// </summary>
        private Dictionary<int, PoolInfo> _poolDic = new Dictionary<int, PoolInfo>();

        /// <summary>
        /// Resources에 저장된 프리팹을 기준으로 풀 정보를 저장하는 딕셔너리 입니다
        /// </summary>
        private Dictionary<string, int> _resourcesPoolDic = new Dictionary<string, int>();
        /// <summary>
        /// 동일한 시간 값에 대해 WaitForSeconds 인스턴스를 재사용하기 위한 캐시입니다.
        /// </summary>
        private Dictionary<float, WaitForSeconds> _delayDic = new Dictionary<float, WaitForSeconds>();

#if UNITY_EDITOR
        public List<IPoolInfoReadOnly> GetAllPoolInfos()
        {
            return _poolDic.Values.Cast<IPoolInfoReadOnly>().ToList();
        }
#endif
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        #region SePreload
        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            PoolInfo info = FindPool(prefab);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. 컴포넌트를 타입으로 지정할 수 있습니다.
        /// Sets the preload count for a specific prefab in the pool.
        /// </summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            PoolInfo info = FindPool(prefab.gameObject);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성합니다. Resources에 저장된 프리팹을 기준으로 합니다.
        /// Sets the preload count for a specific prefab in the pool using a Resources path.
        /// </summary>
        public IPoolInfoReadOnly SetPreload(string resources, int count)
        {
            PoolInfo info = FindResourcePool(resources);
            return ProcessPreload(info, count);
        }
        /// <summary>
        /// 풀을 미리 정의된 개수만큼 생성하는 과정의 메서드입니다
        /// This method processes the preload operation for a given PoolInfo.
        /// </summary>
        private IPoolInfoReadOnly ProcessPreload(PoolInfo info, int count)
        {
            if(info == null)
            {
                Debug.LogError("풀 정보가 유효하지 않습니다.");
                return null;
            }
            // count 수치까지 미리 오브젝트를 생성하고 풀에 추가합니다.
            while (info.PoolCount < count)
            {
                Debug.Log(count);
                GameObject instance = Instantiate(info.Prefab);
                PooledObject poolObject = AddPoolObjectComponent(instance, info);
                instance.transform.SetParent(info.Parent);
                info.Pool.Push(instance);
                instance.gameObject.SetActive(false);
            }
            return info;
        }
        #endregion
        #region GetPool
        #region Common
        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다.
        /// </summary>
        public GameObject Get(GameObject prefab)
        {
            PoolInfo info = FindPool(prefab);
            GameObject instance = ProcessGet(info);
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = FindPool(prefab);
            GameObject instance = ProcessGet(info, transform, worldPositionStay);
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = FindPool(prefab);
            GameObject instance = ProcessGet(info, pos, rot);
            return instance;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T Get<T>(T prefab) where T : Component
        {
            PoolInfo info = FindPool(prefab.gameObject);
            GameObject instance = ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = FindPool(prefab.gameObject);
            GameObject instance = ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = FindPool(prefab.gameObject);
            GameObject instance = ProcessGet(info, pos, rot);
            T component = instance.GetComponent<T>();
            return component;
        }
        #endregion
        #region Resources
        /// <summary>
        /// 풀에서 오브젝트를 가져옵니다.(Resources)
        /// </summary>
        /// <param name="resources"></param>
        /// <returns></returns>
        public GameObject ResourcesGet(string resources)
        {
            PoolInfo info = FindResourcePool(resources);
            GameObject instance = ProcessGet(info);
            return instance;
        }
        /// <summary>
        ///  풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false)
        {
            PoolInfo info = FindResourcePool(resources);
            GameObject instance = ProcessGet(info, transform, worldPositionStay);
            return instance;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다.(Resources)
        /// </summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot)
        {
            PoolInfo info = FindResourcePool(resources);
            GameObject instance = ProcessGet(info, pos, rot);
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 반환합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources) where T : Component
        {
            PoolInfo info = FindResourcePool(resources);
            GameObject instance = ProcessGet(info);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component
        {
            PoolInfo info = FindResourcePool(resources);
            GameObject instance = ProcessGet(info, transform, worldPositionStay);
            T component = instance.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 해당 컴포넌트를 지정된 위치와 회전을 설정합니다.
        /// </summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component
        {
            PoolInfo info = FindResourcePool(resources);
            GameObject instance = ProcessGet(info, pos, rot);
            T component = instance.GetComponent<T>();
            return component;
        }
        #endregion

        #endregion
        #region ReturnPool
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 풀에 다시 추가됩니다.
        /// </summary>
        public IPoolInfoReadOnly Return(GameObject instance)
        {
            return ProcessReturn(instance.gameObject);
        }
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 풀에 다시 추가됩니다.
        /// </summary>
        public IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            return ProcessReturn(instance.gameObject);
        }
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 지정된 지연 시간 후에 풀에 다시 추가됩니다.
        /// </summary>
        public void Return(GameObject instance, float delay)
        {
            if (instance == null)
                return;
            if (instance.activeSelf == false)
                return;

            PooledObject pooledObject = instance.GetComponent<PooledObject>();

            CoroutineRef coroutineRef = new CoroutineRef();
            coroutineRef.coroutine = Instance.StartCoroutine(Instance.ReturnRoutine(instance, delay, coroutineRef));
            System.Action callback = null;
            callback = () =>
            {
                if (coroutineRef.coroutine != null)
                {
                    Instance.StopCoroutine(coroutineRef.coroutine);
                    coroutineRef.coroutine = null;
                }
                pooledObject.OnReturn -= callback;
            };
            pooledObject.OnReturn += callback;
        }
        /// <summary>
        /// 풀에서 오브젝트를 반환합니다. 반환된 오브젝트는 비활성화되고, 지정된 지연 시간 후에 풀에 다시 추가됩니다.
        /// </summary>
        public void Return<T>(T instance, float delay) where T : Component
        {
            Return(instance.gameObject, delay);
        }
        /// <summary>
        /// 풀에서 오브젝트를 반환하는 코루틴입니다. 지정된 지연 시간 후에 오브젝트를 풀에 다시 추가합니다.
        /// </summary>
        IEnumerator ReturnRoutine(GameObject instance, float delay, CoroutineRef coroutineRef = null)
        {
            yield return GetDelay(delay);
            if (instance == null)
                yield break;

            if (instance.activeSelf == false)
                yield break;

            coroutineRef.coroutine = null;
            Return(instance);
        }
        #endregion    
        /// <summary>
        /// 해당 프리팹에 대한 풀 정보를 찾거나, 없으면 새로 생성합니다.
        /// 프리팹의 인스턴스 ID를 기준으로 Dictionary에서 관리됩니다.
        /// </summary>
        private PoolInfo FindPool(GameObject poolPrefab)
        {
            if (poolPrefab == null)
            {
                Debug.LogError($"{poolPrefab}가 참조되어 있지 않습니다");
                return null;
            }

            int prefabID = poolPrefab.GetInstanceID();

            PoolInfo pool = default;
            if (Instance._poolDic.ContainsKey(prefabID) == false)
            {
                RegisterPool(poolPrefab, prefabID);
            }
            pool = Instance._poolDic[prefabID];
            pool.IsUsed = true;
            Instance._poolDic[prefabID] = pool;
            return pool;
        }

        /// <summary>
        /// 해당 프리팹에 대한 풀 정보를 찾거나, 없으면 새로 생성합니다.
        /// 프리팹의 인스턴스 ID를 기준으로 Dictionary에서 관리됩니다.
        /// </summary>
        private PoolInfo FindResourcePool(string resources)
        {
            Dictionary<string, int> resourcePool = Instance._resourcesPoolDic;
            PoolInfo pool = default;
            if (resourcePool.ContainsKey(resources) == false)
            {
                // 리소시스 프리팹 로드
                GameObject prefab = Resources.Load<GameObject>(resources);
                if (prefab == null)
                {
                    Debug.LogError($"Resources에 {resources}와 일치하는 리소스가 없습니다");
                    return null;
                }
                // 프리팹 instanceID값 캐싱
                int prefabID = prefab.GetInstanceID();
                // 풀에 등록
                RegisterPool(prefab, prefabID);
                // 리소시스 풀에 등록
                resourcePool.Add(resources, prefabID);
            }

            pool = Instance._poolDic[resourcePool[resources]];
            pool.IsUsed = true;
            Instance._poolDic[resourcePool[resources]] = pool;
            return pool;
        }

        private PoolInfo RegisterPool(GameObject poolPrefab, int prefabID)
        {
            // 풀용 부모 오브젝트 생성 후 계층 구조 정리
            Transform newParent = new GameObject(poolPrefab.name).transform;
            newParent.SetParent(Instance.transform, true); // parent

            // 새로운 풀 스택과 정보 생성
            Stack<GameObject> newPool = new Stack<GameObject>(); // pool
            PoolInfo newPoolInfo = GetPoolInfo(newPool, poolPrefab, newParent);

            // 풀 딕셔너리 추가
            Instance._poolDic.Add(prefabID, newPoolInfo);

            // 비활성화 여부 감지 코루틴 시작
            Instance.StartCoroutine(Instance.IsActiveRoutine(prefabID));
            return newPoolInfo;
        }
        /// <summary>
        /// 풀 정보 생성
        /// </summary>
        private PoolInfo GetPoolInfo(Stack<GameObject> pool, GameObject prefab, Transform parent)
        {
            PoolInfo info = new PoolInfo();
            info.Pool = pool;
            info.Parent = parent;
            info.Prefab = prefab;
            return info;
        }
        /// <summary>
        /// PooledObject 컴포넌트를 오브젝트에 추가하거나 가져옵니다.
        /// PoolInfo를 연결하고, 풀 개수를 증가시키며, 자동 비활성화 이벤트를 구독합니다.
        /// </summary>
        private PooledObject AddPoolObjectComponent(GameObject instance, PoolInfo info)
        {
            PooledObject poolObject = instance.GetOrAddComponent<PooledObject>();
            poolObject.PoolInfo = info;
            info.PoolCount++;
            poolObject.SubscribePoolDeactivateEvent();

            return poolObject;
        }

        /// <summary>
        /// 오브젝트를 풀에서 가져오는 실제 처리 로직입니다. 오브젝트가 남아있으면 재사용하고, 없으면 새로 생성합니다.
        /// </summary>
        private GameObject ProcessGet(PoolInfo info)
        {
            GameObject instance = null;
            PooledObject poolObject = null;
            if (FindObject(info))
            {
                // 기존 풀에서 꺼냄
                instance = info.Pool.Pop();
                instance.transform.position = Vector3.zero;
                instance.transform.rotation = Quaternion.identity;
                instance.transform.SetParent(null);
                instance.gameObject.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());

                poolObject = instance.GetComponent<PooledObject>();

            }
            else
            {
                // 새로 생성
                instance = Instantiate(info.Prefab);
                poolObject = AddPoolObjectComponent(instance, info);
            }
            // Rigidbody 초기화
            WakeUpRigidBody(poolObject);

            poolObject.OnCreateFromPool();
            info.ActiveCount++;
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 Transform에 위치시키며, 월드 포지션을 유지할지 여부를 설정합니다. 해당 프리팹에 대한 풀 정보를 찾고, 활성화된 오브젝트가 있으면 반환하고, 없으면 새로 생성합니다.
        /// </summary>
        private GameObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false)
        {
            GameObject instance = null;
            PooledObject poolObject = null;
            if (FindObject(info))
            {
                // 기존 풀에서 꺼냄
                instance = info.Pool.Pop();
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
                poolObject = instance.GetComponent<PooledObject>();
            }
            else
            {
                // 새로 생성
                instance = Instantiate(info.Prefab, transform, worldPositionStay);
                poolObject = AddPoolObjectComponent(instance, info);
            }
            // Rigidbody 초기화
            WakeUpRigidBody(poolObject);

            poolObject.OnCreateFromPool();
            info.ActiveCount++;
            return instance;
        }
        /// <summary>
        /// 풀에서 오브젝트를 가져오고, 지정된 위치와 회전을 설정합니다. 해당 프리팹에 대한 풀 정보를 찾고, 활성화된 오브젝트가 있으면 반환하고, 없으면 새로 생성합니다.
        /// </summary>
        private GameObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot)
        {
            GameObject instance = null;

            PooledObject poolObject = null;

            if (FindObject(info))
            {
                // 기존 풀에서 꺼냄
                instance = info.Pool.Pop();
                instance.transform.position = pos;
                instance.transform.rotation = rot;
                instance.transform.SetParent(null);
                instance.gameObject.SetActive(true);
                SceneManager.MoveGameObjectToScene(instance, SceneManager.GetActiveScene());
                poolObject = instance.GetComponent<PooledObject>();
            }
            else
            {
                // 새로 생성
                instance = Instantiate(info.Prefab, pos, rot);
                poolObject = AddPoolObjectComponent(instance, info);
            }
            // Rigidbody 초기화
            WakeUpRigidBody(poolObject);

            poolObject.OnCreateFromPool();
            info.ActiveCount++;
            return instance;
        }
        /// <summary>
        /// 오브젝트를 풀에 반환하고 비활성화 후 다시 스택에 넣습니다.
        /// 위치, 회전, 스케일, 부모 등 초기 상태로 복원합니다.
        /// </summary>
        private IPoolInfoReadOnly ProcessReturn(GameObject instance)
        {
            //CreateObjectPool();
            if (instance == null)
                return null;

            if (instance.activeSelf == false)
                return null;

            PooledObject poolObject = instance.GetComponent<PooledObject>();
            PoolInfo info = FindPool(poolObject.PoolInfo.Prefab);
            info.ActiveCount--;

            // Transform 초기화
            instance.transform.position = info.Prefab.transform.position;
            instance.transform.rotation = info.Prefab.transform.rotation;
            instance.transform.localScale = info.Prefab.transform.localScale;
            instance.transform.SetParent(info.Parent);

            // RigidBody 초기화
            SleepRigidbody(poolObject);

            // 리턴하기 전에 호출 
            poolObject.OnReturnToPool();

            instance.gameObject.SetActive(false);
            info.Pool.Push(instance.gameObject);

            return info;
        }

        private static void SleepRigidbody(PooledObject instance)
        {
#if UNITY_6000_0_OR_NEWER
            Rigidbody rb = instance.CachedRb;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.linearVelocity = Vector2.zero;
                rb2D.angularVelocity = 0;
                rb2D.Sleep();
            }
#else
            Rigidbody rb = instance.CachedRb;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
                rb2D.angularVelocity = 0f;
                rb2D.Sleep();
            }
#endif
        }

        private static void WakeUpRigidBody(PooledObject instance)
        {
#if UNITY_6000_0_OR_NEWER
            Rigidbody rb = instance.CachedRb;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.WakeUp();
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.linearVelocity = Vector2.zero;
                rb2D.angularVelocity = 0;
                rb2D.WakeUp();
            }
#else
            Rigidbody rb = instance.CachedRb;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.WakeUp();
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
                rb2D.angularVelocity = 0f;
                rb2D.WakeUp();
            }
#endif
        }
        /// <summary>
        /// 현재 풀에 재사용 가능한 오브젝트가 존재하는지 확인합니다.
        /// null 오브젝트가 껴 있으면 제거합니다.
        /// </summary>
        private static bool FindObject(PoolInfo info)
        {
            if (info == null) return false;

            GameObject instance = null;
            while (true)
            {
                if (info.Pool.Count <= 0)
                    return false;

                instance = info.Pool.Peek();
                if (instance != null)
                    break;

                // null 제거
                info.Pool.Pop();
            }

            return true;

        }
        /// <summary>
        /// 해당 풀의 활성 상태를 주기적으로 감지하는 코루틴입니다.
        /// 일정 시간 사용되지 않으면 풀을 정리합니다.
        /// </summary>
        private IEnumerator IsActiveRoutine(int id)
        {
            float delayTime = 10f;
            float timer = MaxTimer;
            while (true)
            {
                // 풀 사용했을때 시간 초기화
                if (Instance._poolDic[id].IsUsed == true)
                {
                    timer = MaxTimer;
                    PoolInfo pool = Instance._poolDic[id];
                    pool.IsUsed = false;
                    pool.IsActive = true;
                }

                // 타이머 종료 시 
                if (timer <= 0)
                {
                    ClearPool(id);
                }
                else
                {
                    timer -= delayTime;
                }
                yield return GetDelay(delayTime);
            }
        }

        /// <summary>
        /// 지정된 ID의 풀을 비우고 비활성화 처리합니다.
        /// OnPoolDormant 이벤트가 있다면 호출됩니다.
        /// </summary>
        private void ClearPool(int id)
        {
            PoolInfo info = Instance._poolDic[id];

            if (info.IsActive == true)
                return;

            info.OnPoolDormant?.Invoke();

            info.PoolCount = 0;
            info.ActiveCount = 0;
            info.Pool = new Stack<GameObject>();
            info.IsActive = false;
        }

        /// <summary>
        /// 지정된 시간만큼 대기하는 WaitForSeconds 객체를 반환합니다. 이미 생성된 객체가 있으면 재사용합니다.
        /// </summary>
        private WaitForSeconds GetDelay(float time)
        {
            float normalize = Mathf.Round(time * 100f) * 0.01f;

            if (_delayDic.ContainsKey(normalize) == false)
            {
                _delayDic.Add(normalize, new WaitForSeconds(normalize));
            }
            return _delayDic[normalize];
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnRuntimeLoad()
        {
            s_isApplicationQuit = false;
        }
        private void OnApplicationQuit()
        {
            s_isApplicationQuit = true;
        }

    }
}