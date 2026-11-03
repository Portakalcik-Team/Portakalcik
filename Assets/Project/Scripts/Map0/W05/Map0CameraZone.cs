using UnityEngine;

// Oyuncu bölgeye girince bölge kamerasını açar, çıkınca kapatır.
public class Map0CameraZone : MonoBehaviour
{
    [SerializeField] private GameObject zoneCamera; // Bu bölgede açılacak Cinemachine kamerası

    void OnTriggerEnter(Collider other)
    {
        // Detect: sadece oyuncuyu dikkate al
        if (!other.CompareTag("Player")) return;

        // Act: kamerayı aç, en son açılan canlı olur
        zoneCamera.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Kapat, takip kamerası yine canlı
        zoneCamera.SetActive(false);
    }
}
