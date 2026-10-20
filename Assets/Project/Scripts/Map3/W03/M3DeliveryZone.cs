using UnityEngine;

// Map3 W03: yesil teslim alani. Kargo getiren oyuncudan kargoyu alir.
public class M3DeliveryZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo tasiyan oyuncu
        M3PlayerCarry carry = other.GetComponent<M3PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Ne yap? kargoyu al ve yok et
        M3Cargo cargo = carry.HandOver();
        Destroy(cargo.gameObject);
        Debug.Log("Teslim edildi!");
    }
}
