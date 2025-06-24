using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AutoPool.AutoPool;

namespace AutoPool
{
    public class SamplePoolSpawner2 : MonoBehaviour
    {
        [SerializeField] private Button _getButton;
        [SerializeField] private Button _returnButton;
        [SerializeField] private Button _setPreload;
        [SerializeField] private Button _clear;
        [SerializeField] private InputField _inputField;
        [SerializeField] private Text _text;

        [SerializeField] private SampleObject _samplePrefab;
        [SerializeField] private Vector3 _randomRange;


        private Queue<SampleObject> _sampleQueue = new Queue<SampleObject>();


        private void Start()
        {
            _getButton.onClick.AddListener(() => GetObject());
            _returnButton.onClick.AddListener(() => ReturnObject());
            _setPreload.onClick.AddListener(() => SetPreload());
            _clear.onClick.AddListener(() => ClearPool());
        }
        private void Update()
        {
            IPoolInfoReadOnly poolInfo = ObjectPool.GetInfo(_samplePrefab);
            _text.text = $"Active: {poolInfo.ActiveCount} / Total: {poolInfo.PoolCount}";
        }

        private void SetPreload()
        {
            int count = int.TryParse(_inputField.text, out int result) ? result : 0;
            IPoolInfoReadOnly poolInfo = ObjectPool.SetPreload(_samplePrefab, count).OnDebug("SetResourcesPreload Test");   
            _inputField.text =string.Empty;
        }

        private void ClearPool()
        {
            IPoolInfoReadOnly poolInfo = ObjectPool.ClearPool(_samplePrefab).OnDebug("ClearPool Test");

        }
        private void GetObject()
        {
            // Get Method
            SampleObject newObj = ObjectPool.Get(_samplePrefab, transform).OnDebug("GetPool Test");

            _sampleQueue.Enqueue(newObj);

            newObj.transform.position = GetRandomPos();

            PooledObject pooled = newObj.GetComponent<PooledObject>();
        }
        private void ReturnObject()
        {
            if (_sampleQueue.Count > 0)
            {
                SampleObject obj = _sampleQueue.Dequeue();

                // Return Method
                IPoolInfoReadOnly poolInfoReadOnly = ObjectPool.Return(obj).OnDebug("ReturnPool Test");
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