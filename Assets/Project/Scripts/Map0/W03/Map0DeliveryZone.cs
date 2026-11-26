using UnityEngine;

public class Map0DeliveryZone : MonoBehaviour
{
    [SerializeField] private Map0Round round; // puanı tutan tur

    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo taşıyan oyuncu
        Map0PlayerCarry carry = other.GetComponent<Map0PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu al ve sahneden sil
        Map0Cargo cargo = carry.HandOver();
        round.AddPoints(cargo.Points());
        Debug.Log("Teslim edildi! +" + cargo.Points());
        Destroy(cargo.gameObject);
    }
}
