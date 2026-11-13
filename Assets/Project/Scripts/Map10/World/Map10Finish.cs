using UnityEngine;

[ExecuteAlways]
public class Map10Finish : MonoBehaviour
{
    private bool isFinished = false;
    private float finishTime = 0f;
    private int collectedMandarins = 0;
    private Transform visualContainer;
    private Transform portalRing;

    void OnEnable()
    {
        SetupPortalVisuals();
    }

    void Start()
    {
        SetupPortalVisuals();
    }

    private void SetupPortalVisuals()
    {
        Transform existing = transform.Find("ExitPortalVisuals");
        if (existing != null)
        {
            visualContainer = existing;
            portalRing = existing.Find("PortalRing");
            return;
        }

        BuildPortalVisuals();
    }

    private void BuildPortalVisuals()
    {
        GameObject container = new GameObject("ExitPortalVisuals");
        visualContainer = container.transform;
        visualContainer.SetParent(transform);
        visualContainer.localPosition = Vector3.zero;
        visualContainer.localRotation = Quaternion.identity;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material goldMat = new Material(shader) { color = new Color(1f, 0.78f, 0.15f) };
        Material portalGlowMat = new Material(shader) { color = new Color(0.1f, 0.95f, 0.4f) };
        Material gemMat = new Material(shader) { color = new Color(0.2f, 0.8f, 1f) };

        // Sol Sütun
        GameObject leftPillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        leftPillar.name = "LeftPillar";
        leftPillar.transform.SetParent(visualContainer);
        leftPillar.transform.localPosition = new Vector3(-1.2f, 1.6f, 0f);
        leftPillar.transform.localScale = new Vector3(0.35f, 1.6f, 0.35f);
        StripCollider(leftPillar);
        leftPillar.GetComponent<Renderer>().material = goldMat;

        // Sağ Sütun
        GameObject rightPillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rightPillar.name = "RightPillar";
        rightPillar.transform.SetParent(visualContainer);
        rightPillar.transform.localPosition = new Vector3(1.2f, 1.6f, 0f);
        rightPillar.transform.localScale = new Vector3(0.35f, 1.6f, 0.35f);
        StripCollider(rightPillar);
        rightPillar.GetComponent<Renderer>().material = goldMat;

        // Üst Kemer / Kiriş
        GameObject arch = GameObject.CreatePrimitive(PrimitiveType.Cube);
        arch.name = "Arch";
        arch.transform.SetParent(visualContainer);
        arch.transform.localPosition = new Vector3(0f, 3.2f, 0f);
        arch.transform.localScale = new Vector3(2.8f, 0.35f, 0.5f);
        StripCollider(arch);
        arch.GetComponent<Renderer>().material = goldMat;

        // Ortada Dönen Enerji Portalı (Glow Ring)
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ring.name = "PortalRing";
        portalRing = ring.transform;
        portalRing.SetParent(visualContainer);
        portalRing.localPosition = new Vector3(0f, 1.7f, 0f);
        portalRing.localScale = new Vector3(1.7f, 2.3f, 0.25f);
        StripCollider(ring);
        ring.GetComponent<Renderer>().material = portalGlowMat;

        // Üst Mücevher / Kristal
        GameObject gem = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        gem.name = "ArchGem";
        gem.transform.SetParent(visualContainer);
        gem.transform.localPosition = new Vector3(0f, 3.65f, 0f);
        gem.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        StripCollider(gem);
        gem.GetComponent<Renderer>().material = gemMat;
    }

    private void StripCollider(GameObject obj)
    {
        Collider c = obj.GetComponent<Collider>();
        if (c != null)
        {
            if (Application.isPlaying) Destroy(c);
            else DestroyImmediate(c);
        }
    }

