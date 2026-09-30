using UnityEngine;

namespace Portakalcik
{
    public class MandalinaProjectile : MonoBehaviour
    {
        [Tooltip("Denge dokümanına göre mandalina vuruş hasarı 3 HP")]
        public float damage = 3.0f;
        public float lifetime = 5.0f;

        private Vector3 moveDirection;

        private void Start()
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                moveDirection = rb.linearVelocity.normalized;
            }
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Canavara çarptıysa hasar ver ve geriye it
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy == null)
            {
                enemy = collision.gameObject.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                Vector3 knockback = moveDirection != Vector3.zero ? moveDirection : transform.forward;
                enemy.TakeDamage(damage, knockback);

                // Oyuncuya vuruş hissiyatı (Hit Marker) ver
                PlayerController pc = Object.FindAnyObjectByType<PlayerController>();
                if (pc != null)
                {
                    pc.TriggerHitFeedback();
                }
            }

            Destroy(gameObject);
        }
    }
}
