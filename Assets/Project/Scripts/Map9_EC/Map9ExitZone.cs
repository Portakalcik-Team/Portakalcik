using UnityEngine;

// Seviye 9: çıkış — portakal teslim edilir, hedef skora ulaşınca seviye biter
public class Map9ExitZone : MonoBehaviour
{
    [SerializeField] private int targetScore = 5;

    private int score;
    private bool completed;   // true = bitiş mesajı yazıldı

    void OnTriggerEnter(Collider other)
    {
        // Who? sadece eli olan: oyuncu
        Map9Carry carry = other.GetComponent<Map9Carry>();
        if (carry == null) return;

        // portakal yoksa çıkış kilitli
        if (!carry.IsCarrying())
        {
            Debug.Log("Çıkış kilitli: Portakal_01'deki portakalı getir!");
            return;
        }

        // Act: portakalı teslim al, puanı ekle
        Map9Portakal portakal = carry.HandOver();
        int puan = portakal.Points();
        score += puan;
        Debug.Log("Portakal teslim edildi! +" + puan + "  Skor: " + score + "/" + targetScore);

        // hedefe ulaşıldı: bir kez yaz
        if (score >= targetScore && !completed)
        {
            completed = true;
            Debug.Log("Seviye 9 tamamlandı!");

            // çantada ne birikti?
            Map9Bag bag = other.GetComponent<Map9Bag>();
            if (bag != null) Debug.Log("Çantada: " + bag.Summary());
        }

        // en sonda sahneden kaldır
        Destroy(portakal.gameObject);
    }
}