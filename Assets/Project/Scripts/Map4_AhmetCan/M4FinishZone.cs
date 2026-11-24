using UnityEngine;                                                          // Unity'nin temel sınıfları

public class M4FinishZone : MonoBehaviour                                   // Çıkış bölgesine eklenen script; puanı teslim alır
{
    // skoru artık Round tutuyor
    [SerializeField] private M4Round round;                                 // Sahnedeki Round nesnesi (Inspector'dan atanır)

    void OnTriggerEnter(Collider other)                                     // Bir şey bu tetikleyici (trigger) alana girince çalışır
    {
        M4PlayerLoad load = other.GetComponent<M4PlayerLoad>();             // Giren nesnede yük taşıyıcı bileşen var mı diye bak
        if (load == null) return;                                           // Yoksa oyuncu değildir, hiçbir şey yapma

        int delivered = load.HandOver();                                    // Oyuncunun taşıdığı puanı al (oyuncunun yükü sıfırlanır)
        round.AddPoints(delivered);                                         // Teslim edilen puanı Round'a ver; HUD gösterir
    }
}
