using System;
using System.Collections.Generic;
using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Handles preloading (warm-up) and clearing of GameObject and generic pools.
    /// </summary>
    public class AutoPoolPreloadHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolPreloadHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>Pre-warms the prefab pool to at least <paramref name="count"/> instances.</summary>
        public IPoolInfoReadOnly SetPreload(GameObject prefab, int count)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            return ProcessPreload(info, count);
        }

        /// <summary>Pre-warms the Component prefab pool to at least <paramref name="count"/> instances.</summary>
        public IPoolInfoReadOnly SetPreload<T>(T prefab, int count) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            return ProcessPreload(info, count);
        }

        /// <summary>Pre-warms the Resources pool to at least <paramref name="count"/> instances.</summary>
        public IPoolInfoReadOnly SetResourcesPreload(string resources, int count)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            return ProcessPreload(info, count);
        }

        /// <summary>
        /// Instantiates instances until PoolCount reaches <paramref name="count"/>.
        /// Each instance is deactivated and pushed onto the pool stack.
        /// </summary>
        private IPoolInfoReadOnly ProcessPreload(PoolInfo info, int count)
        {
            if (info == null)
            {
                Debug.LogError("The pool information is invalid.");
                return null;
            }

            info.IsActive = true;
            while (info.PoolCount < count)
            {
                GameObject instance = GameObject.Instantiate(info.Prefab);
                PooledObject poolObject = _autoPool.AddPoolObjectComponent(instance, info);
                instance.transform.SetParent(info.Parent);
                info.Pool.Push(poolObject);

                // Temporarily activate if needed so OnDisable fires and ActiveCount adjusts correctly.
                if (instance.gameObject.activeSelf)
                {
                    info.ActiveCount++;
                    instance.gameObject.SetActive(false);
                }
                else
                {
                    instance.gameObject.SetActive(false);
                }
            }

            return info;
        }

        /// <summary>Destroys all pooled instances for the given prefab.</summary>
        public IPoolInfoReadOnly ClearPool(GameObject prefab)
        {
            PoolInfo info = _autoPool.FindPool(prefab);
            ClearPool(info);
            return info;
        }

        /// <summary>Destroys all pooled instances for the given Component prefab.</summary>
        public IPoolInfoReadOnly ClearPool<T>(T prefab) where T : Component
        {
            PoolInfo info = _autoPool.FindPool(prefab.gameObject);
            ClearPool(info);
            return info;
        }

        /// <summary>Destroys all pooled instances for the given Resources path.</summary>
        public IPoolInfoReadOnly ClearResourcesPool(string resources)
        {
            PoolInfo info = _autoPool.FindResourcesPool(resources);
            ClearPool(info);
            return info;
        }

        /// <summary>Destroys all instances in the generic pool for type <typeparamref name="T"/>.</summary>
        public IGenericPoolInfoReadOnly ClearGenericPool<T>() where T : class, IPoolGeneric, new()
        {
            GenericPoolInfo info = _autoPool.FindGenericPool<T>();
            ClearGenericPool(info);
            return info;
        }

        /// <summary>
        /// Fires the dormant callback (which destroys active instances via SubscribePoolDeactivateEvent)
        /// then replaces the stack with a fresh empty one.
        /// </summary>
        public void ClearPool(PoolInfo info)
        {
            info.OnPoolDormant?.Invoke();
            info.Pool = new Stack<PooledObject>();
            info.IsActive = false;
        }

        /// <summary>
        /// Fires the dormant callback for the generic pool then replaces the stack with a fresh empty one.
        /// </summary>
        public void ClearGenericPool(GenericPoolInfo info)
        {
            info.OnPoolDormant?.Invoke();
            info.Pool = new Stack<IPoolGeneric>();
            info.IsActive = false;
        }
    }
}
