using UnityEngine;

public class Map0DeliveryZone : MonoBehaviour
{
    private int score; // şimdiye kadarki puan

    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo taşıyan oyuncu
        Map0PlayerCarry carry = other.GetComponent<Map0PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu al ve sahneden sil
        Map0Cargo cargo = carry.HandOver();
        score += cargo.Points();
        Debug.Log("Teslim edildi! +" + cargo.Points() + "   Skor: " + score);
        Destroy(cargo.gameObject);
    }
}
