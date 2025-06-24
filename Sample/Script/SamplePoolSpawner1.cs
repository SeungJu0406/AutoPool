using System.Collections;
using UnityEngine;

namespace NSJ_EasyPoolKit
{
    public class SamplePoolSpawner1 : MonoBehaviour
    {
        [SerializeField] private GameObject[] _prefabs;
        [SerializeField] private Vector3 _randomRange;
        [SerializeField] private float _delay;
        [SerializeField] private float _returnDelay;

        void Start()
        {
            StartCoroutine(SpawnRoutine());
        }

        IEnumerator SpawnRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(_delay);
            while (true) 
            {
                GameObject prefab = _prefabs[Random.Range(0, _prefabs.Length)];
                Vector3 pos = GetRandomPos();
                Quaternion rot = transform.rotation;

                // Pooling Method
                ObjectPool.Get(prefab, pos, transform.rotation)
                    .ReturnAfter(_returnDelay);


                yield return delay;
            }
           
        }

        private Vector3 GetRandomPos()
        {
            Vector3 randomPos = new Vector3
                (
                transform.position.x + Random.Range(-_randomRange.x,_randomRange.x),
                transform.position.y + Random.Range(-_randomRange.y, _randomRange.y),
                transform.position.z + Random.Range(-_randomRange.z, _randomRange.z)
                );
            return randomPos;
        }
    }
}