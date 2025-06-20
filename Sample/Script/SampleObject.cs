using UnityEngine;

namespace NSJ_EasyPoolKit
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