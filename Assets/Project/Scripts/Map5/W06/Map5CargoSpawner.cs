using UnityEngine;

// W06: Ped (spawner). Üstünde kargo yoksa geri sayar, süre bitince yeni bir kargo yapar.
public class Map5CargoSpawner : MonoBehaviour
{
    // Neyi kopyalayacağız, ped ne kadar boş kalacak
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private float delay = 4f;

    private float timer;   // bir sonraki kargoya kalan saniye

    void Update()
    {
        // Pedin üstünde hâlâ bir kargo bekliyor mu? O zaman hiçbir şey yapma
        if (GetComponentInChildren<Map5Cargo>() != null) return;

        // Geri say: her karede biraz daha az zaman kalır
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // Süre bitti: prefab'ın bir kopyası, burada, bizim çocuğumuz olarak
        Instantiate(cargoPrefab, transform.position,
                    transform.rotation, transform);
        timer = delay;   // bir sonraki bekleme bu kadar sürecek
    }
}
