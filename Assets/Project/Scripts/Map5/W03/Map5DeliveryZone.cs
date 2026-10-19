using UnityEngine;

// Yeşil teslim alanı: kargo taşıyan oyuncu girince kargoyu alır.
public class Map5DeliveryZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Kim? Kargo taşıyan oyuncu
        Map5PlayerCarry carry = other.GetComponent<Map5PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu al ve sahneden sil
        Map5Cargo cargo = carry.HandOver();
        Destroy(cargo.gameObject);
        Debug.Log("Teslim edildi!");
    }
}
