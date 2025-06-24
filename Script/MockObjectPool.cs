using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

namespace AutoPool
{
    public class MockObjectPool : IObjectPool
    {
        public IPoolInfoReadOnly GetInfo(GameObject prefab)
        {
            return new MockPoolInfo(prefab.name);
        }

        public IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component
        {
            return new MockPoolInfo(prefab.name);
        }

        public IPoolInfoReadOnly GetResourcesInfo(string resources)
        {
           return new MockPoolInfo(resources);
        }

        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            Debug.Log("[SetResourcesPreload]: " + prefab.name + " with count: " + count);
            return new MockPoolInfo(prefab.name);
        }

        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            Debug.Log("[SetResourcesPreload]: " + prefab.name + " with count: " + count);
            return new MockPoolInfo(prefab.name);
        }

        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count)
        {
            Debug.Log("[SetResourcesPreload]: " + resources + " with count: " + count);
            return new MockPoolInfo(resources);
        }

        public IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
           Debug.Log($"[ClearPool]: {prefab.name}");
            // Mocking ClearPool
            return new MockPoolInfo(prefab.name);
        }

        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
           Debug.Log($"[ClearPool]: {prefab.name}");
            // Mocking ClearPool
            return new MockPoolInfo(prefab.name);
        }

        public IPoolInfoReadOnly ClearResourcesPool(string resources)
        {
           Debug.Log($"[ClearResourcesPool]: {resources}");
            // Mocking ClearResourcesPool
            return new MockPoolInfo(resources);
        }

        public GameObject Get(GameObject prefab)
        {
            Debug.Log($"[Get]: {prefab.name}");
            return new GameObject($"{prefab.name}");
        }

        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay)
        {
            Debug.Log($"[Get]: {prefab.name}");

            GameObject gameObject = new GameObject($"[Mock]{prefab.name}", typeof(PooledObject));
            PooledObject pooledObject = gameObject.GetComponent<PooledObject>();
            pooledObject.PoolInfo = new MockPoolInfo(prefab.name);

            gameObject.transform.SetParent(transform, worldPositionStay);

            if (worldPositionStay == false)
            {
                gameObject.transform.localPosition = prefab.transform.localPosition;
                gameObject.transform.localRotation = prefab.transform.localRotation;
                gameObject.transform.localScale = prefab.transform.localScale;
            }

            return gameObject;
        }

        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            Debug.Log($"[Get]: {prefab.name}");
            GameObject gameObject = new GameObject($"[Mock]{prefab.name}");
            gameObject.transform.position = pos;
            gameObject.transform.rotation = rot;
            return gameObject;
        }

        public T Get<T>(T prefab) where T : Component
        {
            Debug.Log($"[Get]: {prefab.name}");
            GameObject gameObject = new GameObject($"[Mock]{prefab.name}", typeof(T));
            T component = gameObject.GetComponent<T>();
            component.gameObject.name = $"[Mock]{prefab.name}";
            return component;
        }

        public T Get<T>(T prefab, Transform transform, bool worldPositionStay) where T : Component
        {
            Debug.Log($"[Get]: {prefab.name}");
            // GameObject 생성 + T 붙이기
            GameObject gameObject = new GameObject($"[Mock]{prefab.name}", typeof(T));
            T component = gameObject.GetComponent<T>();

            // 부모 설정
            gameObject.transform.SetParent(transform, worldPositionStay);

            // 위치 복사 (필요시)
            if (!worldPositionStay)
            {
                gameObject.transform.localPosition = prefab.transform.localPosition;
                gameObject.transform.localRotation = prefab.transform.localRotation;
                gameObject.transform.localScale = prefab.transform.localScale;
            }
            return component;
        }

        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
        {
            Debug.Log($"[Get]: {prefab.name}");
            // GameObject 생성 + T 붙이기
            GameObject gameObject = new GameObject($"[Mock]{prefab.name}", typeof(T));
            T component = gameObject.GetComponent<T>();
            // 위치 설정
            gameObject.transform.position = pos;
            gameObject.transform.rotation = rot;
            return component;
        }

        public GameObject ResourcesGet(string resouces)
        {
            Debug.Log($"[ResourcesGet]: {resouces}");
            // Mocking Resources.Get
            GameObject gameObject = new GameObject($"[Mock]{resouces}");
            return gameObject;
        }

        public GameObject ResourcesGet(string resouces, Transform transform, bool worldPositionStay)
        {
            Debug.Log($"[ResourcesGet]: {resouces}");
            // Mocking Resources.Get
            GameObject gameObject = new GameObject($"[Mock]{resouces}");
            gameObject.transform.SetParent(transform, worldPositionStay);
            return gameObject;
        }

        public GameObject ResourcesGet(string resouces, Vector3 pos, Quaternion rot)
        {
            Debug.Log($"[ResourcesGet]: {resouces}");
            // Mocking Resources.Get
            GameObject gameObject = new GameObject($"[Mock]{resouces}");
            gameObject.transform.position = pos;
            gameObject.transform.rotation = rot;
            return gameObject;
        }

        public T ResourcesGet<T>(string resouces) where T : Component
        {
            Debug.Log($"[ResourcesGet]: {resouces}");
            // Mocking Resources.Get
            GameObject gameObject = new GameObject($"[Mock]{resouces}", typeof(T));
            T component = gameObject.GetComponent<T>();
            component.gameObject.name = $"[Mock]{resouces}";
            return component;
        }

        public T ResourcesGet<T>(string resouces, Transform transform, bool worldPositionStay) where T : Component
        {
            Debug.Log($"[ResourcesGet]: {resouces}");
            // Mocking Resources.Get
            GameObject gameObject = new GameObject($"[Mock]{resouces}", typeof(T));
            T component = gameObject.GetComponent<T>();
            gameObject.transform.SetParent(transform, worldPositionStay);
            if (!worldPositionStay)
            {
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localRotation = Quaternion.identity;
                gameObject.transform.localScale = Vector3.one;
            }
            return component;
        }

        public T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component
        {
            Debug.Log($"[ResourcesGet]: {resouces}");
            // Mocking Resources.Get
            GameObject gameObject = new GameObject($"[Mock]{resouces}", typeof(T));
            T component = gameObject.GetComponent<T>();
            gameObject.transform.position = pos;
            gameObject.transform.rotation = rot;
            return component;
        }

        public IPoolInfoReadOnly Return(GameObject instance)
        {
            Debug.Log($"[Return]: {instance.name}");
            // Mocking Return
            Object.Destroy(instance);
            return new MockPoolInfo(instance.name);
        }

        public IPoolInfoReadOnly Return<T>(T instance) where T : Component
        {
            Debug.Log($"[Return]: {instance.name}");
            // Mocking Return
            Object.Destroy(instance.gameObject);
            return new MockPoolInfo(instance.name);
        }

        public void Return(GameObject instance, float delay)
        {
            Debug.Log($"[Return with delay]: {instance.name} after {delay} seconds");
            // Mocking delayed return
        }

        public void Return<T>(T instance, float delay) where T : Component
        {
            Debug.Log($"[Return with delay]: {instance.name} after {delay} seconds");
        }
    }

    public class MockPoolInfo : IPoolInfoReadOnly
    {
        public MockPoolInfo(string name)
        {
            Name = name;
        }
        public bool IsMock => true;
        public Stack<GameObject> Pool => null;

        public GameObject Prefab => null;

        public string Name;
        string IPoolInfoReadOnly.Name => Name;
        public Transform Parent => null;

        public bool IsActive => false;

        public bool IsUsed => false;

        public int PoolCount => 1;

        public int ActiveCount => 1;

        UnityAction IPoolInfoReadOnly.OnPoolDormant { get => OnPoolDormant; set => OnPoolDormant = value; }

        public UnityAction OnPoolDormant;
    }
}