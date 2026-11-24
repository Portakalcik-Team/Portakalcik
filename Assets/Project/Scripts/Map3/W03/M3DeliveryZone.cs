using UnityEngine;

// Map3 W03: yesil teslim alani. Kargo getiren oyuncudan kargoyu alir.
// W07: skoru artik M3Round tutar (HUD'da gorunur).
public class M3DeliveryZone : MonoBehaviour
{
    [SerializeField] private M3Round round;
    // bu puana ulasinca bolum biter (Inspector'da Target Score)
    [SerializeField] private int targetScore = 10;

    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo tasiyan oyuncu
        M3PlayerCarry carry = other.GetComponent<M3PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Ne yap? kargoyu al, puani tura ekle, kargoyu yok et
        M3Cargo cargo = carry.HandOver();
        round.AddPoints(cargo.Points());
        Destroy(cargo.gameObject);

        // yeterli puan toplandi mi? bolum tamamlandi
        if (round.Score() >= targetScore) round.EndRound("Tebrikler!");
    }
}
