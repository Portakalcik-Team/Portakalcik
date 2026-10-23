using UnityEngine;

public class Map0Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo taşıyan oyuncu
        Map0PlayerCarry carry = other.GetComponent<Map0PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu düşür
        carry.DropCargo();
        Debug.Log(name + " kargoyu düşürdü!");
    }
}
