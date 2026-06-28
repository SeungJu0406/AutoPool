using UnityEngine;

namespace AutoPool_Tool
{
    /// <summary>
    /// Puts pooled objects' Rigidbody / Rigidbody2D to sleep on return
    /// and wakes them on retrieval.
    /// </summary>
    public class AutoPoolSetRbHandler
    {
        MainAutoPool _autoPool;

        public AutoPoolSetRbHandler(MainAutoPool autoPool)
        {
            _autoPool = autoPool;
        }

        /// <summary>
        /// Zeroes velocity on the cached Rigidbody and Rigidbody2D, then calls Sleep()
        /// so the physics engine stops simulating the object while it is in the pool.
        /// </summary>
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

        /// <summary>
        /// Zeroes velocity on the cached Rigidbody and Rigidbody2D, then calls WakeUp()
        /// so the physics engine begins simulating the object again.
        /// </summary>
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
