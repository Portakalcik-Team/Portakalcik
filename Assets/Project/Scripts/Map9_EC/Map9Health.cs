using UnityEngine;

// Seviye 9: oyuncunun canı, bitince S'den başlar
public class Map9Health : MonoBehaviour
{
    // can: denge dokümanı · dönüş yeri: Spawn_09
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Transform respawnPoint;

    private int health;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();   // bir kez bul
        health = maxHealth;
    }

    // canavarlar bunu çağırır
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Can: " + health + " / " + maxHealth);
        if (health <= 0) Respawn();
    }

    // can bitti: S karesine dön, can dolsun
    private void Respawn()
    {
        health = maxHealth;
        rb.position = respawnPoint.position;
        rb.linearVelocity = Vector3.zero;   // Unity 6 adı
        Debug.Log("Öldün! S karesinden yeniden başlıyorsun.");
    }
}