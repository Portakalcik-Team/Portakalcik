using UnityEngine;

// Yeşil teslim alanı: kargo taşıyan oyuncu girince kargoyu alır.
// W06: kargonun puanını skora ekler.
public class Map5DeliveryZone : MonoBehaviour
{
    private int score;   // W06: şimdiye kadar teslim edilen puan

    void OnTriggerEnter(Collider other)
    {
        // Kim? Kargo taşıyan oyuncu
        Map5PlayerCarry carry = other.GetComponent<Map5PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu al, puanını ekle, sahneden sil
        Map5Cargo cargo = carry.HandOver();
        score += cargo.Points();
        Debug.Log("Teslim edildi! +" + cargo.Points() + "   Skor: " + score);
        Destroy(cargo.gameObject);
    }
}
