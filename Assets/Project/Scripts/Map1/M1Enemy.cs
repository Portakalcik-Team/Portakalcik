using UnityEngine;

// Düşman: yuvarlak gövde + gözler çizer, oyuncuyu görünce kovalar, canı biter ölür.
// Elle eklemene gerek yok: M1TreeDecorator (Player'da) Diken/Yarasa/Tilki/Golem/Muhafız adlı objelere ekler.
// Değerler Denge Sistemi Seviye 1: hp / hasar / saldırı aralığı / hız (oyuncu hızı 5 ile çarpılmış).
public class M1Enemy : MonoBehaviour
{
    public int hp;
    public int damage;
    public float attackInterval;      // oyuncuya vurma aralığı (M1PlayerTrigger okur)
    public float speed;               // birim / saniye
    public float detectRange = 9f;
    public float stopDistance = 0.8f;

    [HideInInspector] public bool openedDoor;   // öldüğünde yanındaki kapıyı açtı mı

    Color bodyColor = new Color(0.35f, 0.75f, 0.25f);
    bool spikes = true;
    float bodyRadius = 0.55f;
    float bodyY;                      // gövde merkezi (objeye göre yukarı mesafe)
    Transform player;

    static readonly string[] BlockPrefixes = { "wall", "duvar", "door", "kapi" };

    void Awake()
    {
        SetStats();
        BuildBody();
    }

    // ---------- Değerler ----------
    void SetStats()
    {
        string n = name.ToLower();
        int h, dmg; float interval, spd;

        if (n.StartsWith("yarasa"))       { h = 9;  dmg = 6;  interval = 1.0f; spd = 6.0f; bodyColor = new Color(0.45f, 0.30f, 0.65f); spikes = false; }
        else if (n.StartsWith("tilki"))   { h = 12; dmg = 7;  interval = 1.5f; spd = 4.5f; bodyColor = new Color(0.95f, 0.50f, 0.15f); spikes = false; }
        else if (n.StartsWith("golem"))   { h = 18; dmg = 10; interval = 2.5f; spd = 1.5f; bodyColor = new Color(0.45f, 0.32f, 0.22f); spikes = false; }
        else if (n.StartsWith("muhafiz")) { h = 24; dmg = 12; interval = 1.8f; spd = 3.5f; bodyColor = new Color(0.70f, 0.12f, 0.12f); spikes = false; }
        else                              { h = 6;  dmg = 4;  interval = 2.0f; spd = 2.0f; }   // Diken Böceği

        if (hp <= 0) hp = h;
        if (damage <= 0) damage = dmg;
        if (attackInterval <= 0) attackInterval = interval;
        if (speed <= 0) speed = spd;
    }

