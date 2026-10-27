using UnityEngine;
using UnityEngine.InputSystem;

// Fırlatılan meyve. Düşmana çarparsa düşman hasar alır ve meyve patlar (Splat).
// Duvara/zemine düşerse yerde kalır; oyuncu yaklaşıp E'ye basarak geri alabilir (meyve kaybolmasın diye).
public class Map7_ThrownFruit : MonoBehaviour
{
    private const float PickupRange = 2.2f;
    private const float Lifetime = 40f;

    private Map7_FruitType type;
    private Rigidbody rb;
    private bool landed = false;
    private float age = 0f;
    private Transform player;
    private Map7_FruitBag bag;

    // ---------- Oluşturma ----------

    public static Map7_ThrownFruit Spawn(Map7_FruitType type, Vector3 position, Vector3 velocity, GameObject owner)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "ThrownFruit_" + type;
        go.transform.position = position;
        go.transform.localScale = FruitScale(type) * 1.3f;

        Renderer r = go.GetComponent<Renderer>();
        r.sharedMaterial = FruitMaterial(type);
        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

        Rigidbody body = go.AddComponent<Rigidbody>();
        body.mass = type == Map7_FruitType.Portakal ? 0.6f : 0.3f;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        body.linearVelocity = velocity;
        body.angularVelocity = Random.insideUnitSphere * 6f;

        // Atan oyuncuyla çarpışmasın
        Collider col = go.GetComponent<Collider>();
        if (owner != null)
            foreach (Collider oc in owner.GetComponentsInChildren<Collider>())
                Physics.IgnoreCollision(col, oc);

        Map7_ThrownFruit t = go.AddComponent<Map7_ThrownFruit>();
        t.type = type;
        t.rb = body;
        t.player = owner != null ? owner.transform : null;
        if (owner != null) t.bag = owner.GetComponentInParent<Map7_FruitBag>();
        return t;
    }

    // ---------- Ortak görünüm bilgileri (Launcher da kullanır) ----------

    public static Vector3 FruitScale(Map7_FruitType type)
    {
        switch (type)
        {
            case Map7_FruitType.Limon: return new Vector3(0.17f, 0.22f, 0.17f);
            case Map7_FruitType.Mandalina: return new Vector3(0.2f, 0.17f, 0.2f);
            default: return new Vector3(0.23f, 0.23f, 0.23f);
        }
    }

    public static Color FruitColor(Map7_FruitType type)
    {
        switch (type)
        {
            case Map7_FruitType.Limon: return new Color(1f, 0.92f, 0.15f);
            case Map7_FruitType.Mandalina: return new Color(1f, 0.6f, 0.1f);
            default: return new Color(1f, 0.42f, 0f);
        }
    }

    private static Material[] materialCache = new Material[3];

    public static Material FruitMaterial(Map7_FruitType type)
    {
        int i = (int)type;
        if (materialCache[i] == null) materialCache[i] = MakeMaterial(FruitColor(type));
        return materialCache[i];
    }

    public static Material MakeMaterial(Color color)
    {
        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        Material m = new Material(sh);
        m.color = color;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.4f);
        return m;
    }

    // ---------- Davranış ----------

    void Update()
    {
        age += Time.deltaTime;
        if (age > Lifetime) { Destroy(gameObject); return; }

        if (landed && player != null && bag != null &&
            Vector3.Distance(transform.position, player.position) <= PickupRange &&
            Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            bag.Add(type, 1);
            Destroy(gameObject);
        }
    }

    // Düşmanlar trigger collider'lıdır; meyve (katı + Rigidbody) onların içine girince buraya düşer.
    void OnTriggerEnter(Collider other)
    {
        if (landed) return;
        Map7_EnemyLook enemy = other.GetComponentInParent<Map7_EnemyLook>();
        if (enemy == null) return;

        enemy.Hit(type);
        Splat(transform.position, FruitColor(type));
        Destroy(gameObject);
    }

    // Duvar, zemin, kapı vb. katı bir şeye çarptı: yerde kalır, geri alınabilir.
    void OnCollisionEnter(Collision c)
    {
        if (landed) return;
        landed = true;
    }

    void OnGUI()
    {
        if (!landed || player == null) return;
        if (Vector3.Distance(transform.position, player.position) > PickupRange) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 18;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(0, Screen.height * 0.67f, Screen.width, 32), "[E] " + type + " geri al", style);
    }

    // Küçük meyve suyu/parçacık patlaması (parçacık sistemi gerektirmez)
    public static void Splat(Vector3 position, Color color)
    {
        Material mat = MakeMaterial(color);
        for (int i = 0; i < 9; i++)
        {
            GameObject d = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(d.GetComponent<Collider>());
            d.transform.position = position;
            d.transform.localScale = Vector3.one * Random.Range(0.06f, 0.14f);
            d.GetComponent<Renderer>().sharedMaterial = mat;

            Rigidbody b = d.AddComponent<Rigidbody>();
            b.linearVelocity = Random.onUnitSphere * Random.Range(2f, 5f) + Vector3.up * 2f;
            Destroy(d, 0.8f);
        }
    }
}
