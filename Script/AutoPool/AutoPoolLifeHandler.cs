using System.Collections;
using UnityEngine;

namespace AutoPool
{
    public class AutoPoolLifeHandler
    {
        AutoPool _autoPool;

        public AutoPoolLifeHandler(AutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// 해당 풀의 활성 상태를 주기적으로 감지하는 코루틴입니다.
        /// 일정 시간 사용되지 않으면 풀을 정리합니다.
        /// </summary>
        public IEnumerator IsActiveRoutine(int id)
        {
            float delayTime = 10f;
            float timer = _autoPool.MaxTimer;
            while (true)
            {
                // 풀 사용했을때 시간 초기화
                if (_autoPool.PoolDic[id].IsUsed == true)
                {
                    timer = _autoPool.MaxTimer;
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
    }
}