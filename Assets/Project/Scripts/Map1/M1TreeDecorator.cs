using System.Collections;
using UnityEngine;

// PLAYER'a (ya da herhangi bir objeye) ekle. Asset indirmeden ağaç görünümü yapar:
// Limon / Mandalina / Portakal adlı küp ağaçların yerine gövde + yaprak topu + meyveler koyar.
// Ağacın adı ve Box Collider'ı olduğu gibi kalır, diğer scriptler etkilenmez.
public class M1TreeDecorator : MonoBehaviour
{
    public string[] treePrefixes = { "limon", "mandalina", "portakal" };
    public string[] enemyPrefixes = { "diken", "yarasa", "tilki", "golem", "muhafiz" };

    public Color trunkColor = new Color(0.40f, 0.26f, 0.13f);
    public Color leafColor  = new Color(0.20f, 0.55f, 0.22f);

    public float trunkHeight = 1.3f;
    public float trunkRadius = 0.18f;
    public float crownSize = 1.8f;
    public int fruitCount = 7;
    public float fruitSize = 0.28f;

    IEnumerator Start()
    {
        yield return null;   // harita (varsa oyun başında kuruluyor) oluşsun diye bir kare bekle

        foreach (var tr in FindObjectsByType<Transform>(FindObjectsSortMode.None))
        {
            string n = tr.name.ToLower();

            // Düşmanlara da yuvarlak gövde + takip + saldırı ekle (M1Enemy)
            if (StartsWith(n, enemyPrefixes))
            {
                if (tr.GetComponent<Collider>() != null && tr.GetComponent<M1Enemy>() == null)
                    tr.gameObject.AddComponent<M1Enemy>();
                continue;
            }

            if (!StartsWith(n, treePrefixes)) continue;
            if (tr.Find("M1TreeVisual") != null) continue;   // iki kez yapma
            if (tr.GetComponent<Collider>() == null) continue;   // sadece collider'ı olan asıl ağaç objesi
            Decorate(tr, n);
        }
    }

    void Decorate(Transform tree, string lowerName)
    {
        Color fruitColor =
            lowerName.StartsWith("limon")    ? new Color(1f, 0.92f, 0.1f) :
            lowerName.StartsWith("portakal") ? new Color(1f, 0.40f, 0f) :
                                               new Color(1f, 0.62f, 0.1f);

        var mr = tree.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;                 // küp görünmesin, collider kalsın

        var col = tree.GetComponent<Collider>();
        float groundY = col.bounds.min.y;                   // ağacın yere bastığı yükseklik
        Vector3 c = col.bounds.center;

        var root = new GameObject("M1TreeVisual").transform;
        root.SetParent(tree, false);

        // Gövde (silindir, varsayılan boyu 2 birim)
        Part(root, "Govde", PrimitiveType.Cylinder,
             new Vector3(c.x, groundY + trunkHeight * 0.5f, c.z),
             new Vector3(trunkRadius * 2f, trunkHeight * 0.5f, trunkRadius * 2f), trunkColor);

        // Yaprak topu
        float crownY = groundY + trunkHeight + crownSize * 0.35f;
        Part(root, "Yaprak", PrimitiveType.Sphere,
             new Vector3(c.x, crownY, c.z),
             Vector3.one * crownSize, leafColor);

        // Meyveler: topun etrafına dağıt
        for (int i = 0; i < fruitCount; i++)
        {
            float a = i * Mathf.PI * 2f / fruitCount;
            float up = (i % 3 - 1) * 0.28f;
            Vector3 dir = new Vector3(Mathf.Cos(a), up, Mathf.Sin(a)).normalized;
            Vector3 pos = new Vector3(c.x, crownY, c.z) + dir * (crownSize * 0.5f);
            Part(root, "Meyve", PrimitiveType.Sphere, pos, Vector3.one * fruitSize, fruitColor);
        }
    }

    // worldPos / worldScale dünya birimi: ağacın kendi (düzgün olmayan) ölçeğini telafi eder
    void Part(Transform parent, string partName, PrimitiveType type, Vector3 worldPos, Vector3 worldScale, Color color)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = partName;
        var col = go.GetComponent<Collider>();
        if (col != null) Destroy(col);                      // parçalar çarpışma yapmasın

        var t = go.transform;
        t.SetParent(parent, false);
        t.position = worldPos;
        Vector3 ls = parent.lossyScale;
        t.localScale = new Vector3(
            ls.x != 0 ? worldScale.x / ls.x : worldScale.x,
            ls.y != 0 ? worldScale.y / ls.y : worldScale.y,
            ls.z != 0 ? worldScale.z / ls.z : worldScale.z);

        go.GetComponent<Renderer>().material.color = color;
    }

    bool StartsWith(string n, string[] prefixes)
    {
        foreach (var p in prefixes) if (n.StartsWith(p)) return true;
        return false;
    }
}