    // ---------- Görünüm ----------
    void BuildBody()
    {
        var box = GetComponent<BoxCollider>();
        float groundY = box != null ? box.bounds.min.y : transform.position.y - 0.25f;

        // Kök ölçeğini 1 yap (eski küp ölçeği gövdeyi bozmasın), collider'ı gövdeye uyduracağız
        transform.localScale = Vector3.one;
        bodyY = groundY + bodyRadius - transform.position.y;
        if (box != null)
        {
            box.isTrigger = true;
            box.size = Vector3.one * (bodyRadius * 2f + 0.1f);
            box.center = new Vector3(0f, bodyY, 0f);
        }

        var mr = GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;                 // eski küp görünmesin

        // Meyvenin çarpması ve oyuncunun değmesi için sabit (kinematik) Rigidbody
        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Vector3 c = new Vector3(0f, bodyY, 0f);

        // Gövde (yuvarlak)
        Part("Govde", PrimitiveType.Sphere, c, Quaternion.identity, Vector3.one * (bodyRadius * 2f), bodyColor);

        // Gözler (+Z yönüne bakar, kök oyuncuya döner)
        foreach (float side in new[] { -0.26f, 0.26f })
        {
            Part("Goz", PrimitiveType.Sphere, c + new Vector3(side, 0.2f, 0.40f), Quaternion.identity, Vector3.one * 0.30f, Color.white);
            Part("Gozbebegi", PrimitiveType.Sphere, c + new Vector3(side, 0.2f, 0.53f), Quaternion.identity, Vector3.one * 0.15f, Color.black);
        }

        // Dikenler (sadece Diken Böceği)
        if (spikes)
        {
            Color spikeColor = bodyColor * 0.6f;
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI * 2f / 8f;
                Vector3 dir = new Vector3(Mathf.Cos(a) * 0.9f, 0.55f, Mathf.Sin(a) * 0.9f).normalized;
                if (dir.z > 0.6f) continue;                  // gözlerin önünü kapatma
                Part("Diken", PrimitiveType.Capsule, c + dir * bodyRadius,
                     Quaternion.FromToRotation(Vector3.up, dir), new Vector3(0.14f, 0.22f, 0.14f), spikeColor);
            }
        }
    }

    void Part(string partName, PrimitiveType type, Vector3 localPos, Quaternion localRot, Vector3 scale, Color color)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = partName;
        var col = go.GetComponent<Collider>();
        if (col != null) Destroy(col);                       // parçalar çarpışma yapmasın

        var t = go.transform;
        t.SetParent(transform, false);
        t.localPosition = localPos;
        t.localRotation = localRot;
        t.localScale = scale;
        go.GetComponent<Renderer>().material.color = color;
    }

    // ---------- Davranış ----------
    void Update()
    {
        if (M1PlayerTrigger.Stopped) return;

        if (player == null)
        {
            var p = FindFirstObjectByType<M1PlayerTrigger>();
            if (p == null) return;
            player = p.transform;
        }

        Vector3 to = player.position - transform.position;
        to.y = 0f;
        float d = to.magnitude;
        if (d > detectRange || d < 0.01f) return;

        transform.rotation = Quaternion.LookRotation(to.normalized, Vector3.up);   // gözler oyuncuya baksın

        if (!HasLineOfSight()) return;     // duvarın arkasından kovalama
        if (d <= stopDistance) return;     // yetişti, temas hasarını M1PlayerTrigger verir

        Vector3 dir = to.normalized;
        float step = speed * Time.deltaTime;
        if (TryMove(dir * step)) return;
        if (TryMove(new Vector3(dir.x, 0f, 0f).normalized * step * Mathf.Abs(dir.x))) return;   // duvar boyunca kay
        TryMove(new Vector3(0f, 0f, dir.z).normalized * step * Mathf.Abs(dir.z));
    }

    bool HasLineOfSight()
    {
        Vector3 from = transform.position + Vector3.up * bodyY;
        Vector3 target = player.position;
        if (Physics.Linecast(from, target, out RaycastHit hit, ~0, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.GetComponentInParent<M1PlayerTrigger>() != null) return true;
            return !IsBlocker(hit.collider.transform);
        }
        return true;
    }

    bool TryMove(Vector3 delta)
    {
        if (delta.sqrMagnitude < 1e-8f) return false;
        Vector3 center = transform.position + delta + Vector3.up * bodyY;
        foreach (var h in Physics.OverlapSphere(center, bodyRadius * 0.9f, ~0, QueryTriggerInteraction.Ignore))
            if (IsBlocker(h.transform)) return false;
        transform.position += delta;
        return true;
    }

    static bool IsBlocker(Transform t)
    {
        for (; t != null; t = t.parent)
        {
            string n = t.name.ToLower();
            foreach (var p in BlockPrefixes) if (n.StartsWith(p)) return true;
        }
        return false;
    }

    // ---------- Hasar / ölüm ----------
    // Öldüyse true döner
    public bool TakeDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            openedDoor = OpenNearbyDoor();
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    // Yanındaki kısayol kapısını açar (canavar ölünce kapı açılır)
    bool OpenNearbyDoor()
    {
        Transform best = null;
        float bestD = 4f;     // hücre boyu 3: komşu hücredeki kapı
        foreach (var t in FindObjectsByType<Transform>(FindObjectsSortMode.None))
        {
            string n = t.name.ToLower();
            if (!n.StartsWith("door") || n.StartsWith("doors")) continue;
            if (!t.gameObject.activeInHierarchy) continue;
            Vector3 d = t.position - transform.position;
            d.y = 0f;
            if (d.magnitude < bestD) { bestD = d.magnitude; best = t; }
        }
        if (best == null) return false;
        best.gameObject.SetActive(false);
        return true;
    }
}
