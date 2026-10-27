using UnityEngine;

// Yeşil teslimat alanına kargo getirildiğinde kargoyu kabul eder
public class Map6DeliveryZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Gelen nesnede veya ebeveyninde Map6PlayerCarry ara
        Map6PlayerCarry carry = other.GetComponent<Map6PlayerCarry>();
        if (carry == null) carry = other.GetComponentInParent<Map6PlayerCarry>();

        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Kargoyu teslim al ve sahneden kaldır
        Map6Cargo cargo = carry.HandOver();
        if (cargo != null)
        {
            Destroy(cargo.gameObject);
            Debug.Log("Teslim edildi!");
        }
    }
}
