using UnityEngine;                                                          // Unity'nin temel sınıfları

// Yerden toplanan meyve: türü (hafif / ağır) bir veri dosyasından gelir.
public class M4Fruit : MonoBehaviour                                        // Toplanabilir meyve prefab'ına eklenen script
{
    [SerializeField] private M4FruitType type;   // FT_Light veya FT_Heavy    // Bu meyvenin türü (veri dosyası)

    void Awake()                                                            // Nesne oluşturulunca, Start'tan önce çalışır
    {
        // tür boşsa sessiz kalma, kırmızı yaz
        if (type == null) Debug.LogError(name + ": Meyve Türü yok");        // Tür atanmamışsa konsola kırmızı hata yaz
    }

    void OnTriggerEnter(Collider other)                                     // Bir şey meyvenin tetikleyici alanına girince çalışır
    {
        // Who? sadece yük taşıyabilen oyuncu
        M4PlayerLoad load = other.GetComponent<M4PlayerLoad>();             // Giren nesnede yük taşıyıcı var mı?
        if (load == null) return;                                           // Yoksa oyuncu değildir, çık

        // Act: yükü oyuncuya ver, meyveyi kaldır
        load.Add(type);                                                     // Meyvenin puanını ve ağırlığını oyuncuya ekle
        Destroy(gameObject);                                                // Meyveyi sahneden sil
    }
}
