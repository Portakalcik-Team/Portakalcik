using UnityEngine;

// Map 7 oyuncusunu labirent temasına uygun bir "Labirent Kaşifi" olarak kurar:
// turuncu ceket, kaşif şapkası, sırt çantası (içinde meyve), elinde fener.
// Düz capsule görünümünü gizler. Collider, Rigidbody ve hareket scriptlerine dokunmaz.
public class Map7_PlayerLook : MonoBehaviour
{
    private Transform visual;
    private Transform legL, legR, armL, armR;
    private Light lantern;
    private Rigidbody rb;
    private float seed;
    private float walkPhase;

    void Awake()
    {
        seed = Random.value * 10f;
        rb = GetComponent<Rigidbody>();
        if (transform.Find("Visual") == null) Build();
        else CacheParts();
    }

    // Editör aracı (Map7_BakeEnemies) bunu çağırarak kaşifi prefab'a gerçek alt objeler olarak kaydeder.
    [ContextMenu("Build Visual")]
    public void Build()
    {
        Transform old = transform.Find("Visual");
        if (old != null) DestroyObj(old.gameObject);

        // Eski capsule görünümünü gizle (collider kalır)
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        GameObject v = new GameObject("Visual");
        visual = v.transform;
        visual.SetParent(transform, false);
        Vector3 s = transform.lossyScale;
        visual.localScale = new Vector3(1f / s.x, 1f / s.y, 1f / s.z);

        Color jacket = new Color(1f, 0.5f, 0.1f);       // portakal turuncusu ceket
        Color pants = new Color(0.25f, 0.28f, 0.38f);
        Color boots = new Color(0.25f, 0.15f, 0.08f);
        Color skin = new Color(0.95f, 0.78f, 0.62f);
        Color hat = new Color(0.45f, 0.3f, 0.15f);
        Color leather = new Color(0.4f, 0.26f, 0.14f);
        Color eye = new Color(0.1f, 0.1f, 0.12f);
        Color glow = new Color(1f, 0.85f, 0.4f);

        // Gövde ve baş (origin = capsule merkezi; boy -1..+1)
        Part(PrimitiveType.Capsule, "Torso", new Vector3(0f, -0.05f, 0f), new Vector3(0.62f, 0.4f, 0.5f), jacket, visual);
        Part(PrimitiveType.Cube, "Belt", new Vector3(0f, -0.33f, 0f), new Vector3(0.64f, 0.08f, 0.52f), leather, visual);
        Part(PrimitiveType.Sphere, "Head", new Vector3(0f, 0.52f, 0f), new Vector3(0.48f, 0.5f, 0.48f), skin, visual);
        Part(PrimitiveType.Sphere, "EyeL", new Vector3(-0.11f, 0.56f, 0.21f), new Vector3(0.07f, 0.09f, 0.07f), eye, visual);
        Part(PrimitiveType.Sphere, "EyeR", new Vector3(0.11f, 0.56f, 0.21f), new Vector3(0.07f, 0.09f, 0.07f), eye, visual);
        // Kaşif şapkası
        Part(PrimitiveType.Sphere, "HatTop", new Vector3(0f, 0.68f, 0f), new Vector3(0.52f, 0.3f, 0.52f), hat, visual);
        Part(PrimitiveType.Cylinder, "HatBrim", new Vector3(0f, 0.62f, 0f), new Vector3(0.82f, 0.015f, 0.82f), hat * 0.9f, visual);
        // Atkı
        Part(PrimitiveType.Cylinder, "Scarf", new Vector3(0f, 0.27f, 0f), new Vector3(0.42f, 0.05f, 0.42f), new Color(0.85f, 0.2f, 0.15f), visual);

        // Sırt çantası: içinden bir portakal görünür
        Part(PrimitiveType.Cube, "Backpack", new Vector3(0f, 0f, -0.36f), new Vector3(0.5f, 0.55f, 0.25f), leather, visual);
        Part(PrimitiveType.Cube, "BackpackFlap", new Vector3(0f, 0.22f, -0.37f), new Vector3(0.52f, 0.12f, 0.27f), leather * 0.8f, visual);
        Part(PrimitiveType.Sphere, "BagFruit", new Vector3(0.1f, 0.34f, -0.36f), new Vector3(0.2f, 0.2f, 0.2f), jacket, visual);

        // Bacaklar (pivot kalçada, yürüyüşte sallanır)
        legL = Pivot("LegL", new Vector3(-0.16f, -0.38f, 0f));
        legR = Pivot("LegR", new Vector3(0.16f, -0.38f, 0f));
        foreach (Transform leg in new[] { legL, legR })
        {
            Part(PrimitiveType.Cylinder, "Leg", new Vector3(0f, -0.25f, 0f), new Vector3(0.22f, 0.25f, 0.22f), pants, leg);
            Part(PrimitiveType.Cube, "Boot", new Vector3(0f, -0.55f, 0.04f), new Vector3(0.24f, 0.14f, 0.34f), boots, leg);
        }

        // Kollar (pivot omuzda); sağ kol fenerli
        armL = Pivot("ArmL", new Vector3(-0.42f, 0.18f, 0f));
        armR = Pivot("ArmR", new Vector3(0.42f, 0.18f, 0f));
        foreach (Transform arm in new[] { armL, armR })
        {
            Part(PrimitiveType.Capsule, "Arm", new Vector3(0f, -0.22f, 0f), new Vector3(0.16f, 0.22f, 0.16f), jacket, arm);
            Part(PrimitiveType.Sphere, "Hand", new Vector3(0f, -0.46f, 0f), new Vector3(0.14f, 0.14f, 0.14f), skin, arm);
        }

        // Fener: labirentin karanlığında yolu aydınlatır
        Transform lan = Pivot("Lantern", new Vector3(0.1f, -0.52f, 0.12f));
        lan.SetParent(armR, false);
        lan.localPosition = new Vector3(0f, -0.58f, 0.1f);
        Part(PrimitiveType.Cylinder, "LanternBody", Vector3.zero, new Vector3(0.14f, 0.1f, 0.14f), new Color(0.2f, 0.2f, 0.22f), lan);
        Part(PrimitiveType.Sphere, "LanternGlow", new Vector3(0f, 0.02f, 0f), new Vector3(0.16f, 0.16f, 0.16f), glow, lan);
        Light l = lan.gameObject.AddComponent<Light>();
        l.type = LightType.Point;
        l.color = new Color(1f, 0.8f, 0.5f);
        l.range = 9f;
        l.intensity = 2.2f;
        l.shadows = LightShadows.None;

        CacheParts();
    }

