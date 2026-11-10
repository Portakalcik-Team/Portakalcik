using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Oyun başlayınca (MapGenerator haritayı kurduktan sonra) çalışır:
//  - tüm ağaçlara "normal" meyve verisini atar,
//  - DÜŞMANA EN YAKIN ağaçlardan birkaçını Altın Portakal ağacına çevirir (büyük ödül, bedeli: tehlike + yavaşlama),
//  - oyuncuya skor sistemini ekler.
// Harita yazısına ve MapGenerator'a dokunmaz.
public class Map7_LevelSetup : MonoBehaviour
{
    [SerializeField] private Map7_FruitData normalData;      // Map7_FD_Normal
    [SerializeField] private Map7_FruitData goldenData;      // Map7_FD_Golden
    [SerializeField] private int goldenTreeCount = 2;        // kaç ağaç Altın olsun
    [SerializeField] private float maxEnemyDistance = 12f;   // düşmana en fazla bu kadar yakın (metre) ağaçlar adaydır
    [SerializeField] private int goldenFruitCount = 3;       // Altın ağaçta kalan meyve sayısı (nadir)

    IEnumerator Start()
    {
        if (GetComponent<Map7_Score>() == null) gameObject.AddComponent<Map7_Score>();

        // MapGenerator ve ağaçların Start'ları bitsin diye iki kare bekle
        yield return null;
        yield return null;
        Setup();
    }

    void Setup()
    {
        Map7_FruitPickup[] all = FindObjectsByType<Map7_FruitPickup>(FindObjectsSortMode.None);
        Map7_EnemyLook[] enemies = FindObjectsByType<Map7_EnemyLook>(FindObjectsSortMode.None);

        // Gövdedeki kapalı kopyaları atla; sadece gerçek ağaçlar
        List<Map7_FruitPickup> trees = new List<Map7_FruitPickup>();
        foreach (Map7_FruitPickup p in all)
            if (p.enabled) trees.Add(p);

        // Hepsine normal veri
        foreach (Map7_FruitPickup t in trees)
            t.SetDataIfEmpty(normalData);

        if (goldenData == null || enemies.Length == 0)
        {
            Debug.LogWarning("[Map7] Altın ağaç yapılmadı: goldenData veya düşman yok.");
            return;
        }

        // Her ağacın en yakın düşmana uzaklığı (yatay düzlemde)
        List<KeyValuePair<float, Map7_FruitPickup>> ranked = new List<KeyValuePair<float, Map7_FruitPickup>>();
        foreach (Map7_FruitPickup t in trees)
        {
            float best = float.MaxValue;
            foreach (Map7_EnemyLook e in enemies)
            {
                Vector3 d = t.transform.position - e.transform.position;
                d.y = 0f;
                best = Mathf.Min(best, d.magnitude);
            }
            ranked.Add(new KeyValuePair<float, Map7_FruitPickup>(best, t));
        }
        ranked.Sort((a, b) => a.Key.CompareTo(b.Key));

        int made = 0;
        foreach (KeyValuePair<float, Map7_FruitPickup> r in ranked)
        {
            if (made >= goldenTreeCount) break;
            if (r.Key > maxEnemyDistance) break;   // sıralı olduğu için kalanlar da uzak

            r.Value.MakeGolden(goldenData, goldenFruitCount);
            Debug.Log("[Map7] Altın Portakal ağacı: " + r.Value.name + "  (en yakın düşmana " + r.Key.ToString("0.0") + " m)");
            made++;
        }
        if (made == 0) Debug.LogWarning("[Map7] Düşmana yakın ağaç bulunamadı: maxEnemyDistance'ı büyüt.");
    }
}
