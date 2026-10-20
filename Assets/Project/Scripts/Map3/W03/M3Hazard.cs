using UnityEngine;

// Map3 W03: kirmizi serit ve canavar. Kargo tasiyan oyuncunun kargosunu dusurur.
public class M3Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo tasiyan oyuncu
        M3PlayerCarry carry = other.GetComponent<M3PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Ne yap? kargoyu dusur
        carry.DropCargo();
    }
}
