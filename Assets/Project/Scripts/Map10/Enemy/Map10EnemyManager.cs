using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class Map10EnemyManager : MonoBehaviour
{
    void OnEnable()
    {
        EnsureMap();
        AttachEnemies();
    }

    void Start()
    {
        EnsureMap();
        AttachEnemies();
        if (Application.isPlaying)
        {
            StartCoroutine(SetupEnemiesDelayed());
        }
    }

    private void EnsureMap()
    {
        Map10TreeManager.EnsureMap();
    }

    private IEnumerator SetupEnemiesDelayed()
    {
        // MapGenerator'ın düşmanları instantiate etmesini beklemek için
        yield return null;
        AttachEnemies();

        yield return new WaitForSeconds(0.5f);
        AttachEnemies();
    }

    public static void AttachEnemies()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;

            string name = obj.name.ToLower();

            // Manager veya player objelerini kesinlikle atla
            if (name.Contains("manager") || name.Contains("player") || name.Contains("generator"))
                continue;

            // Canavar adlandırmalarına uyuyor mu?
            bool isEnemy = name.Contains("dikenbocegi") ||
                           name.Contains("diken") ||
                           name.Contains("golgeyarasa") ||
                           name.Contains("ormantilkisi") ||
                           name.Contains("kokgolemi") ||
                           name.Contains("labirentmuhafiz") ||
                           name.Contains("hazard");

            // Eğer GeneratedMap altındaki Enemies klasöründeyse
            if (obj.transform.parent != null && obj.transform.parent.name.ToLower().Contains("enemies"))
            {
                isEnemy = true;
            }

            // Path veya zemin objeleri hariç
            if (name.StartsWith("path_"))
            {
                isEnemy = false;
            }

            if (isEnemy)
            {
                Map10Enemy enemy = obj.GetComponent<Map10Enemy>();
                if (enemy == null)
                {
                    enemy = obj.AddComponent<Map10Enemy>();
                }
                else
                {
                    enemy.SetupEnemyVisuals();
                }
            }
        }
    }
}
