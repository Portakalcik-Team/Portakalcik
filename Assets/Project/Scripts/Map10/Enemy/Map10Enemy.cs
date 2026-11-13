using UnityEngine;

[ExecuteAlways]
public class Map10Enemy : MonoBehaviour
{
    [Header("Canavar Can Ayarları")]
    [SerializeField] private int maxHealth = 5; // 4-5 vuruşta ölme
    [SerializeField] private int currentHealth;

    [Header("Yapay Zeka ve Takip (Agro)")]
    [SerializeField] private float detectRadius = 8f;   // Oyuncuyu fark etme menzili
    [SerializeField] private float loseRadius = 20f;    // Takibi bırakma menzili (oyuncu bu kadar uzaklaşınca bırakır)
    [SerializeField] private float moveSpeed = 2.8f;    // Takip hızı
    [SerializeField] private float rotationSpeed = 6f;  // Dönüş hızı

    [Header("Görsel Renkler")]
    [SerializeField] private Color bodyColor = new Color(0.15f, 0.04f, 0.15f);   // Koyu mor canavar gövdesi
    [SerializeField] private Color eyeColor = new Color(1f, 0.15f, 0.1f);        // Parlayan kırmızı gözler
    [SerializeField] private Color hornColor = new Color(0.2f, 0.05f, 0.25f);    // Sivri boynuzlar
    [SerializeField] private Color teethColor = new Color(0.95f, 0.95f, 0.85f);  // Sivri dişler

    private Transform playerTransform;
    private Rigidbody rb;
    private bool isAggro = false;
    private Transform visualContainer;
    private Transform leftEye, rightEye;
    private Vector3 spawnPosition;
    private float hitFlashTimer = 0f;
    private Renderer[] bodyRenderers;
    private Color originalBodyColor;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void OnEnable()
    {
        SetupEnemyVisuals();
    }

    void Start()
    {
        // Güvenlik: Eğer inspector'dan 0 girilmiş veya kapatılmışsa otomatik düzelt
        if (detectRadius <= 0.1f) detectRadius = 8f;
        if (loseRadius <= detectRadius) loseRadius = 20f;
        if (moveSpeed <= 0.1f) moveSpeed = 2.8f;
        if (rotationSpeed <= 0.1f) rotationSpeed = 6f;
        if (maxHealth <= 0) maxHealth = 5;

        currentHealth = maxHealth;
        spawnPosition = transform.position;

        rb = GetComponent<Rigidbody>();
        if (rb == null && Application.isPlaying)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.mass = 2f;
        }

