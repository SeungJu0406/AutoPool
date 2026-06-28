using System;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Contract for a pool manager that handles prefab, Resources, and generic object pools.
    /// </summary>
    public interface IObjectPool
    {
        /// <summary>Returns pool info for the given prefab.</summary>
        IPoolInfoReadOnly GetInfo(GameObject prefab);

        /// <summary>Returns pool info for the given Component prefab.</summary>
        IPoolInfoReadOnly GetInfo<T>(T prefab) where T : Component;

        /// <summary>Returns pool info for a Resources-loaded prefab.</summary>
        IPoolInfoReadOnly GetResourcesInfo(string resources);

        /// <summary>Pre-warms the prefab pool to at least <paramref name="count"/> instances.</summary>
        IPoolInfoReadOnly SetPreload(GameObject prefab, int count);

        /// <summary>Pre-warms the Component prefab pool to at least <paramref name="count"/> instances.</summary>
        IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component;

        /// <summary>Pre-warms the Resources-path pool to at least <paramref name="count"/> instances.</summary>
        IPoolInfoReadOnly SetResourcesPreload(string resources, int count);

        /// <summary>Destroys all pooled instances for the given prefab.</summary>
        IPoolInfoReadOnly ClearPool(GameObject prefab);

        /// <summary>Destroys all pooled instances for the given Component prefab.</summary>
        IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component;

        /// <summary>Destroys all pooled instances for the given Resources path.</summary>
        IPoolInfoReadOnly ClearResourcesPool(string resources);

        /// <summary>Retrieves a GameObject instance from the pool.</summary>
        GameObject Get(GameObject prefab);

        /// <summary>Retrieves a GameObject instance and parents it under <paramref name="transform"/>.</summary>
        GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay);

        /// <summary>Retrieves a GameObject instance and places it at the given position and rotation.</summary>
        GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot);

        /// <summary>Retrieves a Component instance from the pool.</summary>
        T Get<T>(T prefab) where T : Component;

        /// <summary>Retrieves a Component instance and parents it under <paramref name="transform"/>.</summary>
        T Get<T>(T prefab, Transform transform, bool worldPositionStay) where T : Component;

        /// <summary>Retrieves a Component instance and places it at the given position and rotation.</summary>
        T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component;

        /// <summary>Retrieves a GameObject from the Resources-path pool.</summary>
        GameObject ResourcesGet(string resouces);

        /// <summary>Retrieves a Resources-path GameObject and parents it under <paramref name="transform"/>.</summary>
        GameObject ResourcesGet(string resouces, Transform transform, bool worldPositionStay);

        /// <summary>Retrieves a Resources-path GameObject at the given position and rotation.</summary>
        GameObject ResourcesGet(string resouces, Vector3 pos, Quaternion rot);

        /// <summary>Retrieves a Component from the Resources-path pool.</summary>
        T ResourcesGet<T>(string resouces) where T : Component;

        /// <summary>Retrieves a Resources-path Component and parents it under <paramref name="transform"/>.</summary>
        T ResourcesGet<T>(string resouces, Transform transform, bool worldPositionStay) where T : Component;

        /// <summary>Retrieves a Resources-path Component at the given position and rotation.</summary>
        T ResourcesGet<T>(string resouces, Vector3 pos, Quaternion rot) where T : Component;

        /// <summary>Retrieves a generic pool instance of type <typeparamref name="T"/>.</summary>
        T GenericPool<T>() where T : class, IPoolGeneric, new();

        /// <summary>Returns a GameObject instance to the pool.</summary>
        IPoolInfoReadOnly Return(GameObject instance);

        /// <summary>Returns a Component instance to the pool.</summary>
        IPoolInfoReadOnly Return<T>(T instance) where T : Component;

        /// <summary>Returns a GameObject instance to the pool after a delay.</summary>
        void Return(GameObject instance, float delay);

        /// <summary>Returns a Component instance to the pool after a delay.</summary>
        void Return<T>(T instance, float delay) where T : Component;

        /// <summary>Returns a generic pool instance and provides updated pool info.</summary>
        IGenericPoolInfoReadOnly GenericReturn<T>(T instance) where T : class, IPoolGeneric, new();

        /// <summary>Returns a generic pool instance after a delay.</summary>
        void GenericReturn<T>(T instance, float delay) where T : class, IPoolGeneric, new();
    }
}
