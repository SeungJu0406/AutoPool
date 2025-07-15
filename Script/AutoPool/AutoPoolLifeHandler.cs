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
        /// �ش� Ǯ�� Ȱ�� ���¸� �ֱ������� �����ϴ� �ڷ�ƾ�Դϴ�.
        /// ���� �ð� ������ ������ Ǯ�� �����մϴ�.
        /// </summary>
        public IEnumerator IsActiveRoutine(int id)
        {
            float delayTime = 10f;
            float timer = _autoPool.MaxTimer;
            while (true)
            {
                // Ǯ ��������� �ð� �ʱ�ȭ
                if (_autoPool.PoolDic[id].IsUsed == true)
                {
                    timer = _autoPool.MaxTimer;
                    PoolInfo pool = _autoPool.PoolDic[id];
                    pool.IsUsed = false;
                    pool.IsActive = true;
                }
                // Ÿ�̸� ���� �� 
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