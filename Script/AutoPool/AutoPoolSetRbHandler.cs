using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// 풀링된 오브젝트의 Rigidbody / Rigidbody2D 물리 상태를 Sleep/Wake 처리하는 핸들러입니다.
    /// </summary>
    public class AutoPoolSetRbHandler
    {
        MainAutoPool _autoPool;

        /// <summary>
        /// 메인 풀 인스턴스를 받아 Rigidbody 제어 핸들러를 초기화합니다.
        /// </summary>
        public AutoPoolSetRbHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// 주어진 풀 오브젝트에 연결된 Rigidbody / Rigidbody2D를 정지시키고 Sleep 상태로 전환합니다.
        /// </summary>
        public void SleepRigidbody(PooledObject instance)
        {
#if UNITY_6000_0_OR_NEWER
            Rigidbody rb = instance.CachedRb;                     // 1) 캐시된 3D Rigidbody 참조
            if (rb != null)                                       // 2) 존재하면
            {
                rb.linearVelocity = Vector3.zero;                 //    선형 속도 0
                rb.angularVelocity = Vector3.zero;                //    각속도 0
                rb.Sleep();                                       //    물리 시뮬레이션에서 Sleep 처리
            }
            Rigidbody2D rb2D = instance.CachedRb2D;               // 3) 캐시된 2D Rigidbody 참조
            if (rb2D != null)                                     // 4) 존재하면
            {
                rb2D.linearVelocity = Vector2.zero;               //    선형 속도 0
                rb2D.angularVelocity = 0;                         //    각속도 0
                rb2D.Sleep();                                     //    2D 물리 Sleep 처리
            }
#else
            Rigidbody rb = instance.CachedRb;                     // 1) 6000 이전 버전에서는 velocity API 사용
            if (rb != null)
            {
                rb.velocity = Vector3.zero;                       //    선형 속도 0
                rb.angularVelocity = Vector3.zero;                //    각속도 0
                rb.Sleep();                                       //    Sleep 처리
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;                     //    선형 속도 0
                rb2D.angularVelocity = 0f;                        //    각속도 0
                rb2D.Sleep();                                     //    2D Sleep 처리
            }
#endif
        }

        /// <summary>
        /// 주어진 풀 오브젝트에 연결된 Rigidbody / Rigidbody2D를 깨우고, 속도를 초기화합니다.
        /// </summary>
        public void WakeUpRigidBody(PooledObject instance)
        {
#if UNITY_6000_0_OR_NEWER
            Rigidbody rb = instance.CachedRb;                     // 1) 캐시된 3D Rigidbody 참조
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;                 //    깨어날 때 초기 속도 0
                rb.angularVelocity = Vector3.zero;
                rb.WakeUp();                                      //    물리 시뮬레이션에 다시 참여
            }
            Rigidbody2D rb2D = instance.CachedRb2D;               // 2) 캐시된 2D Rigidbody 참조
            if (rb2D != null)
            {
                rb2D.linearVelocity = Vector2.zero;
                rb2D.angularVelocity = 0;
                rb2D.WakeUp();                                    //    2D 물리 WakeUp
            }
#else
            Rigidbody rb = instance.CachedRb;                     // 1) 6000 이전 버전에서는 velocity API 사용
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.WakeUp();
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
                rb2D.angularVelocity = 0f;
                rb2D.WakeUp();
            }
#endif
        }
    }
}