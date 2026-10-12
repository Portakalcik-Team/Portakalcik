using UnityEngine;

// Map_3: W03 slayttaki DeliveryZone. Yesil alana tasinan kasayi teslim alir.
// "Bölüm tamamlandı!" mesajini ayni nesnedeki Map3PlayerTrigger yazar; ikisi birbirini engellemez.
public class DeliveryZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Who? kasa tasiyan oyuncu (bos elle giris teslimat sayilmaz)
        PlayerCarry carry = other.GetComponentInParent<PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Act: kasayi al ve kaldir
        Cargo cargo = carry.HandOver();
        if (cargo == null) return;
        cargo.MarkDelivered();
        cargo.gameObject.SetActive(false);   // ayni karede ikinci kez sayilmasin
        Destroy(cargo.gameObject);

        Debug.Log("Teslim edildi!");
    }
}
