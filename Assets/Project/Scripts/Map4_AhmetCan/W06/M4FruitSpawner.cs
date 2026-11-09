using UnityEngine;                                                          // Unity'nin temel sınıfları

// Boş pad: meyve alınınca bekler, süre dolunca yenisini koyar.
public class M4FruitSpawner : MonoBehaviour                                 // Pad nesnesine eklenen meyve üretici script
{
    // neyin kopyalanacağı ve pad'in ne kadar boş kalacağı
    [SerializeField] private GameObject fruitPrefab;                        // Kopyalanacak meyve prefab'ı
    [SerializeField] private float delay = 4f;                              // Meyve alındıktan sonra yenisinin gelme süresi (saniye)

    private float timer;   // sıradaki meyveye kalan saniye                  // Geri sayım sayacı; Play başında 0 (ilk meyve hemen çıkar)

    void Update()                                                           // Her karede çalışır
    {
        // prefab slotu boşsa her karede hata yağdırma
        if (fruitPrefab == null)                                            // Prefab atanmamışsa
        {
            Debug.LogError(name + ": Fruit Prefab yok");                    // Bir kez kırmızı hata yaz
            enabled = false;                                                // Bu scripti kapat, tekrar çalışmasın
            return;                                                         // Fonksiyondan çık
        }

        // pad'in üstünde meyve bekliyorsa bir şey yapma
        if (GetComponentInChildren<M4Fruit>() != null) return;              // Pad'in çocukları arasında meyve varsa bekliyordur, çık

        // geri say: her karede biraz daha az süre
        timer -= Time.deltaTime;                                            // Son kareden beri geçen süreyi sayaçtan çıkar
        if (timer > 0f) return;                                             // Süre dolmadıysa sonraki kareyi bekle

        // süre doldu: pad'in çocuğu olarak prefab'ın kopyası
        Instantiate(fruitPrefab, transform.position,                        // Prefab'ın kopyasını pad'in konumunda oluştur
                    transform.rotation, transform);                         // Pad'in dönüşüyle ve pad'in çocuğu olarak
        timer = delay;   // sıradaki bekleme bu kadar sürecek               // Sayacı yeniden kur; bir sonraki bekleme bu kadar sürer
    }
}
