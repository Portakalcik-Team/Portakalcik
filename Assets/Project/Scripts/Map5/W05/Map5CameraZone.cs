using UnityEngine;

// W05: Oyuncu bu alana girince alanın kamerasını açar, çıkınca kapatır.
// Cinemachine kodu yok: sadece bir objeyi açıp kapatıyoruz.
public class Map5CameraZone : MonoBehaviour
{
    // Bu alanda açılacak Cinemachine kamerası (CM_Rampa)
    [SerializeField] private GameObject zoneCamera;

    // Algıla: bir şey alana girdi
    void OnTriggerEnter(Collider other)
    {
        // Kim? Sadece "Player" etiketli oyuncu
        if (!other.CompareTag("Player")) return;
        // Yap: alan kamerasını aç, canlı kamera o olur
        zoneCamera.SetActive(true);
    }

    // Algıla: bir şey alandan çıktı
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        // Kapat: takip kamerası yeniden canlı olur
        zoneCamera.SetActive(false);
    }
}
