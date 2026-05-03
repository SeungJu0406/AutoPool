using AutoPool_Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SamplePoolSpawner3 : MonoBehaviour
{
    [SerializeField] private Button _getButton;
    [SerializeField] private Text _text;
    [SerializeField] private Text _returnTimer;

    [SerializeField] private SampleObject _samplePrefab;
    [SerializeField] private Vector3 _randomRange;

    private Queue<SampleObject> _sampleQueue = new Queue<SampleObject>();
    private bool _isReturn;

    [SerializeField]private float _curTime;
    private float _returnTime = 5f;

    // Spawns a SampleObject from the pool and sets its parent.
    private SampleObject Spawn(SampleObject sample, Transform parent)
    {
        return ObjectPool.Get(sample, parent).OnDebug("GetPool Test").ReturnWhen(() => _isReturn == true);
    }



    private void LateUpdate()
    {
        if (_isReturn == true)
        {
            _isReturn = false;
        }
    }

    private void Start()
    {
        _getButton.onClick.AddListener(() => GetObject());
    }
    private void Update()
    {
        IPoolInfoReadOnly poolInfo = GetPoolInfo(_samplePrefab);
        _text.text = $"Active: {poolInfo.ActiveCount} / Total: {poolInfo.PoolCount}";
        _returnTimer.text = $"Return Timer    {(int)_curTime} / {_returnTime}";

        _curTime += Time.deltaTime;
        if (_curTime >= _returnTime)
        {
            _curTime = 0f;
            _isReturn = true;
        }
    }

    private void GetObject()
    {
        SampleObject newObj = Spawn(_samplePrefab, transform);

        _sampleQueue.Enqueue(newObj);

        newObj.transform.position = GetRandomPos();

        PooledObject pooled = newObj.GetComponent<PooledObject>();
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

    // Gets the pool information for a specific SampleObject.
    private IPoolInfoReadOnly GetPoolInfo(SampleObject sample)
    {
        return ObjectPool.GetInfo(sample);
    }
}
