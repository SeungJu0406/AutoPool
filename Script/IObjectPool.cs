using System;
using UnityEngine;

namespace AutoPool
{
    public interface IObjectPool
    {
        IPoolInfoReadOnly GetInfo(GameObject prefab);
        IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component;
        IPoolInfoReadOnly GetResourcesInfo(string resources);
        IPoolInfoReadOnly SetPreload(GameObject prefab, int count);
        IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component;
        IPoolInfoReadOnly SetResourcesPreload(string resources, int count);
        IPoolInfoReadOnly ClearPool(GameObject prefab);
        IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component;
        IPoolInfoReadOnly ClearResourcesPool(string resources);
        GameObject Get(GameObject prefab);
        GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay);
        GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot);
        T Get<T>(T prefab) where T : Component;
        T Get<T>(T prefab, Transform transform, bool worldPositionStay) where T : Component;
        T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component;
        GameObject ResourcesGet(string resouces);
        GameObject ResourcesGet(string resouces, Transform transform, bool worldPositionStay);
        GameObject ResourcesGet(string resouces, Vector3 pos, Quaternion rot);
        T ResourcesGet<T>(string resouces) where T : Component;
        T ResourcesGet<T>(string resouces, Transform transform, bool worldPositionStay) where T : Component;
        T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component;
        T GenericPool<T>() where T : class, IPoolGeneric, new();
        IPoolInfoReadOnly Return(GameObject instance);
        IPoolInfoReadOnly Return<T>(T instance) where T : Component;
        public void Return(GameObject instance, float delay);
        public void Return<T>(T instance, float delay) where T : Component;
        public IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new();
    }
}