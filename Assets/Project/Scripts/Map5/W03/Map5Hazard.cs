using UnityEngine;

// Kırmızı tehlike: kargo taşıyan oyuncunun kargosunu düşürür.
public class Map5Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Kim? Kargo taşıyan oyuncu
        Map5PlayerCarry carry = other.GetComponent<Map5PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu düşür
        carry.DropCargo();
        Debug.Log(name + " kargoyu düşürdü!");
    }
}
