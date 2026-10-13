using UnityEngine;

// Map_3: W03 slayttaki Hazard. Kasa tasiyan oyuncunun kasasini dusurur.
// "Ah!" mesajini ayni nesnedeki Map3PlayerTrigger yazar; burada tekrar yazilmaz.
public class Hazard : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Who? kasa tasiyan oyuncu
        PlayerCarry carry = other.GetComponentInParent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Act: kasayi dusur
        carry.DropCargo();
        Debug.Log(name + " kasayı düşürdü!");
    }
}
