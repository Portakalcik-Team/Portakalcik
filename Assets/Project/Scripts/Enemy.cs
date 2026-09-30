using UnityEngine;

namespace Portakalcik
{
    public enum EnemyType
    {
        DikenBocegi,
        GolgeYarasa,
        OrmanTilkisi,
        KokGolemi,
        LabirentMuhafizi
    }

    public class Enemy : MonoBehaviour
    {
        [Header("Canavar Bilgisi")]
        public EnemyType enemyType = EnemyType.DikenBocegi;
        public string monsterName = "Diken Böceği";

        [Header("Denge Sistemi (Seviye 6 Değerleri)")]
        public float maxHealth = 11.0f;
        public float currentHealth = 11.0f;
        public float attackDamage = 9.0f;
        public float attackCooldown = 2.0f;
        public float moveSpeed = 2.4f;

        [Header("AI & Algılama")]
        public float detectionRadius = 8.0f;
        public float attackRadius = 1.6f;

        [Header("Kısayol Kapısı")]
        [Tooltip("Bu canavar öldüğünde açılacak olan kapı")]
        public Door guardedDoor;

        private Transform playerTarget;
        private PlayerController playerController;
        private float lastAttackTime = -99f;
        private bool isDead = false;

        private void Start()
        {
            currentHealth = maxHealth;

            // Oyuncuyu bul
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController>();
            }
        }

        private void Update()
        {
            if (isDead || playerTarget == null) return;

            float dist = Vector3.Distance(transform.position, playerTarget.position);

            if (dist <= detectionRadius)
            {
                // Oyuncuya doğru dön
                Vector3 lookPos = playerTarget.position;
                if (enemyType != EnemyType.GolgeYarasa)
                {
                    lookPos.y = transform.position.y; // Yarasalar hariç yer canavarları yukarı bakmasın
                }
                transform.LookAt(lookPos);

                // Oyuncuya yaklaş
                if (dist > attackRadius)
                {
                    transform.position = Vector3.MoveTowards(transform.position, lookPos, moveSpeed * Time.deltaTime);
                }
                else
                {
                    // Saldırı menzilinde: Saldır
                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        AttackPlayer();
                    }
                }
            }
        }

        private void AttackPlayer()
        {
            lastAttackTime = Time.time;
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage);
                Debug.Log($"[Portakalcik] {monsterName} oyuncuya saldırdı! Hasar: {attackDamage}");
            }
        }

        public void TakeDamage(float amount)
        {
            if (isDead) return;

            currentHealth -= amount;
            Debug.Log($"[Portakalcik] {monsterName} vuruldu! Kalan Can: {currentHealth} / {maxHealth}");

            // Vurulma efekti / Renk yanıp sönmesi
            StartCoroutine(FlashHit());

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private System.Collections.IEnumerator FlashHit()
        {
            Renderer rend = GetComponent<Renderer>();
            if (rend != null)
            {
                Color orig = rend.material.color;
                rend.material.color = Color.red;
                yield return new WaitForSeconds(0.1f);
                if (rend != null) rend.material.color = orig;
            }
        }

        private void Die()
        {
            isDead = true;
            Debug.Log($"[Portakalcik] {monsterName} yenildi!");

            // Korunan kapı varsa aç
            if (guardedDoor != null)
            {
                guardedDoor.OpenDoor();
            }

            Destroy(gameObject, 0.2f);
        }
    }
}
