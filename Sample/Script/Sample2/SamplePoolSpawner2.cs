using NUnit.Framework.Constraints;
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

        // Sets the preload count for a specific SampleObject in the pool.
        private IPoolInfoReadOnly SetPreload(SampleObject sample, int count)
        {
            return ObjectPool.SetPreload(sample, count).OnDebug("SetPreload Test");
        }

        // Clears the pool for a specific SampleObject.
        private IPoolInfoReadOnly ClearPool(SampleObject sample)
        {
            return ObjectPool.ClearPool(sample).OnDebug("ClearPool Test");
        }

        // Spawns a SampleObject from the pool and sets its parent.
        private SampleObject Spawn(SampleObject sample, Transform parent)
        {
            return ObjectPool.Get(sample, parent).OnDebug("GetPool Test");
        }

        // Spawns a SampleObject from the pool at a specific position and rotation.
        private IPoolInfoReadOnly Return(SampleObject instance)
        {
            return ObjectPool.Return(instance).OnDebug("ReturnPool Test");
        }

        // Gets the pool information for a specific SampleObject.
        private IPoolInfoReadOnly GetPoolInfo(SampleObject sample)
        {
            return ObjectPool.GetInfo(sample);
        }

        private void Start()
        {
            _getButton.onClick.AddListener(() => GetObject());
            _returnButton.onClick.AddListener(() => ReturnObject());
            _setPreload.onClick.AddListener(() => ProcessPreload());
            _clear.onClick.AddListener(() => ProcessClearPool());
        }
        private void Update()
        {
            IPoolInfoReadOnly poolInfo = GetPoolInfo(_samplePrefab);
            _text.text = $"Active: {poolInfo.ActiveCount} / Total: {poolInfo.PoolCount}";
        }

        private void ProcessPreload()
        {
            int count = int.TryParse(_inputField.text, out int result) ? result : 0;
            IPoolInfoReadOnly poolInfo = SetPreload(_samplePrefab, count);
            _inputField.text =string.Empty;
        }

        private void ProcessClearPool()
        {
            IPoolInfoReadOnly poolInfo = ClearPool(_samplePrefab);

        }
        private void GetObject()
        {
            SampleObject newObj = Spawn(_samplePrefab, transform);

            _sampleQueue.Enqueue(newObj);

            newObj.transform.position = GetRandomPos();

            PooledObject pooled = newObj.GetComponent<PooledObject>();
        }
        private void ReturnObject()
        {
            if (_sampleQueue.Count > 0)
            {
                SampleObject obj = _sampleQueue.Dequeue();

                IPoolInfoReadOnly poolInfoReadOnly = Return(obj);
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