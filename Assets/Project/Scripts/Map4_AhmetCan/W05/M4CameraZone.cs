using UnityEngine;                                                          // Unity'nin temel sınıfları

// Oyuncu bölgeye girince bölge kamerasını açar, çıkınca kapatır.
public class M4CameraZone : MonoBehaviour                                   // Kamera bölgesi tetikleyicisine eklenen script
{
    // bu bölgede açılacak Cinemachine Camera
    [SerializeField] private GameObject zoneCamera;                         // Bölgeye girince açılacak kamera nesnesi (Inspector'dan atanır)

    // Detect: bir şey bölgeye girdi
    void OnTriggerEnter(Collider other)                                     // Bir şey bölgeye girince çalışır
    {
        // Who? sadece "Player" etiketli nesne
        if (!other.CompareTag("Player")) return;                            // Etiketi Player değilse çık
        // Act: bölge kamerasını aç, canlı olur
        zoneCamera.SetActive(true);                                         // Kamerayı aç; Brain geçişi yumuşakça yapar
    }

    // Detect: bir şey bölgeden çıktı
    void OnTriggerExit(Collider other)                                      // Bir şey bölgeden çıkınca çalışır
    {
        if (!other.CompareTag("Player")) return;                            // Etiketi Player değilse çık
        // kapat: takip kamerası tekrar canlı
        zoneCamera.SetActive(false);                                        // Bölge kamerasını kapat; takip kamerası tekrar canlı olur
    }
}
