using UnityEngine;
using Unity.Cinemachine;

// Tehlike alanına veya canavara çarpıldığında oyuncunun kargosunu düşürür ve (varsa) hasar verir
public class Map6Hazard : MonoBehaviour
{
    [Header("Hasar Özelliği")]
    [SerializeField] private int damage = 0;

    [Header("Kamera Sarsıntısı (Impulse) Özelliği")]
    [SerializeField] private CinemachineImpulseSource shake = null;

    void Start()
    {
        if (shake == null) shake = GetComponent<CinemachineImpulseSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        bool hitSomething = false;

        // Kargo taşıyorsa düşür
        Map6PlayerCarry carry = other.GetComponent<Map6PlayerCarry>();
        if (carry == null) carry = other.GetComponentInParent<Map6PlayerCarry>();

        if (carry != null && carry.IsCarrying())
        {
            carry.DropCargo();
            hitSomething = true;
            Debug.Log(name + " kargoyu düşürdü!");
        }

        // Hasar tanımlıysa (canavarlar) oyuncunun canından düş
        if (damage > 0)
        {
            Map6Health health = other.GetComponent<Map6Health>();
            if (health == null) health = other.GetComponentInParent<Map6Health>();

            if (health != null)
            {
                health.TakeDamage(damage);
                hitSomething = true;
            }
        }

        if (hitSomething && shake != null)
        {
            shake.GenerateImpulse();
        }
    }
}
