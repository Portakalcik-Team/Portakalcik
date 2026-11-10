using UnityEngine;

// Seviye 9: yerindeki portakal alınınca gecikmeyle yenisini üretir
public class Map9PortakalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject portakalPrefab;
    [SerializeField] private float delay = 4f;   // saniye

    private float timer;

    void Update()
    {
        // portakal hâlâ çocuğumsa bekle (alınınca CarryPoint'in çocuğu olur)
        if (GetComponentInChildren<Map9Portakal>() != null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Instantiate(portakalPrefab, transform.position, transform.rotation, transform);
        timer = delay;
    }
}
