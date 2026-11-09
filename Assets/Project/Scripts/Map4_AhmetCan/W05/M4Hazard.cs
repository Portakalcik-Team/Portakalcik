using UnityEngine;                                                          // Unity'nin temel sınıfları
using Unity.Cinemachine;                                                    // Cinemachine sınıfları (CinemachineImpulseSource için)

// Diken böceği oyuncuya değince kamerayı kısa bir an sarsar.
public class M4Hazard : MonoBehaviour                                       // Tehlikeye (diken böceği) eklenen script
{
    private CinemachineImpulseSource shake;   // null = sarsıntı yok        // Bu tehlikedeki sarsıntı kaynağı; yoksa null

    void Start()                                                            // Oyun başlayınca bir kez çalışır
    {
        // bu böcekteki Impulse Source, varsa
        shake = GetComponent<CinemachineImpulseSource>();                   // Aynı nesnedeki Impulse Source bileşenini bul
    }

    void OnTriggerEnter(Collider other)                                     // Bir şey tehlikenin tetikleyici alanına girince çalışır
    {
        // Who? sadece oyuncu (M4PlayerMove taşıyan)
        if (other.GetComponent<M4PlayerMove>() == null) return;            // Hareket scripti yoksa oyuncu değildir, çık

        // Act: taşınan yük düşer ve kameraya kısa bir tosluk ver
        Debug.Log(name + " oyuncuya çarptı!");                              // Konsola hangi tehlikenin çarptığını yaz
        M4PlayerLoad load = other.GetComponent<M4PlayerLoad>();             // Oyuncunun yük taşıyıcısını bul
        if (load != null) load.DropAll();                                   // Varsa taşıdığı tüm yükü düşür
        if (shake != null) shake.GenerateImpulse();                         // Sarsıntı kaynağı varsa kameraya tek bir darbe gönder
    }
}
