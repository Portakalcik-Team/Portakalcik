using UnityEngine;

// Tehlike alanına veya engeline çarpıldığında oyuncunun kargosunu düşürür
public class Map6Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Gelen nesnede veya ebeveyninde Map6PlayerCarry ara
        Map6PlayerCarry carry = other.GetComponent<Map6PlayerCarry>();
        if (carry == null) carry = other.GetComponentInParent<Map6PlayerCarry>();

        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Kargoyu düşür ve konsola bildir
        carry.DropCargo();
        Debug.Log(name + " kargoyu düşürdü!");
    }
}
