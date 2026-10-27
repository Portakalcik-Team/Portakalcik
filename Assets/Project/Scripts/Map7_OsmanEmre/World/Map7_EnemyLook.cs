using UnityEngine;

public enum Map7_EnemyKind { DikenBocegi, GolgeYarasa, OrmanTilkisi, KokGolemi }

// Düşmanın düz küp görünümünü gizler, yerine primitive şekillerden küçük bir karakter kurar ve canlandırır.
// Collider ve hareket scriptlerine dokunmaz; sadece görsel ekler.
public class Map7_EnemyLook : MonoBehaviour
{
    [SerializeField] private Map7_EnemyKind kind = Map7_EnemyKind.DikenBocegi;

    private Transform visual;          // dünya ölçeğinde 1 olan görsel kök
    private Transform wingL, wingR;    // yarasa kanatları
    private Transform tail;            // tilki kuyruğu
    private Transform armL, armR;      // golem kolları
    private Transform body;            // nefes alma animasyonu
    private Vector3 bodyBaseScale;
    private float seed;

    void Awake()
    {
        seed = Random.value * 10f;

        // Prefab'a kaydedilmiş karakter varsa sadece animasyon parçalarını bul; yoksa çalışma anında kur.
        if (transform.Find("Visual") == null) Build();
        else CacheParts();
    }

    // Editör aracı (Map7_BakeEnemies) bunu çağırarak karakteri prefab'a gerçek alt objeler olarak kaydeder.
    [ContextMenu("Build Visual")]
    public void Build()
    {
        Transform old = transform.Find("Visual");
        if (old != null) DestroyObj(old.gameObject);

        // Eski küp görünümünü gizle (collider kalır)
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
            r.enabled = false;

        // Kök ölçeği (1,0.8,1) gibi eşit değilse şekiller bozulur; ters ölçekle dengeliyoruz.
        GameObject v = new GameObject("Visual");
        visual = v.transform;
        visual.SetParent(transform, false);
        Vector3 s = transform.lossyScale;
        visual.localScale = new Vector3(1f / s.x, 1f / s.y, 1f / s.z);

        switch (kind)
        {
            case Map7_EnemyKind.DikenBocegi: BuildBeetle(); break;
            case Map7_EnemyKind.GolgeYarasa: BuildBat(); break;
            case Map7_EnemyKind.OrmanTilkisi: BuildFox(); break;
            case Map7_EnemyKind.KokGolemi: BuildGolem(); break;
        }
        CacheParts();
    }

    // Animasyon yapılacak parçaları isimlerinden bulur.
    void CacheParts()
    {
        visual = transform.Find("Visual");
        wingL = FindPart("WingL"); wingR = FindPart("WingR");
        tail = FindPart("Tail");
        armL = FindPart("ArmL"); armR = FindPart("ArmR");
        body = FindPart("Body");
        if (body != null) bodyBaseScale = body.localScale;
    }

    Transform FindPart(string partName)
    {
        if (visual == null) return null;
        foreach (Transform t in visual.GetComponentsInChildren<Transform>(true))
            if (t.name == partName) return t;
        return null;
    }

    static void DestroyObj(Object o)
    {
        if (Application.isPlaying) Destroy(o);
        else DestroyImmediate(o);
    }

    // ---------- Hasar (Meyve Kepçesi) ----------
    [Header("Hasar")]
    [SerializeField] private int health = 3;       // can; Portakal 2, diğerleri 1 hasar verir
    private float stunTimer = 0f;                  // dondurma süresi
    private float punchTimer = 0f;                 // vurulma "sıçrama" animasyonu
    private float deathTimer = -1f;                // ölüm küçülme animasyonu
    private Vector3 deathStartScale;
    private Vector3 visualBaseScale;
    private bool visualScaleKnown = false;
    private System.Collections.Generic.List<MonoBehaviour> pausedMovers = new System.Collections.Generic.List<MonoBehaviour>();

    // Map7_ThrownFruit çağırır: fırlatılan meyve düşmana çarptı.
    public void Hit(Map7_FruitType type)
    {
        if (deathTimer >= 0f) return;

        int damage = type == Map7_FruitType.Portakal ? 2 : 1;
        float stun = type == Map7_FruitType.Limon ? 3f : (type == Map7_FruitType.Mandalina ? 1f : 1.5f);

        health -= damage;
        punchTimer = 0.2f;

        if (health <= 0)
        {
            deathTimer = 0f;
            deathStartScale = transform.localScale;
            PauseMovers();
            Destroy(gameObject, 0.4f);
            return;
        }

        stunTimer = Mathf.Max(stunTimer, stun);
        PauseMovers();
    }