    void CacheParts()
    {
        visual = transform.Find("Visual");
        legL = FindPart("LegL"); legR = FindPart("LegR");
        armL = FindPart("ArmL"); armR = FindPart("ArmR");
        Transform lan = FindPart("Lantern");
        lantern = lan != null ? lan.GetComponent<Light>() : null;
    }

    Transform FindPart(string partName)
    {
        if (visual == null) return null;
        foreach (Transform t in visual.GetComponentsInChildren<Transform>(true))
            if (t.name == partName) return t;
        return null;
    }

    void Update()
    {
        // Yürüme hızına göre bacak/kol sallanması
        float speed = 0f;
        if (rb != null)
        {
            Vector3 v = rb.linearVelocity;
            speed = new Vector3(v.x, 0f, v.z).magnitude;
        }
        float moving = Mathf.Clamp01(speed / 3f);
        walkPhase += speed * 2.2f * Time.deltaTime;
        float swing = Mathf.Sin(walkPhase) * 35f * moving;

        if (legL != null) legL.localRotation = Quaternion.Euler(swing, 0f, 0f);
        if (legR != null) legR.localRotation = Quaternion.Euler(-swing, 0f, 0f);
        if (armL != null) armL.localRotation = Quaternion.Euler(-swing * 0.8f, 0f, 0f);
        if (armR != null) armR.localRotation = Quaternion.Euler(swing * 0.4f, 0f, 0f);   // fener kolu az sallanır

        // Fener hafifçe titrer
        if (lantern != null)
            lantern.intensity = 2.2f + Mathf.Sin((Time.time + seed) * 9f) * 0.12f + Mathf.Sin((Time.time + seed) * 23f) * 0.08f;
    }

    // ---------- Yardımcılar ----------

    static void DestroyObj(Object o)
    {
        if (Application.isPlaying) Destroy(o);
        else DestroyImmediate(o);
    }

    Transform Pivot(string name, Vector3 pos)
    {
        Transform p = new GameObject(name).transform;
        p.SetParent(visual, false);
        p.localPosition = pos;
        return p;
    }

    Transform Part(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Color color, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        DestroyObj(go.GetComponent<Collider>());   // oyuncunun fiziğini bozmasın
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = MakeMaterial(color);
        return go.transform;
    }

    Material MakeMaterial(Color color)
    {
#if UNITY_EDITOR
        // Editörde: renge göre .mat dosyası oluştur/kullan (prefab'a kaydedilebilsin diye)
        if (!Application.isPlaying)
        {
            const string dir = "Assets/Project/Art/Map7_Enemies";
            if (!UnityEditor.AssetDatabase.IsValidFolder(dir))
                UnityEditor.AssetDatabase.CreateFolder("Assets/Project/Art", "Map7_Enemies");
            string path = dir + "/Player_" + ColorUtility.ToHtmlStringRGB(color) + ".mat";
            Material existing = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null) return existing;
            Material created = NewMaterial(color);
            UnityEditor.AssetDatabase.CreateAsset(created, path);
            return created;
        }
#endif
        return NewMaterial(color);
    }

    static Material NewMaterial(Color color)
    {
        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        Material m = new Material(sh);
        m.color = color;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.3f);
        return m;
    }
}
