using UnityEngine;

public class Map3_CargoSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cargoPrefab;
    [SerializeField] private float delay = 4f;
    private float timer;

    void Update()
    {
        if (GetComponentInChildren<M3Cargo>() != null) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Instantiate(cargoPrefab, transform.position, transform.rotation, transform);
        timer = delay;
    }
}
