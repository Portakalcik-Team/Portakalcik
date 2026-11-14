using UnityEngine;

public class Map0DeliveryZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo taşıyan oyuncu
        Map0PlayerCarry carry = other.GetComponent<Map0PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu al ve sahneden sil
        Map0Cargo cargo = carry.HandOver();
        Destroy(cargo.gameObject);
        Debug.Log("Teslim edildi!");
    }
}
