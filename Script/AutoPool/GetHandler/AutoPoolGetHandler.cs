using System;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Central get handler that routes prefab, Resources, and generic pool requests
    /// to their respective sub-handlers.
    /// </summary>
    public class AutoPoolGetHandler
    {
        MainAutoPool _autoPool;
        AutoPoolResourcesGetHandler _resourcesGetHandler;
        AutoPoolCommonGetHandler _commonGetHandler;
        AutoPoolGenericPoolGetHandler _genericGetHandler;
        AutoPoolProcessGetHandler _processGetHandler;

        public AutoPoolGetHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
            _resourcesGetHandler = new AutoPoolResourcesGetHandler(this, autoPool);
            _commonGetHandler = new AutoPoolCommonGetHandler(this, autoPool);
            _genericGetHandler = new AutoPoolGenericPoolGetHandler(this, autoPool);
            _processGetHandler = new AutoPoolProcessGetHandler(this, autoPool);
        }

        #region GetPool
        #region Common

        /// <summary>Retrieves a GameObject instance from the prefab pool.</summary>
        public GameObject Get(GameObject prefab) => _commonGetHandler.Get(prefab);

        /// <summary>Retrieves a GameObject instance and parents it under <paramref name="transform"/>.</summary>
        public GameObject Get(GameObject prefab, Transform transform, bool worldPositionStay = false) => _commonGetHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>Retrieves a GameObject instance at the given position and rotation.</summary>
        public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot) => _commonGetHandler.Get(prefab, pos, rot);

        /// <summary>Retrieves a Component instance from the prefab pool.</summary>
        public T Get<T>(T prefab) where T : Component => _commonGetHandler.Get(prefab);

        /// <summary>Retrieves a Component instance and parents it under <paramref name="transform"/>.</summary>
        public T Get<T>(T prefab, Transform transform, bool worldPositionStay = false) where T : Component => _commonGetHandler.Get(prefab, transform, worldPositionStay);

        /// <summary>Retrieves a Component instance at the given position and rotation.</summary>
        public T Get<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component => _commonGetHandler.Get(prefab, pos, rot);

        #endregion

        #region Resources

        /// <summary>Retrieves a GameObject from the Resources pool.</summary>
        public GameObject ResourcesGet(string resources) => _resourcesGetHandler.ResourcesGet(resources);

        /// <summary>Retrieves a Resources-path GameObject and parents it under <paramref name="transform"/>.</summary>
        public GameObject ResourcesGet(string resources, Transform transform, bool worldPositionStay = false) => _resourcesGetHandler.ResourcesGet(resources, transform, worldPositionStay);

        /// <summary>Retrieves a Resources-path GameObject at the given position and rotation.</summary>
        public GameObject ResourcesGet(string resources, Vector3 pos, Quaternion rot) => _resourcesGetHandler.ResourcesGet(resources, pos, rot);

        /// <summary>Retrieves a Component from the Resources pool.</summary>
        public T ResourcesGet<T>(string resources) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources);

        /// <summary>Retrieves a Resources-path Component and parents it under <paramref name="transform"/>.</summary>
        public T ResourcesGet<T>(string resources, Transform transform, bool worldPositionStay = false) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, transform, worldPositionStay);

        /// <summary>Retrieves a Resources-path Component at the given position and rotation.</summary>
        public T ResourcesGet<T>(string resources, Vector3 pos, Quaternion rot) where T : Component => _resourcesGetHandler.ResourcesGet<T>(resources, pos, rot);

        #endregion

        #region Generic

        /// <summary>Retrieves an instance of <typeparamref name="T"/> from the generic pool.</summary>
        public T GenericGet<T>() where T : class, IPoolGeneric, new() => _genericGetHandler.Get<T>();

        #endregion
        #endregion

        /// <summary>Pops an instance from the pool stack and activates it at default position/rotation.</summary>
        public PooledObject ProcessGet(PoolInfo info) => _processGetHandler.ProcessGet(info);

        /// <summary>Pops an instance from the pool stack and parents it under <paramref name="transform"/>.</summary>
        public PooledObject ProcessGet(PoolInfo info, Transform transform, bool worldPositionStay = false) => _processGetHandler.ProcessGet(info, transform, worldPositionStay);

        /// <summary>Pops an instance from the pool stack and places it at the given position/rotation.</summary>
        public PooledObject ProcessGet(PoolInfo info, Vector3 pos, Quaternion rot) => _processGetHandler.ProcessGet(info, pos, rot);

        /// <summary>Pops or creates a generic instance of type <typeparamref name="T"/>.</summary>
        public T ProcessGenericGet<T>(GenericPoolInfo poolInfo) where T : class, IPoolGeneric, new() => _processGetHandler.ProcessGenericGet<T>(poolInfo);
    }
}
