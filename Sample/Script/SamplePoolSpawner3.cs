using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AutoPool
{
    public class SamplePoolSpawner3 : MonoBehaviour
    {
        [SerializeField] private Button _getButton;
        [SerializeField] private Button _returnButton;

        [SerializeField] private Button _setMockButton;
        [SerializeField] private Button _setRealButton;

        [SerializeField] private GameObject _samplePrefab;
        [SerializeField] private Vector3 _randomRange;


        private Queue<GameObject> _sampleQueue = new Queue<GameObject>();


        private void Start()
        {
            _getButton.onClick.AddListener(() => GetObject());
            _returnButton.onClick.AddListener(() => ReturnObject());

            _setMockButton.onClick.AddListener(() => SetMock(true));
            _setRealButton.onClick.AddListener(() => SetMock(false));
        }

        private void SetMock(bool isMock)
        {
            if(isMock == true)
            {
                ObjectPool.SetMock();
            }
            else
            {
                ObjectPool.SetReal();
            }
        }


        private void GetObject()
        {
            // Get Method
            GameObject newObj = ObjectPool.Get(_samplePrefab, transform).OnDebug("GetPool Test");

            _sampleQueue.Enqueue(newObj);

            newObj.transform.position = GetRandomPos();
        }
        private void ReturnObject()
        {
            if (_sampleQueue.Count > 0)
            {
                GameObject obj = _sampleQueue.Dequeue();

                // Return Method
                ObjectPool.Return(obj).OnDebug("ReturnPool Test");
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