    void Update()
    {
        // Portal enerjisinin hafifçe dalgalanıp dönmesi
        if (portalRing != null)
        {
            portalRing.Rotate(Vector3.forward, 35f * Time.deltaTime, Space.Self);
            float pulse = 1f + Mathf.Sin(Time.time * 3f) * 0.08f;
            portalRing.localScale = new Vector3(1.7f * pulse, 2.3f * pulse, 0.25f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isFinished) return;

        if (other.CompareTag("Player") || other.GetComponent<M10PlayerMove>() != null)
        {
            TriggerFinish(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isFinished) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<M10PlayerMove>() != null)
        {
            TriggerFinish(collision.gameObject);
        }
    }

    private void TriggerFinish(GameObject player)
    {
        isFinished = true;
        finishTime = Time.time;

        Debug.Log("[Map10] 🏆 LEVEL COMPLETED! Labirent tamamlandı!");

        // 1. Kamera Sarsıntısı (Camera Shake)
        if (Map10CameraFollow.Instance != null)
        {
            Map10CameraFollow.Instance.Shake(1.5f, 0.5f);
        }

        // 2. Kalan mandalinaları öğren
        Map10PlayerCarry carry = player.GetComponent<Map10PlayerCarry>();
        if (carry != null)
        {
            collectedMandarins = carry.MandarinCount;
        }

        // 3. Konfeti / Kutlama Parçacıkları Saç
        SpawnCelebrationParticles(player.transform.position);
    }

    private void SpawnCelebrationParticles(Vector3 origin)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Color[] colors = new Color[]
        {
            new Color(1f, 0.85f, 0.1f), // Altın sarısı
            new Color(1f, 0.45f, 0f),   // Mandalina turuncusu
            new Color(0.2f, 0.85f, 0.3f), // Canlı yeşil
            new Color(0.9f, 0.2f, 0.4f), // Pembe/Kırmızı
            new Color(0.2f, 0.6f, 1f)    // Mavi
        };

        for (int i = 0; i < 25; i++)
        {
            GameObject confetti = GameObject.CreatePrimitive(PrimitiveType.Cube);
            confetti.name = "Confetti";
            confetti.transform.position = origin + Vector3.up * 1.5f + Random.insideUnitSphere * 0.5f;
            confetti.transform.localScale = new Vector3(0.12f, 0.12f, 0.04f);
            confetti.transform.rotation = Random.rotation;

            Collider c = confetti.GetComponent<Collider>();
            if (c != null) Destroy(c);

            Material mat = new Material(shader) { color = colors[i % colors.Length] };
            confetti.GetComponent<Renderer>().material = mat;

            Rigidbody rb = confetti.AddComponent<Rigidbody>();
            rb.mass = 0.02f;
            rb.linearVelocity = (Random.onUnitSphere + Vector3.up * 2f) * Random.Range(3f, 7f);
            rb.angularVelocity = Random.insideUnitSphere * 20f;

            Destroy(confetti, 3f);
        }
    }

    void OnGUI()
    {
        if (!isFinished) return;

        // Arka planı hafif karartan şık gölge
        GUI.color = new Color(0f, 0f, 0f, 0.65f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Ortadaki Zafer Kutusu
        float boxWidth = 480f;
        float boxHeight = 220f;
        float x = (Screen.width - boxWidth) / 2f;
        float y = (Screen.height - boxHeight) / 2f;

        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        GUI.Box(new Rect(x, y, boxWidth, boxHeight), "", boxStyle);

        // Başlık: LEVEL COMPLETED!
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 32;
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.normal.textColor = new Color(1f, 0.85f, 0.2f); // Altın sarısı

        GUI.Label(new Rect(x, y + 25, boxWidth, 45), "🎉 LEVEL COMPLETED! 🎉", titleStyle);

        // Alt Açıklama
        GUIStyle subStyle = new GUIStyle();
        subStyle.fontSize = 18;
        subStyle.fontStyle = FontStyle.Normal;
        subStyle.alignment = TextAnchor.MiddleCenter;
        subStyle.normal.textColor = Color.white;

        GUI.Label(new Rect(x, y + 80, boxWidth, 30), "Tebrikler! Labirenti başarıyla tamamladın!", subStyle);

        // Mandalina İstatistiği
        GUIStyle statStyle = new GUIStyle();
        statStyle.fontSize = 16;
        statStyle.fontStyle = FontStyle.Bold;
        statStyle.alignment = TextAnchor.MiddleCenter;
        statStyle.normal.textColor = new Color(1f, 0.6f, 0.1f);

        GUI.Label(new Rect(x, y + 115, boxWidth, 30), $"🍊 Kalan Mandalina: {collectedMandarins}", statStyle);

        // Tebrik Mesajı
        GUIStyle hintStyle = new GUIStyle();
        hintStyle.fontSize = 13;
        hintStyle.fontStyle = FontStyle.Italic;
        hintStyle.alignment = TextAnchor.MiddleCenter;
        hintStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);

        GUI.Label(new Rect(x, y + 160, boxWidth, 25), "Harika bir kaçıştı! Yeni maceralara hazır ol.", hintStyle);
    }
}
