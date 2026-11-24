using UnityEngine;
using Unity.Cinemachine;

// Oyuncu bu tetikleyici alana girdiğinde rampa/bölge kamerasını etkinleştirir
public class Map6CameraZone : MonoBehaviour
{
    [Header("Bölge Kamerası Özelliği")]
    [SerializeField] private CinemachineCamera zoneCamera = null; // Alana girildiğinde açılacak sanal kamera

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && zoneCamera != null)
        {
            zoneCamera.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && zoneCamera != null)
        {
            zoneCamera.gameObject.SetActive(false);
        }
    }
}