        // Oyuncunun canavarın üstünden takılmadan zıplayıp aşabilmesi için kaygan Kapsül Collider
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            if (Application.isPlaying) Destroy(box);
            else DestroyImmediate(box);
        }

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        if (capsule == null) capsule = gameObject.AddComponent<CapsuleCollider>();

        capsule.center = new Vector3(0f, 0.4f, 0f);
        capsule.radius = 0.3f;
        capsule.height = 0.8f;

        // Pürüzsüz kaygan materyal (sıkışınca zıplayıp üstünden rahatça kayabilsin)
        PhysicsMaterial slickMat = new PhysicsMaterial("MonsterSlick")
        {
            dynamicFriction = 0f,
            staticFriction = 0f,
            frictionCombine = PhysicsMaterialCombine.Minimum
        };
        capsule.material = slickMat;

        SetupEnemyVisuals();

        if (Application.isPlaying)
        {
            FindPlayer();
        }
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    public void SetupEnemyVisuals()
    {
        // Orijinal kaba küp MeshRenderer'ını gizle
        MeshRenderer baseRend = GetComponent<MeshRenderer>();
        if (baseRend != null)
        {
            baseRend.enabled = false;
        }

        // Zaten görsel oluşturulmuşsa boyutunu güncelle ve devam et
        Transform existing = transform.Find("EnemyVisuals");
        if (existing != null)
        {
            float shrink = 0.55f;
            Vector3 parentScale = transform.localScale;
            existing.localScale = new Vector3(
                (parentScale.x != 0 ? 1f / parentScale.x : 1f) * shrink,
                (parentScale.y != 0 ? 1f / parentScale.y : 1f) * shrink,
                (parentScale.z != 0 ? 1f / parentScale.z : 1f) * shrink
            );
            visualContainer = existing;
            leftEye = existing.Find("LeftEye");
            rightEye = existing.Find("RightEye");
            return;
        }

        BuildMonsterVisuals();
    }

    /// <summary>
    /// Canavara gerçekçi ve korkutucu bir canavar görünümü verir
    /// </summary>
    private void BuildMonsterVisuals()
    {
        GameObject container = new GameObject("EnemyVisuals");
        visualContainer = container.transform;
        visualContainer.SetParent(transform);
        visualContainer.localPosition = Vector3.zero;
        visualContainer.localRotation = Quaternion.identity;

        // Boyutu orantıla — canavar %55 küçültülmüş (köşede sıkıştırmasın)
        float shrink = 0.55f;
        Vector3 parentScale = transform.localScale;
        visualContainer.localScale = new Vector3(
            (parentScale.x != 0 ? 1f / parentScale.x : 1f) * shrink,
            (parentScale.y != 0 ? 1f / parentScale.y : 1f) * shrink,
            (parentScale.z != 0 ? 1f / parentScale.z : 1f) * shrink
        );

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material bodyMat = new Material(shader) { color = bodyColor };
        Material eyeMat = new Material(shader) { color = eyeColor };
        Material hornMat = new Material(shader) { color = hornColor };
        Material teethMat = new Material(shader) { color = teethColor };

        // 1. Ana Canavar Gövdesi (Tombul ve heybetli canavar vücudu)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        body.name = "MonsterBody";
        body.transform.SetParent(visualContainer);
        body.transform.localPosition = new Vector3(0f, 0.75f, 0f);
        body.transform.localScale = new Vector3(1.3f, 1.1f, 1.2f);
        StripCollider(body);
        body.GetComponent<Renderer>().material = bodyMat;

        // 2. Canavar Sırt Dikenleri (Spikes)
        for (int i = 0; i < 3; i++)
        {
            GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            spike.name = $"Spike_{i}";
            spike.transform.SetParent(visualContainer);
            spike.transform.localPosition = new Vector3(0f, 1.15f - i * 0.2f, -0.45f - i * 0.15f);
            spike.transform.localScale = new Vector3(0.18f, 0.35f, 0.18f);
            spike.transform.localRotation = Quaternion.Euler(-40f, 0f, 0f);
            StripCollider(spike);
            spike.GetComponent<Renderer>().material = hornMat;
        }

        // 3. Sivri Canavar Boynuzları (Horns)
        CreateHorn(visualContainer, new Vector3(-0.45f, 1.4f, 0.2f), new Vector3(20f, -25f, -25f), hornMat);
        CreateHorn(visualContainer, new Vector3(0.45f, 1.4f, 0.2f), new Vector3(20f, 25f, 25f), hornMat);

        // 4. Parlayan Kırmızı Gözler (Eyes)
        GameObject lEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lEye.name = "LeftEye";
        leftEye = lEye.transform;
        leftEye.SetParent(visualContainer);
        leftEye.localPosition = new Vector3(-0.32f, 0.9f, 0.52f);
        leftEye.localScale = new Vector3(0.24f, 0.24f, 0.15f);
        StripCollider(lEye);
        lEye.GetComponent<Renderer>().material = eyeMat;

        GameObject rEye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rEye.name = "RightEye";
        rightEye = rEye.transform;
        rightEye.SetParent(visualContainer);
        rightEye.localPosition = new Vector3(0.32f, 0.9f, 0.52f);
        rightEye.localScale = new Vector3(0.24f, 0.24f, 0.15f);
        StripCollider(rEye);
        rEye.GetComponent<Renderer>().material = eyeMat;

        // 5. Korkunç Canavar Ağzı ve Sivri Dişler (Mouth & Fangs)
        GameObject mouth = GameObject.CreatePrimitive(PrimitiveType.Cube);
        mouth.name = "Mouth";
        mouth.transform.SetParent(visualContainer);
        mouth.transform.localPosition = new Vector3(0f, 0.52f, 0.56f);
        mouth.transform.localScale = new Vector3(0.6f, 0.14f, 0.1f);
        StripCollider(mouth);
        mouth.GetComponent<Renderer>().material = hornMat;

        // Üst dişler
        CreateFang(mouth.transform, new Vector3(-0.35f, -0.6f, 0.1f), teethMat);
        CreateFang(mouth.transform, new Vector3(0.35f, -0.6f, 0.1f), teethMat);

        // 6. Pençeli Kollar / Yan Dikenler
        CreateArm(visualContainer, new Vector3(-0.75f, 0.65f, 0.2f), Quaternion.Euler(15f, 0f, 35f), bodyMat);
        CreateArm(visualContainer, new Vector3(0.75f, 0.65f, 0.2f), Quaternion.Euler(15f, 0f, -35f), bodyMat);

        // Renderers önbelleğe al
        bodyRenderers = visualContainer.GetComponentsInChildren<Renderer>();
        originalBodyColor = bodyColor;
    }

    private void CreateHorn(Transform parent, Vector3 localPos, Vector3 rot, Material mat)
    {
        GameObject horn = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        horn.name = "Horn";
        horn.transform.SetParent(parent);
        horn.transform.localPosition = localPos;
        horn.transform.localScale = new Vector3(0.16f, 0.4f, 0.16f);
        horn.transform.localRotation = Quaternion.Euler(rot);
        StripCollider(horn);
        horn.GetComponent<Renderer>().material = mat;
    }

    private void CreateFang(Transform parent, Vector3 localPos, Material mat)
    {
        GameObject fang = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        fang.name = "Fang";
        fang.transform.SetParent(parent);
        fang.transform.localPosition = localPos;
        fang.transform.localScale = new Vector3(0.2f, 0.5f, 0.2f);
        fang.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
        StripCollider(fang);
        fang.GetComponent<Renderer>().material = mat;
    }

    private void CreateArm(Transform parent, Vector3 localPos, Quaternion rot, Material mat)
    {
        GameObject arm = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        arm.name = "Arm";
        arm.transform.SetParent(parent);
        arm.transform.localPosition = localPos;
        arm.transform.localScale = new Vector3(0.25f, 0.45f, 0.25f);
        arm.transform.localRotation = rot;
        StripCollider(arm);
        arm.GetComponent<Renderer>().material = mat;
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
        if (!Application.isPlaying) return;

        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        float dist = Vector3.Distance(transform.position, playerTransform.position);

        // Tetiklenme (Agro) kontrolü
        if (!isAggro && dist <= detectRadius)
        {
            isAggro = true;
            Debug.Log($"[Map10Enemy] Canavar tetiklendi! Oyuncuyu kovalıyor.");

            // Bobber varsa kapat ki hareket edebilsin
            Map10Bobber bobber = GetComponent<Map10Bobber>();
            if (bobber != null) bobber.enabled = false;

            // Spinner varsa kapat ki yüzünü dönebilsin
            Map10Spinner spinner = GetComponent<Map10Spinner>();
            if (spinner != null) spinner.enabled = false;
        }
        else if (isAggro && dist > loseRadius)
        {
            isAggro = false; // Oyuncu çok uzaklaşınca kovalamayı bırakır
        }

        // Oyuncuyu kovalama
        if (isAggro)
        {
            ChasePlayer();
        }

        // Vurulma parıltısı sayacı
        if (hitFlashTimer > 0f)
        {
            hitFlashTimer -= Time.deltaTime;
            if (hitFlashTimer <= 0f)
            {
                ResetColor();
            }
        }
    }

    private void ChasePlayer()
    {
        Vector3 targetPos = playerTransform.position;
        targetPos.y = transform.position.y; // Y ekseninde eğilmesin

        Vector3 moveDir = (targetPos - transform.position).normalized;
        float currentDist = Vector3.Distance(transform.position, targetPos);

        // Oyuncuya doğru dön
        if (moveDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // Oyuncunun dibindeyse (0.95m) ezmemek için durur (oyuncu rahatça zıplayıp aşabilir)
        if (currentDist > 0.95f)
        {
            if (rb != null)
            {
                Vector3 vel = moveDir * moveSpeed;
                vel.y = rb.linearVelocity.y;
                rb.linearVelocity = vel;
            }
            else
            {
                transform.position += moveDir * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            }
        }

        // Yürürken hafif canavar salınımı (waddle)
        if (visualContainer != null)
        {
            float waddle = Mathf.Sin(Time.time * 10f) * 8f;
            visualContainer.localRotation = Quaternion.Euler(0f, 0f, waddle);
        }
    }

    /// <summary>
    /// Mandalina mermisi çarpınca hasar alır
    /// </summary>
    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        currentHealth -= damage;
        Debug.Log($"[Map10Enemy] Canavar mandalinayla vuruldu! Kalan Can: {currentHealth}/{maxHealth}");

        // Vurulunca anında oyuncuya agro olur
        isAggro = true;

        // Hafif geriye itilme (knockback)
        if (rb != null && playerTransform != null)
        {
            Vector3 pushDir = (transform.position - playerTransform.position).normalized;
            pushDir.y = 0.2f;
            rb.AddForce(pushDir * 4f, ForceMode.Impulse);
        }

        // Vurulma flaşı (kırmızı/beyaz parlama)
        FlashColor(Color.white);

        // Canı bitti mi? (4-5 vuruş)
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void FlashColor(Color color)
    {
        if (bodyRenderers == null) return;
        hitFlashTimer = 0.15f;

        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            if (bodyRenderers[i] != null && bodyRenderers[i].material != null)
            {
                bodyRenderers[i].material.color = color;
            }
        }
    }

    private void ResetColor()
    {
        if (bodyRenderers == null) return;
        for (int i = 0; i < bodyRenderers.Length; i++)
        {
            if (bodyRenderers[i] != null && bodyRenderers[i].material != null)
            {
                bodyRenderers[i].material.color = originalBodyColor;
            }
        }
    }

    private void Die()
    {
        Debug.Log("[Map10Enemy] 💀 Canavar yenildi ve öldü!");

        // Ölüm parçalanma efekti (mandalina ile patlayan canavar parçacıkları)
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material deathMat = new Material(shader) { color = bodyColor };

        for (int i = 0; i < 10; i++)
        {
            GameObject chunk = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            chunk.name = "MonsterChunk";
            chunk.transform.position = transform.position + Random.insideUnitSphere * 0.4f;
            chunk.transform.localScale = Vector3.one * Random.Range(0.15f, 0.3f);

            Collider c = chunk.GetComponent<Collider>();
            if (c != null) Destroy(c);

            chunk.GetComponent<Renderer>().material = deathMat;

            Rigidbody cRb = chunk.AddComponent<Rigidbody>();
            cRb.mass = 0.1f;
            cRb.linearVelocity = (Random.onUnitSphere + Vector3.up * 1.2f) * Random.Range(3f, 6f);

            Destroy(chunk, 1.2f);
        }

        Destroy(gameObject);
    }

    void OnGUI()
    {
        // Tetiklenmiş canavarın üzerinde can barı (5 bölmeli) göster
        if (Application.isPlaying && isAggro && currentHealth > 0 && Camera.main != null)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.8f);

            if (screenPos.z > 0)
            {
                float barWidth = 90f;
                float barHeight = 12f;
                float x = screenPos.x - barWidth / 2f;
                float y = Screen.height - screenPos.y;

                // Arka plan kutusu
                GUI.Box(new Rect(x - 2, y - 2, barWidth + 4, barHeight + 4), "");

                // Doluluk oranı
                float healthPercent = (float)currentHealth / maxHealth;
                Color barColor = Color.Lerp(Color.red, Color.green, healthPercent);

                Texture2D barTex = new Texture2D(1, 1);
                barTex.SetPixel(0, 0, barColor);
                barTex.Apply();

                GUI.DrawTexture(new Rect(x, y, barWidth * healthPercent, barHeight), barTex);

                // Yazı
                GUIStyle style = new GUIStyle();
                style.fontSize = 11;
                style.fontStyle = FontStyle.Bold;
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                GUI.Label(new Rect(x, y - 1, barWidth, barHeight), $"{currentHealth}/{maxHealth}", style);
            }
        }
    }
}
