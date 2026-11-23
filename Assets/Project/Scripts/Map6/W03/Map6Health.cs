using UnityEngine;

// Oyuncunun canını ve canı sıfırlandığında başlangıç noktasına dönüşünü yönetir
public class Map6Health : MonoBehaviour
{
    [Header("Can ve Yeniden Doğma Özelliği")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Transform respawnPoint = null;
    [SerializeField] private float hitCooldown = 0.75f;

    private int health;
    private Rigidbody rb;
    private float nextHitTime;

    void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;
        if (Time.time < nextHitTime) return;

        nextHitTime = Time.time + hitCooldown;
        health -= damage;
        Debug.Log("Hasar alındı: -" + damage + " | Kalan Can: " + Mathf.Max(0, health));

        if (health <= 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        health = maxHealth;

        if (respawnPoint != null)
        {
            Vector3 targetPos = respawnPoint.position + Vector3.up * 1f;
            if (rb != null)
            {
                rb.position = targetPos;
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                transform.position = targetPos;
            }
        }

        Debug.Log("Can sıfırlandı, başlangıç noktasına dönüldü!");
    }

    public int CurrentHealth() => health;
}
