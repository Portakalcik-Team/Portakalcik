using UnityEngine;

public enum Map7_PropKind { Wall, Door, Exit }

// Duvar, kapı ve çıkış prefab'larının düz küp görünümünü gizler; yerine primitive şekillerden süslü bir görünüm kurar.
// Collider/trigger ve oyun mantığına dokunmaz; sadece görsel ekler.
public class Map7_PropLook : MonoBehaviour
{
    [SerializeField] private Map7_PropKind kind = Map7_PropKind.Wall;

    private Transform visual;
    private Transform orbs;      // çıkıştaki dönen küreler
    private Transform glow;      // çıkıştaki parlayan disk
    private float seed;

    void Awake()
    {
        seed = Random.value * 10f;
        if (transform.Find("Visual") == null) Build();
        else CacheParts();
    }

    // Editör aracı (Map7_BakeEnemies) bunu çağırarak görünümü prefab/sahneye gerçek alt objeler olarak kaydeder.
    [ContextMenu("Build Visual")]
    public void Build()
    {
        Transform old = transform.Find("Visual");
        if (old != null) DestroyObj(old.gameObject);

        // Eski küpü gizle (collider kalır)
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // Kök ölçeği eşit değil; ters ölçekle dengeliyoruz ki şekiller bozulmasın.
        GameObject v = new GameObject("Visual");
        visual = v.transform;
        visual.SetParent(transform, false);
        Vector3 s = transform.lossyScale;
        visual.localScale = new Vector3(1f / s.x, 1f / s.y, 1f / s.z);

        switch (kind)
        {
            case Map7_PropKind.Wall: BuildWall(); break;
            case Map7_PropKind.Door: BuildDoor(); break;
            case Map7_PropKind.Exit: BuildExit(); break;
        }
        CacheParts();
    }

    void CacheParts()
    {
        visual = transform.Find("Visual");
        orbs = visual != null ? visual.Find("Orbs") : null;
        glow = visual != null ? visual.Find("Glow") : null;
    }

    void Update()
    {
        float t = Time.time + seed;
        if (orbs != null) orbs.localRotation = Quaternion.Euler(0f, t * 50f, 0f);
        if (glow != null)
        {
            float p = 1f + Mathf.Sin(t * 3f) * 0.06f;   // nabız gibi parlama
            glow.localScale = new Vector3(2.2f * p, glow.localScale.y, 2.2f * p);
        }
    }

    // ---------- Tasarımlar (birimler metre) ----------

    // Duvar: 3x3x3 taş blok; kök merkezde, y -1.5..1.5
    void BuildWall()
    {
        Color stone = new Color(0.58f, 0.56f, 0.52f);
        Color band = new Color(0.46f, 0.44f, 0.4f);
        Color cap = new Color(0.7f, 0.68f, 0.62f);
        Color moss = new Color(0.3f, 0.5f, 0.22f);

        Part(PrimitiveType.Cube, "Stone", Vector3.zero, new Vector3(3f, 3f, 3f), stone, visual);
        // Tuğla sıralarını andıran yatay şeritler
        Part(PrimitiveType.Cube, "Band", new Vector3(0f, -0.5f, 0f), new Vector3(3.04f, 0.1f, 3.04f), band, visual);
        Part(PrimitiveType.Cube, "Band", new Vector3(0f, 0.5f, 0f), new Vector3(3.04f, 0.1f, 3.04f), band, visual);
        // Üst kapak ve alt kaide
        Part(PrimitiveType.Cube, "Cap", new Vector3(0f, 1.44f, 0f), new Vector3(3.08f, 0.16f, 3.08f), cap, visual);
        Part(PrimitiveType.Cube, "Base", new Vector3(0f, -1.42f, 0f), new Vector3(3.08f, 0.2f, 3.08f), band, visual);

        // Konuma göre rastgele ama her seferinde aynı yosun lekeleri
        System.Random rnd = new System.Random(Mathf.RoundToInt(transform.position.x * 73f + transform.position.z * 131f));
        Vector3[] normals = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        foreach (Vector3 n in normals)
        {
            if (rnd.NextDouble() > 0.5) continue;
            float along = (float)(rnd.NextDouble() * 1.6 - 0.8);
            Vector3 tangent = Vector3.Cross(Vector3.up, n);
            Vector3 pos = n * 1.52f + tangent * along + Vector3.up * (-1.1f + (float)rnd.NextDouble() * 0.5f);
            Transform m = Part(PrimitiveType.Sphere, "Moss", pos, new Vector3(0.9f, 0.3f, 0.9f), moss, visual);
            m.localRotation = Quaternion.LookRotation(n) * Quaternion.Euler(90f, 0f, 0f);
        }
    }

