using UnityEngine;

namespace AutoPool
{
    public class SampleObject : MonoBehaviour, IPooledObject
    {
        public void OnCreateFromPool()
        {
            Debug.Log($"{name} : PooledObject Initialize");
        }

        public void OnReturnToPool()
        {
            Debug.Log($"{name} : PooledObject Return");
        }
    }
}