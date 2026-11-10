using UnityEngine;

public class Map0CargoSpawner : MonoBehaviour
{
    // ne kopyalanacak ve ped ne kadar boş kalacak
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private float delay = 4f;
    private float timer;   // sonraki kargoya kalan saniye

    void Update()
    {
        // pedde hâlâ bir kargo bekliyor mu? o zaman bir şey yapma
        if (GetComponentInChildren<Map0Cargo>() != null) return;

        // geri say
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // süre doldu: prefab'ın bir kopyası, burada, bizim çocuğumuz olarak
        Instantiate(cargoPrefab, transform.position, transform.rotation, transform);
        timer = delay;   // sonraki bekleme
    }
}
