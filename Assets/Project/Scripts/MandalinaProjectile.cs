using UnityEngine;

namespace Portakalcik
{
    public class MandalinaProjectile : MonoBehaviour
    {
        [Tooltip("Denge dokümanına göre mandalina vuruş hasarı 3 HP")]
        public float damage = 3.0f;
        public float lifetime = 5.0f;

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            // Canavara çarptıysa hasar ver
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy == null)
            {
                enemy = collision.gameObject.GetComponentInParent<Enemy>();
            }

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Çarpışmada yok ol
            Destroy(gameObject);
        }
    }
}
