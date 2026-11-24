using UnityEngine;

// Yeşil teslimat alanına kargo getirildiğinde kargoyu kabul eder
public class Map6DeliveryZone : MonoBehaviour
{
    [Header("Teslimat Puanı Özelliği")]
    [SerializeField] private int score = 0;

    void OnTriggerEnter(Collider other)
    {
        // Gelen nesnede veya ebeveyninde Map6PlayerCarry ara
        Map6PlayerCarry carry = other.GetComponent<Map6PlayerCarry>();
        if (carry == null) carry = other.GetComponentInParent<Map6PlayerCarry>();

        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Kargoyu teslim al, puanı ekle ve sahneden kaldır
        Map6Cargo cargo = carry.HandOver();
        if (cargo != null)
        {
            int points = cargo.Points();
            score += points;
            Destroy(cargo.gameObject);
            Debug.Log("Delivered! +" + points + " Score: " + score);
        }
    }
}