    // Kapı: taş çerçeve içinde ahşap kapı; kök z'de ince (3x3x0.5), y -1.5..1.5
    void BuildDoor()
    {
        Color frame = new Color(0.5f, 0.48f, 0.44f);
        Color wood = new Color(0.45f, 0.27f, 0.12f);
        Color plank = new Color(0.34f, 0.2f, 0.09f);
        Color iron = new Color(0.22f, 0.22f, 0.25f);
        Color gold = new Color(1f, 0.8f, 0.2f);

        // Taş çerçeve
        Part(PrimitiveType.Cube, "PostL", new Vector3(-1.3f, 0f, 0f), new Vector3(0.4f, 3f, 0.5f), frame, visual);
        Part(PrimitiveType.Cube, "PostR", new Vector3(1.3f, 0f, 0f), new Vector3(0.4f, 3f, 0.5f), frame, visual);
        Part(PrimitiveType.Cube, "Lintel", new Vector3(0f, 1.3f, 0f), new Vector3(3f, 0.4f, 0.55f), frame * 1.1f, visual);
        Part(PrimitiveType.Cube, "Threshold", new Vector3(0f, -1.45f, 0f), new Vector3(2.4f, 0.1f, 0.55f), frame * 0.9f, visual);

        // Ahşap kapı ve tahta çizgileri
        Part(PrimitiveType.Cube, "DoorBody", new Vector3(0f, -0.1f, 0f), new Vector3(2.2f, 2.7f, 0.28f), wood, visual);
        for (int i = -2; i <= 2; i++)
            Part(PrimitiveType.Cube, "Plank", new Vector3(i * 0.4f, -0.1f, 0.15f), new Vector3(0.05f, 2.7f, 0.02f), plank, visual);
        // Demir bantlar
        Part(PrimitiveType.Cube, "IronBand", new Vector3(0f, 0.7f, 0.16f), new Vector3(2.2f, 0.16f, 0.04f), iron, visual);
        Part(PrimitiveType.Cube, "IronBand", new Vector3(0f, -0.9f, 0.16f), new Vector3(2.2f, 0.16f, 0.04f), iron, visual);
        // Altın kulp (her iki yüzde)
        Part(PrimitiveType.Sphere, "Handle", new Vector3(0.7f, -0.1f, 0.2f), new Vector3(0.18f, 0.18f, 0.18f), gold, visual);
        Part(PrimitiveType.Sphere, "Handle", new Vector3(0.7f, -0.1f, -0.2f), new Vector3(0.18f, 0.18f, 0.18f), gold, visual);
    }

    // Çıkış: ince zemin plakası (3x0.1x3); parlayan disk ve dönen küreler
    void BuildExit()
    {
        Color stone = new Color(0.55f, 0.53f, 0.5f);
        Color glowColor = new Color(0.3f, 1f, 0.7f);
        Color orbColor = new Color(1f, 0.85f, 0.3f);

        Part(PrimitiveType.Cylinder, "Base", new Vector3(0f, 0f, 0f), new Vector3(2.7f, 0.03f, 2.7f), stone, visual);
        Transform g = Part(PrimitiveType.Cylinder, "Glow", new Vector3(0f, 0.06f, 0f), new Vector3(2.2f, 0.02f, 2.2f), glowColor, visual);
        g.name = "Glow";

        // Köşelerde taş sütunlar
        foreach (Vector2 c in new[] { new Vector2(-1.2f, -1.2f), new Vector2(1.2f, -1.2f), new Vector2(-1.2f, 1.2f), new Vector2(1.2f, 1.2f) })
            Part(PrimitiveType.Cylinder, "Pillar", new Vector3(c.x, 0.4f, c.y), new Vector3(0.22f, 0.4f, 0.22f), stone * 1.1f, visual);

        // Çıkışın üstünde dönen altın küreler
        orbs = new GameObject("Orbs").transform;
        orbs.SetParent(visual, false);
        orbs.localPosition = new Vector3(0f, 1.1f, 0f);
        for (int i = 0; i < 4; i++)
        {
            float a = i * Mathf.PI * 0.5f;
            Part(PrimitiveType.Sphere, "Orb", new Vector3(Mathf.Cos(a) * 0.7f, 0f, Mathf.Sin(a) * 0.7f), new Vector3(0.22f, 0.22f, 0.22f), orbColor, orbs);
        }
    }

    // ---------- Yardımcılar ----------

    static void DestroyObj(Object o)
    {
        if (Application.isPlaying) Destroy(o);
        else DestroyImmediate(o);
    }

    Transform Part(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Color color, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        DestroyObj(go.GetComponent<Collider>());   // tetik/fizik bozulmasın
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
            string path = dir + "/Prop_" + ColorUtility.ToHtmlStringRGB(color) + ".mat";
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
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.15f);
        return m;
    }
}
