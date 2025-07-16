using System.Collections;
using UnityEngine;

namespace AutoPool
{
    public class SamplePoolSpawner1 : MonoBehaviour
    {
        private GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
        {
            // Pooling Method
            return AutoPool.Get(prefab, pos, transform.rotation).ReturnAfter(_returnDelay);
        }


        public int Count;
        [SerializeField] private GameObject[] _prefabs;
        [SerializeField] private Vector3 _randomRange;
        [SerializeField] private float _returnDelay;
        WaitForSeconds _second;

        public void SetCount(int count)
        {
            Count = count;
            if (Count == 0)
                return;

            float delay = 1f / (float)count;
            _second = new WaitForSeconds(delay);
        }

        void Start()
        {
            float delay = 1f / (float)Count;
            _second = new WaitForSeconds(delay);
            StartCoroutine(SpawnRoutine());
        }
        IEnumerator SpawnRoutine()
        {
            while (true)
            {
                if (Count > 0)
                {
                    GameObject prefab = _prefabs[Random.Range(0, _prefabs.Length)];
                    Vector3 pos = GetRandomPos();
                    Quaternion rot = transform.rotation;
                    GameObject instance = Spawn(prefab, pos, rot);
                    instance.transform.SetParent(transform, false);
                }
                yield return _second;
            }
        }
        private Vector3 GetRandomPos()
        {
            Vector3 randomPos = new Vector3
                (
                transform.position.x + Random.Range(-_randomRange.x, _randomRange.x),
                transform.position.y + Random.Range(-_randomRange.y, _randomRange.y),
                transform.position.z + Random.Range(-_randomRange.z, _randomRange.z)
                );
            return randomPos;
        }
    }
}