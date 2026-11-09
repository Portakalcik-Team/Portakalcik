using UnityEngine;                                                          // Unity'nin temel sınıfları

// Oyuncunun taşıdığı meyve yükü: puan biriktirir, ağırlık kütleyi artırır.
public class M4PlayerLoad : MonoBehaviour                                   // Oyuncuya eklenen yük taşıma scripti
{
    private Rigidbody rb;                                                   // Oyuncunun fizik bedeni
    private float baseMass;   // eller boşken kütle                         // Yük yokken oyuncunun kütlesi
    private int points;       // taşınan puan                               // Şu an taşınan toplam puan
    private float weight;     // taşınan ağırlık                            // Şu an taşınan toplam ağırlık

    void Awake()                                                            // Nesne oluşturulunca, Start'tan önce çalışır
    {
        rb = GetComponent<Rigidbody>();                                     // Kendi Rigidbody bileşenini bul
        baseMass = rb.mass;                                                 // Başlangıç kütlesini hatırla
    }

    public void Add(M4FruitType type)                                       // Bir meyve toplanınca meyve çağırır
    {
        points += type.Points();                                            // Meyvenin puanını taşınan puana ekle
        weight += type.Weight();                                            // Meyvenin ağırlığını taşınan ağırlığa ekle
        rb.mass = baseMass + weight;   // ağır yük = daha zayıf zıplama     // Kütleyi artır; aynı zıplama gücü daha az yükseltir
        Debug.Log("Toplandı! +" + type.Points() + "  Taşınan: " + points + "  Kütle: " + rb.mass); // Konsola toplanan puanı, taşınanı ve kütleyi yaz
    }

    // tehlike çarpınca: taşınan her şey düşer
    public void DropAll()                                                   // Tehlike (diken böceği) çağırır
    {
        if (points == 0) return;                                            // Taşınan bir şey yoksa yapacak şey yok
        Debug.Log("Yük düştü! -" + points);                                 // Konsola kaybedilen puanı yaz
        Clear();                                                            // Yükü sıfırla
    }

    // çıkış bölgesi bunu çağırır: puanı teslim eder
    public int HandOver()                                                   // Çıkış bölgesi çağırır
    {
        int delivered = points;                                             // Teslim edilecek puanı kaydet
        Clear();                                                            // Yükü sıfırla (eller boşalır)
        return delivered;                                                   // Teslim edilen puanı çağırana ver
    }

    private void Clear()                                                    // Yükü sıfırlayan yardımcı fonksiyon
    {
        points = 0;                                                         // Taşınan puanı sıfırla
        weight = 0f;                                                        // Taşınan ağırlığı sıfırla
        rb.mass = baseMass;   // yine hafif                                 // Kütleyi başlangıç değerine döndür
    }
}