    void PauseMovers()
    {
        // Hareket eden scriptleri durdur (Patroller, Spinner, Bobber)
        MonoBehaviour[] movers = { GetComponent<Map7_Patroller>(), GetComponent<Map7_Spinner>(), GetComponent<Map7_Bobber>() };
        foreach (MonoBehaviour m in movers)
            if (m != null && m.enabled) { m.enabled = false; pausedMovers.Add(m); }
    }

    void ResumeMovers()
    {
        foreach (MonoBehaviour m in pausedMovers) if (m != null) m.enabled = true;
        pausedMovers.Clear();
    }

    void UpdateHitEffects()
    {
        if (!visualScaleKnown && visual != null) { visualBaseScale = visual.localScale; visualScaleKnown = true; }

        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f && deathTimer < 0f) ResumeMovers();
        }

        if (punchTimer > 0f)
        {
            punchTimer -= Time.deltaTime;
            if (visual != null && visualScaleKnown)
                visual.localScale = visualBaseScale * (1f + Mathf.Max(0f, punchTimer) * 1.5f);   // vurulunca şişip söner
        }

        if (deathTimer >= 0f)
        {
            deathTimer += Time.deltaTime;
            transform.localScale = deathStartScale * Mathf.Clamp01(1f - deathTimer / 0.4f);
        }
    }
    void Update()
    {
        UpdateHitEffects();
        float t = Time.time + seed;

        if (body != null)
        {
            float b = 1f + Mathf.Sin(t * 3f) * 0.04f;   // nefes alma
            body.localScale = new Vector3(bodyBaseScale.x, bodyBaseScale.y * b, bodyBaseScale.z);
        }
        if (wingL != null && wingR != null)
        {
            float flap = Mathf.Sin(t * 10f) * 35f;       // kanat çırpma
            wingL.localRotation = Quaternion.Euler(0f, 0f, flap);
            wingR.localRotation = Quaternion.Euler(0f, 0f, -flap);
        }
        if (tail != null)
            tail.localRotation = Quaternion.Euler(-30f, Mathf.Sin(t * 6f) * 25f, 0f);   // kuyruk sallama
        if (armL != null && armR != null)
        {
            float sw = Mathf.Sin(t * 2f) * 20f;          // kol sallama
            armL.localRotation = Quaternion.Euler(sw, 0f, 0f);
            armR.localRotation = Quaternion.Euler(-sw, 0f, 0f);
        }
    }

    // ---------- Karakterler ----------

    void BuildBeetle()
    {
        Color shell = new Color(0.35f, 0.12f, 0.45f);
        Color spike = new Color(1f, 0.55f, 0.1f);
        Color leg = new Color(0.2f, 0.07f, 0.28f);

        body = Part(PrimitiveType.Sphere, "Body", new Vector3(0f, 0f, 0f), new Vector3(0.9f, 0.55f, 1.2f), shell);
        bodyBaseScale = body.localScale;
        Part(PrimitiveType.Sphere, "Head", new Vector3(0f, -0.02f, 0.6f), new Vector3(0.5f, 0.4f, 0.4f), shell * 1.2f);
        Eyes(new Vector3(0f, 0.08f, 0.78f), 0.17f, 0.07f, Color.yellow);

        // Sırt dikenleri
        for (int i = 0; i < 5; i++)
        {
            float z = -0.4f + i * 0.22f;
            Transform sp = Part(PrimitiveType.Cube, "Spike", new Vector3(0f, 0.3f, z), new Vector3(0.14f, 0.34f, 0.14f), spike);
            sp.localRotation = Quaternion.Euler(0f, 45f, 20f);
        }
        // Bacaklar
        for (int i = 0; i < 3; i++)
        {
            float z = -0.3f + i * 0.3f;
            Part(PrimitiveType.Capsule, "LegL", new Vector3(-0.5f, -0.2f, z), new Vector3(0.1f, 0.22f, 0.1f), leg).localRotation = Quaternion.Euler(0f, 0f, 55f);
            Part(PrimitiveType.Capsule, "LegR", new Vector3(0.5f, -0.2f, z), new Vector3(0.1f, 0.22f, 0.1f), leg).localRotation = Quaternion.Euler(0f, 0f, -55f);
        }
    }

    void BuildBat()
    {
        Color fur = new Color(0.18f, 0.1f, 0.28f);
        Color wing = new Color(0.28f, 0.14f, 0.4f);

        body = Part(PrimitiveType.Sphere, "Body", Vector3.zero, new Vector3(0.45f, 0.5f, 0.45f), fur);
        bodyBaseScale = body.localScale;
        Part(PrimitiveType.Sphere, "Head", new Vector3(0f, 0.32f, 0.05f), new Vector3(0.32f, 0.3f, 0.3f), fur * 1.15f);
        Part(PrimitiveType.Cube, "EarL", new Vector3(-0.1f, 0.52f, 0.05f), new Vector3(0.08f, 0.16f, 0.05f), fur).localRotation = Quaternion.Euler(0f, 0f, 15f);
        Part(PrimitiveType.Cube, "EarR", new Vector3(0.1f, 0.52f, 0.05f), new Vector3(0.08f, 0.16f, 0.05f), fur).localRotation = Quaternion.Euler(0f, 0f, -15f);
        Eyes(new Vector3(0f, 0.34f, 0.2f), 0.09f, 0.045f, new Color(1f, 0.15f, 0.1f));

        // Kanatlar: pivot gövdede, kanat dışa uzanıyor
        wingL = Pivot("WingL", new Vector3(-0.2f, 0.05f, 0f));
        wingR = Pivot("WingR", new Vector3(0.2f, 0.05f, 0f));
        Part(PrimitiveType.Cube, "WingBlade", new Vector3(-0.32f, 0f, 0f), new Vector3(0.65f, 0.04f, 0.4f), wing, wingL);
        Part(PrimitiveType.Cube, "WingBlade", new Vector3(0.32f, 0f, 0f), new Vector3(0.65f, 0.04f, 0.4f), wing, wingR);
    }

    void BuildFox()
    {
        Color orange = new Color(1f, 0.5f, 0.1f);
        Color white = new Color(1f, 0.95f, 0.88f);
        Color dark = new Color(0.15f, 0.08f, 0.05f);

        // Gövde yatay capsule (z yönünde uzun)
        body = Part(PrimitiveType.Capsule, "Body", new Vector3(0f, 0f, -0.1f), new Vector3(0.6f, 0.65f, 0.6f), orange);
        body.localRotation = Quaternion.Euler(90f, 0f, 0f);
        bodyBaseScale = body.localScale;
        Part(PrimitiveType.Sphere, "Head", new Vector3(0f, 0.18f, 0.6f), new Vector3(0.5f, 0.45f, 0.45f), orange);
        Part(PrimitiveType.Sphere, "Snout", new Vector3(0f, 0.1f, 0.85f), new Vector3(0.22f, 0.18f, 0.25f), white);
        Part(PrimitiveType.Sphere, "Nose", new Vector3(0f, 0.14f, 0.98f), new Vector3(0.07f, 0.07f, 0.07f), dark);
        Part(PrimitiveType.Cube, "EarL", new Vector3(-0.15f, 0.48f, 0.55f), new Vector3(0.12f, 0.24f, 0.06f), orange).localRotation = Quaternion.Euler(0f, 0f, 10f);
        Part(PrimitiveType.Cube, "EarR", new Vector3(0.15f, 0.48f, 0.55f), new Vector3(0.12f, 0.24f, 0.06f), orange).localRotation = Quaternion.Euler(0f, 0f, -10f);
        Eyes(new Vector3(0f, 0.27f, 0.78f), 0.15f, 0.06f, dark);
        // Bacaklar
        foreach (float z in new[] { -0.45f, 0.25f })
        {
            Part(PrimitiveType.Capsule, "LegL", new Vector3(-0.2f, -0.35f, z), new Vector3(0.14f, 0.22f, 0.14f), dark);
            Part(PrimitiveType.Capsule, "LegR", new Vector3(0.2f, -0.35f, z), new Vector3(0.14f, 0.22f, 0.14f), dark);
        }
        // Kuyruk (pivot gövdenin arkasında, ucu beyaz)
        tail = Pivot("Tail", new Vector3(0f, 0.05f, -0.65f));
        Part(PrimitiveType.Capsule, "TailBody", new Vector3(0f, 0f, -0.3f), new Vector3(0.22f, 0.32f, 0.22f), orange, tail).localRotation = Quaternion.Euler(90f, 0f, 0f);
        Part(PrimitiveType.Sphere, "TailTip", new Vector3(0f, 0f, -0.58f), new Vector3(0.24f, 0.24f, 0.24f), white, tail);
    }

    void BuildGolem()
    {
        Color bark = new Color(0.4f, 0.26f, 0.14f);
        Color darkBark = new Color(0.28f, 0.17f, 0.09f);
        Color moss = new Color(0.25f, 0.6f, 0.2f);
        Color glow = new Color(0.4f, 1f, 0.3f);

        body = Part(PrimitiveType.Cube, "Body", new Vector3(0f, 0f, 0f), new Vector3(1.1f, 1.2f, 0.8f), bark);
        bodyBaseScale = body.localScale;
        Part(PrimitiveType.Cube, "Head", new Vector3(0f, 0.85f, 0.05f), new Vector3(0.7f, 0.6f, 0.65f), darkBark);
        Eyes(new Vector3(0f, 0.92f, 0.38f), 0.28f, 0.1f, glow);
        // Yosun parçaları
        Part(PrimitiveType.Sphere, "Moss1", new Vector3(-0.3f, 0.65f, -0.1f), new Vector3(0.5f, 0.25f, 0.5f), moss);
        Part(PrimitiveType.Sphere, "Moss2", new Vector3(0.25f, 0.4f, 0.4f), new Vector3(0.3f, 0.18f, 0.3f), moss);
        Part(PrimitiveType.Sphere, "Moss3", new Vector3(0.1f, 1.2f, 0f), new Vector3(0.4f, 0.2f, 0.4f), moss);
        // Dal gibi kollar (pivot omuzda)
        armL = Pivot("ArmL", new Vector3(-0.7f, 0.4f, 0f));
        armR = Pivot("ArmR", new Vector3(0.7f, 0.4f, 0f));
        Part(PrimitiveType.Cube, "ArmBlock", new Vector3(0f, -0.4f, 0f), new Vector3(0.3f, 0.9f, 0.3f), darkBark, armL);
        Part(PrimitiveType.Cube, "ArmBlock", new Vector3(0f, -0.4f, 0f), new Vector3(0.3f, 0.9f, 0.3f), darkBark, armR);
        Part(PrimitiveType.Sphere, "Fist", new Vector3(0f, -0.9f, 0f), new Vector3(0.4f, 0.4f, 0.4f), bark, armL);
        Part(PrimitiveType.Sphere, "Fist", new Vector3(0f, -0.9f, 0f), new Vector3(0.4f, 0.4f, 0.4f), bark, armR);
        // Bacaklar
        Part(PrimitiveType.Cube, "LegL", new Vector3(-0.3f, -0.8f, 0f), new Vector3(0.4f, 0.5f, 0.45f), darkBark);
        Part(PrimitiveType.Cube, "LegR", new Vector3(0.3f, -0.8f, 0f), new Vector3(0.4f, 0.5f, 0.45f), darkBark);
    }

    // ---------- Yardımcılar ----------

    Transform Pivot(string name, Vector3 pos)
    {
        Transform p = new GameObject(name).transform;
        p.SetParent(visual, false);
        p.localPosition = pos;
        return p;
    }

    Transform Part(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Color color, Transform parent = null)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        DestroyObj(go.GetComponent<Collider>());   // tetik/fizik bozulmasın
        go.transform.SetParent(parent != null ? parent : visual, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = MakeMaterial(color);
        return go.transform;
    }

    void Eyes(Vector3 center, float spacing, float size, Color color)
    {
        Part(PrimitiveType.Sphere, "EyeL", center + Vector3.left * spacing, Vector3.one * size, color);
        Part(PrimitiveType.Sphere, "EyeR", center + Vector3.right * spacing, Vector3.one * size, color);
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
            string path = dir + "/Enemy_" + ColorUtility.ToHtmlStringRGB(color) + ".mat";
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
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.35f);
        return m;
    }
}
