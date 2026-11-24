using UnityEngine;

// Meyve ağacının düz küp görünümünü gizler; yerine gövde, yapraklı taç ve meyvelerden oluşan bir ağaç kurar.
// Meyveler "Visual/Crown/Fruits" altında durur; toplanınca Map7_FruitPickup bu grubu siler.
// Collider/trigger ve toplama scriptlerine dokunmaz.
public class Map7_TreeLook : MonoBehaviour
{
    [SerializeField] private Map7_FruitType fruitType = Map7_FruitType.Limon;

    private Transform visual;
    private Transform crown;
    private float seed;

    void Awake()
    {
        seed = Random.value * 10f;
        if (transform.Find("Visual") == null) Build();
        else CacheParts();
    }

    // Editör aracı (Map7_BakeEnemies) bunu çağırarak ağacı prefab/sahneye gerçek alt objeler olarak kaydeder.
    [ContextMenu("Build Visual")]
    public void Build()
    {
        Transform old = transform.Find("Visual");
        if (old != null) DestroyObj(old.gameObject);

        // Eski küpleri gizle (collider kalır)
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // Kök ölçeği eşit değil; ters ölçekle dengeliyoruz ki şekiller bozulmasın.
        GameObject v = new GameObject("Visual");
        visual = v.transform;
        visual.SetParent(transform, false);
        Vector3 s = transform.lossyScale;
        visual.localScale = new Vector3(1f / s.x, 1f / s.y, 1f / s.z);

        Color trunkColor = new Color(0.4f, 0.26f, 0.14f);
        Color leafDark, leafLight, fruitColor;
        Vector3 fruitScale;
        switch (fruitType)
        {
            case Map7_FruitType.Limon:
                leafDark = new Color(0.35f, 0.62f, 0.2f); leafLight = new Color(0.5f, 0.75f, 0.25f);
                fruitColor = new Color(1f, 0.92f, 0.15f); fruitScale = new Vector3(0.2f, 0.27f, 0.2f); break;
            case Map7_FruitType.Mandalina:
                leafDark = new Color(0.2f, 0.5f, 0.2f); leafLight = new Color(0.32f, 0.65f, 0.25f);
                fruitColor = new Color(1f, 0.6f, 0.1f); fruitScale = new Vector3(0.24f, 0.2f, 0.24f); break;
            default: // Portakal
                leafDark = new Color(0.12f, 0.42f, 0.18f); leafLight = new Color(0.2f, 0.55f, 0.22f);
                fruitColor = new Color(1f, 0.42f, 0f); fruitScale = new Vector3(0.28f, 0.28f, 0.28f); break;
        }

        const float baseY = -0.45f;   // zemin hizası
        // Gövde
        Part(PrimitiveType.Cylinder, "Trunk_Visual", new Vector3(0f, baseY + 0.4f, 0f), new Vector3(0.3f, 0.4f, 0.3f), trunkColor, visual);
        Part(PrimitiveType.Cylinder, "RootFlare", new Vector3(0f, baseY + 0.05f, 0f), new Vector3(0.42f, 0.05f, 0.42f), trunkColor * 0.85f, visual);

        // Yapraklı taç (hafifçe sallanır)
        crown = new GameObject("Crown").transform;
        crown.SetParent(visual, false);
        crown.localPosition = new Vector3(0f, baseY + 0.75f, 0f);
        Part(PrimitiveType.Sphere, "Leaves", new Vector3(0f, 0.3f, 0f), new Vector3(1.3f, 1.1f, 1.3f), leafDark, crown);
        Part(PrimitiveType.Sphere, "Leaves", new Vector3(-0.5f, 0.1f, 0.1f), new Vector3(0.9f, 0.8f, 0.9f), leafLight, crown);
        Part(PrimitiveType.Sphere, "Leaves", new Vector3(0.5f, 0.15f, -0.1f), new Vector3(0.95f, 0.85f, 0.95f), leafDark, crown);
        Part(PrimitiveType.Sphere, "Leaves", new Vector3(0.05f, 0.75f, 0.05f), new Vector3(0.8f, 0.7f, 0.8f), leafLight, crown);

        // Meyveler: yaprak yüzeyine dağılmış, hepsi tek grupta (toplanınca birlikte kaybolur)
        Transform fruits = new GameObject("Fruits").transform;
        fruits.SetParent(crown, false);
        Vector3[] spots =
        {
            new Vector3( 0.55f, 0.35f,  0.45f), new Vector3(-0.60f, 0.25f,  0.40f),
            new Vector3( 0.15f, 0.30f, -0.70f), new Vector3(-0.30f, 0.05f,  0.75f),
            new Vector3( 0.70f, 0.00f, -0.30f), new Vector3(-0.65f,-0.10f, -0.35f),
            new Vector3( 0.20f, 0.85f,  0.35f),
        };
        foreach (Vector3 p in spots)
            Part(PrimitiveType.Sphere, "Fruit_Visual", p, fruitScale, fruitColor, fruits);

        CacheParts();
    }

    void CacheParts()
    {
        visual = transform.Find("Visual");
        crown = visual != null ? visual.Find("Crown") : null;
    }

    void Update()
    {
        if (crown == null) return;
        float t = Time.time + seed;
        crown.localRotation = Quaternion.Euler(Mathf.Sin(t * 1.3f) * 1.2f, 0f, Mathf.Sin(t * 1.7f) * 1.8f);   // rüzgârda sallanma
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
            string path = dir + "/Tree_" + ColorUtility.ToHtmlStringRGB(color) + ".mat";
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
