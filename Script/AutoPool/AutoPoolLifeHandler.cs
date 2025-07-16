using System;
using System.Collections;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolLifeHandler
    {
        MainAutoPool _autoPool;
        float _maxTimer = 600f;
        int _delayTime = 10; // seconds
        public AutoPoolLifeHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// 해당 풀의 활성 상태를 주기적으로 감지하는 코루틴입니다.
        /// 일정 시간 사용되지 않으면 풀을 정리합니다.
        /// </summary>
        public IEnumerator IsActiveRoutine(int id)
        {
            float delayTime = _delayTime;
            float timer = _maxTimer;
            while (true)
            {
                // 풀 사용했을때 시간 초기화
                if (_autoPool.PoolDic[id].IsUsed == true)
                {
                    timer = _maxTimer;
                    PoolInfo pool = _autoPool.PoolDic[id];
                    pool.IsUsed = false;
                    pool.IsActive = true;
                }
                // 타이머 종료 시 
                if (timer <= 0)
                {
                    _autoPool.ClearPool(_autoPool.PoolDic[id]);
                }
                else
                {
                    timer -= delayTime;
                }
                yield return _autoPool.Second(delayTime);
            }
        }

        public IEnumerator IsActiveGenericRoutine<T>() where T : class, IPoolGeneric, new()
        {
            Type type = typeof(T);
            float delayTime = _delayTime;
            float timer = _maxTimer;
            while (true)
            {
                // 풀 사용했을때 시간 초기화
                if (_autoPool.GenericPoolDic[type].IsUsed == true)
                {
                    timer = _maxTimer;
                    GenericPoolInfo pool = _autoPool.GenericPoolDic[type];
                    pool.IsUsed = false;
                    pool.IsActive = true;
                }
                // 타이머 종료 시 
                if (timer <= 0)
                {
                    _autoPool.ClearGenericPool<T>();
                }
                else
                {
                    timer -= delayTime;
                }
                yield return _autoPool.Second(delayTime);
            }

        }
    }
}