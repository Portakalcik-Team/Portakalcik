using UnityEngine;                                                          // Unity'nin temel sınıfları

public class M4FinishZone : MonoBehaviour                                   // Çıkış bölgesine eklenen script; puanı teslim alır
{
    private int score;                                                      // Teslim edilen toplam puan; her Play'de 0'dan başlar

    void OnTriggerEnter(Collider other)                                     // Bir şey bu tetikleyici (trigger) alana girince çalışır
    {
        M4PlayerLoad load = other.GetComponent<M4PlayerLoad>();             // Giren nesnede yük taşıyıcı bileşen var mı diye bak
        if (load == null) return;                                           // Yoksa oyuncu değildir, hiçbir şey yapma

        int delivered = load.HandOver();                                    // Oyuncunun taşıdığı puanı al (oyuncunun yükü sıfırlanır)
        score += delivered;                                                 // Teslim edilen puanı toplam skora ekle
        Debug.Log("Çıkışa Ulaştınız! +" + delivered + "  Skor: " + score);  // Konsola teslim edilen puanı ve toplam skoru yaz
    }
}
