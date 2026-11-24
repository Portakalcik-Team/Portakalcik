using UnityEngine;

// Map3 W03: yesil teslim alani. Kargo getiren oyuncudan kargoyu alir.
public class M3DeliveryZone : MonoBehaviour
{
    private int score;
    // bu puana ulasinca bolum biter (Inspector'da Target Score)
    [SerializeField] private int targetScore = 10;

    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo tasiyan oyuncu
        M3PlayerCarry carry = other.GetComponent<M3PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Ne yap? kargoyu al ve yok et
        M3Cargo cargo = carry.HandOver();
        score += cargo.Points();
        Debug.Log("Delivered! +" + cargo.Points() + "   Score: " + score);
        Destroy(cargo.gameObject);

        // yeterli puan toplandi mi? bolum tamamlandi
        if (score >= targetScore) Debug.Log("Bölüm tamamlandı!");
    }
}
