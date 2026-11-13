using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Map10TreeManager : MonoBehaviour
{
    private static string lastSyncedMapText = "";

    void Awake()
    {
        Transform tv = transform.Find("TreeVisuals");
        if (tv != null)
        {
            if (Application.isPlaying) Destroy(tv.gameObject);
            else DestroyImmediate(tv.gameObject);
        }
        Map10Tree t = GetComponent<Map10Tree>();
        if (t != null)
        {
            if (Application.isPlaying) Destroy(t);
            else DestroyImmediate(t);
        }
    }

    void OnEnable()
    {
        EnsureMap();
        AttachTrees();
    }

    void Start()
    {
        EnsureMap();
        AttachTrees();
        if (Application.isPlaying)
        {
            StartCoroutine(SetupTreesDelayed());
        }
    }

    void Update()
    {
        // Edit modunda harita güncel değilse anında oluştur
        if (!Application.isPlaying)
        {
            MapGenerator gen = FindFirstObjectByType<MapGenerator>();
            if (gen != null && gen.mapText != lastSyncedMapText)
            {
                EnsureMap(true);
            }
        }
    }

    [ContextMenu("Haritayı Yeniden Üret (Force Generate)")]
    public void ForceRegenerate()
    {
        EnsureMap(true);
    }

    public static void EnsureMap(bool force = false)
    {
        MapGenerator gen = FindFirstObjectByType<MapGenerator>();
        if (gen == null || string.IsNullOrEmpty(gen.mapText)) return;

        GameObject genMap = GameObject.Find("GeneratedMap");
        Transform walls = genMap != null ? genMap.transform.Find("Walls") : null;
        int wallCount = walls != null ? walls.childCount : 0;

        bool needsRegen = force || (lastSyncedMapText != gen.mapText) || (genMap == null) || (wallCount < 200);

        if (needsRegen)
        {
            // Eski GeneratedMap objesini sahneden kaldır
            if (genMap != null)
            {
                if (Application.isPlaying) Destroy(genMap);
                else DestroyImmediate(genMap);
            }

            gen.GenerateMap();
            lastSyncedMapText = gen.mapText;

            // Ağaçları ve canavarları dönüştür
            AttachTrees();
            Map10EnemyManager.AttachEnemies();

            Debug.Log($"[Map10TreeManager] ✅ Güncel 21x21 harita başarıyla sahneye yerleştirildi! Duvar: {wallCount}");
        }
    }

    private IEnumerator SetupTreesDelayed()
    {
        yield return null;
        AttachTrees();

        yield return new WaitForSeconds(0.5f);
        AttachTrees();
    }

    public static void AttachTrees()
    {
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            if (obj == null) continue;

            string name = obj.name.ToLower();

            // Manager veya player objelerini kesinlikle atla
            if (name.Contains("manager") || name.Contains("player") || name.Contains("generator"))
                continue;

            // Ağaç adlandırmalarına uyuyor mu?
            bool isTree = name.Contains("mandalina") ||
                          name.Contains("tree") ||
                          name.Contains("portakal") ||
                          name.Contains("limon") ||
                          obj.GetComponent("FruitTree") != null;

            // Eğer GeneratedMap altındaki Trees klasöründeyse veya ağaçsa
            if (obj.transform.parent != null && obj.transform.parent.name.ToLower().Contains("trees"))
            {
                isTree = true;
            }

            // Path veya zemin objeleri hariç
            if (name.StartsWith("path_"))
            {
                isTree = false;
            }

            if (isTree)
            {
                if (obj.GetComponent<Map10Tree>() == null)
                {
                    obj.AddComponent<Map10Tree>();
                }
            }
        }
    }
}
