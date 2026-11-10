using UnityEngine;

// Kamera bölgesi (W05): oyuncu bölgeye girince zoneCamera açılır ve canlı olur, çıkınca kapanır ve CM_Follow geri döner.
// Cinemachine kodu yok: sadece bir nesneyi açıp kapatıyoruz.
public class Map7_CameraZone : MonoBehaviour
{
    // bu bölgede açılacak Cinemachine kamerası
    [SerializeField] private GameObject zoneCamera;

    // Rig'i kodla kuran script kullanır; elle kuruyorsan Inspector'dan CM_Overview'u sürükle.
    public void Configure(GameObject cam)
    {
        zoneCamera = cam;
    }

    // Algıla: bir şey bölgeye girdi
    void OnTriggerEnter(Collider other)
    {
        // Kim? sadece "Player" etiketli obje (düşen meyveler yok sayılır)
        if (!other.CompareTag("Player")) return;
        if (zoneCamera != null) zoneCamera.SetActive(true);
    }

    // Algıla: bir şey bölgeden çıktı
    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (zoneCamera != null) zoneCamera.SetActive(false);
    }
}
