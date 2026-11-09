using UnityEngine;

public class Map3_CameraZone : MonoBehaviour
{
    [SerializeField] private GameObject zoneCamera;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        zoneCamera.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        zoneCamera.SetActive(false);
    }
}
