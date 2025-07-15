using UnityEngine;

namespace AutoPool
{
    public class AutoPoolSetRbHandler
    {
        AutoPool _autoPool;

        public AutoPoolSetRbHandler(AutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        public void SleepRigidbody(PooledObject instance)
        {
#if UNITY_6000_0_OR_NEWER
            Rigidbody rb = instance.CachedRb;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.linearVelocity = Vector2.zero;
                rb2D.angularVelocity = 0;
                rb2D.Sleep();
            }
#else
            Rigidbody rb = instance.CachedRb;
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.velocity = Vector2.zero;
                rb2D.angularVelocity = 0f;
                rb2D.Sleep();
            }
#endif
        }

        public void WakeUpRigidBody(PooledObject instance)
        {
#if UNITY_6000_0_OR_NEWER
            Rigidbody rb = instance.CachedRb;
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.WakeUp();
            }
            Rigidbody2D rb2D = instance.CachedRb2D;
            if (rb2D != null)
            {
                rb2D.linearVelocity = Vector2.zero;
                rb2D.angularVelocity = 0;
                rb2D.WakeUp();
            }
#else
            Rigidbody rb = instance.CachedRb;
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