using System.Collections;
using UnityEngine;

// Kargo platformunda belirli bir bekleme süresi sonrasında yeni kargo üretir
public class Map6CargoSpawner : MonoBehaviour
{
    [Header("Kargo Üretim (Spawner) Özelliği")]
    [SerializeField] private Map6Cargo cargoPrefab = null;
    [SerializeField] private float delay = 4f;

    private Coroutine waiting;

    void Start()
    {
        Spawn();
    }

    void Update()
    {
        if (GetComponentInChildren<Map6Cargo>() != null) return;
        if (waiting == null)
        {
            waiting = StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        Spawn();
        waiting = null;
    }

    private void Spawn()
    {
        if (cargoPrefab == null) return;
        Instantiate(cargoPrefab, transform.position, transform.rotation, transform);
    }
}
