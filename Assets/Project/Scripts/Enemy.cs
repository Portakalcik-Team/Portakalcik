using UnityEngine;

namespace Portakalcik
{
    public enum EnemyType
    {
        DikenBocegi,
        GolgeYarasa,
        OrmanTilkisi,
        KokGolemi,
        LabirentMuhafizi
    }

    public class Enemy : MonoBehaviour
    {
        [Header("Canavar Bilgisi")]
        public EnemyType enemyType = EnemyType.DikenBocegi;
        public string monsterName = "Diken Böceği";

        [Header("Denge Sistemi (Seviye 6 Değerleri)")]
        public float maxHealth = 11.0f;
        public float currentHealth = 11.0f;
        public float attackDamage = 9.0f;
        public float attackCooldown = 2.0f;
        public float moveSpeed = 2.4f;

        [Header("Algılama & Farkındalık (Tepki Süresi)")]
        [Tooltip("Oyuncuyu algılama menzili")]
        public float detectionRadius = 9.0f;
        [Tooltip("Oyuncuyu ilk gördüğünde tepki süresi (sarı barın dolma süresi)")]
        public float alertReactionTime = 1.5f;
        [Tooltip("Maksimum kovalama mesafesi (bu mesafeyi aşınca peşini bırakır)")]
        public float maxChaseDistance = 14.0f;
        public float attackRadius = 1.6f;

        [Header("Kısayol Kapısı")]
        public Door guardedDoor;

        // Dahili Durumlar
        public float awareness = 0f; // 0 (huzurlu) - 1 (alarm)
        public bool isAlerted = false;
        private Vector3 spawnPosition;
        private Transform playerTarget;
        private PlayerController playerController;
        private float lastAttackTime = -99f;
        private bool isDead = false;

        // Görsel Parçalar (Küplerden İnsan Benzeri Model)
        private Renderer leftEyeRenderer;
        private Renderer rightEyeRenderer;
        private Material whiteEyeMat;
        private Material redEyeMat;
        private Transform modelRoot;

        private void Start()
        {
            spawnPosition = transform.position;
            currentHealth = maxHealth;

            // Oyuncuyu bul
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTarget = playerObj.transform;
                playerController = playerObj.GetComponent<PlayerController>();
            }

            BuildHumanoidModel();
        }

        private void BuildHumanoidModel()
        {
            // Eski collider/mesh varsa temizle, temiz gövde kuralım
            MeshRenderer baseMesh = GetComponent<MeshRenderer>();
            if (baseMesh != null) baseMesh.enabled = false;

            // Göz Materyalleri (Beyaz ve Kırmızı Parlama)
            whiteEyeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            whiteEyeMat.color = Color.white;
            whiteEyeMat.EnableKeyword("_EMISSION");
            whiteEyeMat.SetColor("_EmissionColor", Color.white * 1.5f);

            redEyeMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            redEyeMat.color = Color.red;
            redEyeMat.EnableKeyword("_EMISSION");
            redEyeMat.SetColor("_EmissionColor", Color.red * 2.0f);

            // Takımın Enemy materyalini al
            Material bodyMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Project/Art/Materials/Enemy.mat");
            if (bodyMat == null)
            {
                bodyMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                bodyMat.color = new Color(0.4f, 0.15f, 0.15f);
            }

            // Model Root
            GameObject mRoot = new GameObject("Humanoid_Model");
            mRoot.transform.parent = transform;
            mRoot.transform.localPosition = Vector3.zero;
            mRoot.transform.localRotation = Quaternion.identity;
            modelRoot = mRoot.transform;

            // Gövde (Torso)
            CreateBodyCube(mRoot.transform, "Gövde", new Vector3(0, 1.0f, 0), new Vector3(0.8f, 1.0f, 0.5f), bodyMat);

            // Baş (Head)
            Transform head = CreateBodyCube(mRoot.transform, "Bas", new Vector3(0, 1.75f, 0), new Vector3(0.55f, 0.55f, 0.55f), bodyMat).transform;

            // Gözler (Küplerden, başta Beyaz)
            GameObject leftEye = CreateBodyCube(head, "Sol_Goz", new Vector3(-0.14f, 0.05f, 0.28f), new Vector3(0.12f, 0.12f, 0.05f), whiteEyeMat);
            leftEyeRenderer = leftEye.GetComponent<Renderer>();

            GameObject rightEye = CreateBodyCube(head, "Sag_Goz", new Vector3(0.14f, 0.05f, 0.28f), new Vector3(0.12f, 0.12f, 0.05f), whiteEyeMat);
            rightEyeRenderer = rightEye.GetComponent<Renderer>();

            // Kollar (Arms)
            CreateBodyCube(mRoot.transform, "Sol_Kol", new Vector3(-0.55f, 1.0f, 0), new Vector3(0.25f, 0.9f, 0.25f), bodyMat);
            CreateBodyCube(mRoot.transform, "Sag_Kol", new Vector3(0.55f, 1.0f, 0), new Vector3(0.25f, 0.9f, 0.25f), bodyMat);

            // Bacaklar (Legs)
            CreateBodyCube(mRoot.transform, "Sol_Bacak", new Vector3(-0.25f, 0.35f, 0), new Vector3(0.3f, 0.7f, 0.3f), bodyMat);
            CreateBodyCube(mRoot.transform, "Sag_Bacak", new Vector3(0.25f, 0.35f, 0), new Vector3(0.3f, 0.7f, 0.3f), bodyMat);

            // Ana BoxCollider
            BoxCollider col = GetComponent<BoxCollider>();
            if (col == null) col = gameObject.AddComponent<BoxCollider>();
            col.center = new Vector3(0, 1.0f, 0);
            col.size = new Vector3(1.2f, 2.1f, 1.0f);
        }

        private GameObject CreateBodyCube(Transform parent, string name, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.parent = parent;
            cube.transform.localPosition = localPos;
            cube.transform.localScale = localScale;
            cube.transform.localRotation = Quaternion.identity;

            // Alt parçaların collider'ını kaldır (ana collider üstte var)
            Collider c = cube.GetComponent<Collider>();
            if (c != null) Destroy(c);

            Renderer r = cube.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;

            return cube;
        }

        private void Update()
        {
            if (isDead || playerTarget == null) return;

            float distToPlayer = Vector3.Distance(transform.position, playerTarget.position);
            float distToSpawn = Vector3.Distance(transform.position, spawnPosition);

            // 1. Görüş & Farkındalık Yönetimi
            if (distToPlayer <= detectionRadius && distToSpawn <= maxChaseDistance)
            {
                // Görüş alanında: Farkındalık yavaşça sarıya doğru dolar
                awareness = Mathf.MoveTowards(awareness, 1.0f, Time.deltaTime / alertReactionTime);

                // Oyuncuya doğru yavaşça dön
                Vector3 lookPos = playerTarget.position;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos);

                if (awareness >= 1.0f && !isAlerted)
                {
                    SetAlerted(true);
                }
            }
            else
            {
                // Görüşten çıktı veya çok uzaklaştı: Farkındalık yavaşça soğur
                awareness = Mathf.MoveTowards(awareness, 0f, Time.deltaTime * 0.8f);

                if (awareness <= 0f && isAlerted)
                {
                    SetAlerted(false);
                }
            }

            // 2. Alarm durumundaysa Kovalama ve Saldırı
            if (isAlerted)
            {
                // Maksimum kovalama mesafesi aşıldıysa kovalamayı bırak ve başlangıç noktasına dön
                if (distToSpawn > maxChaseDistance || distToPlayer > detectionRadius * 1.5f)
                {
                    SetAlerted(false);
                    return;
                }

                Vector3 targetPos = playerTarget.position;
                if (enemyType != EnemyType.GolgeYarasa)
                {
                    targetPos.y = transform.position.y;
                }

                if (distToPlayer > attackRadius)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

                    // Yürüme sallantısı (Body bobbing)
                    if (modelRoot != null)
                    {
                        float bob = Mathf.Sin(Time.time * 8f) * 0.05f;
                        modelRoot.localPosition = new Vector3(0, bob, 0);
                    }
                }
                else
                {
                    // Saldır
                    if (Time.time >= lastAttackTime + attackCooldown)
                    {
                        AttackPlayer();
                    }
                }
            }
            else if (distToSpawn > 0.5f)
            {
                // Alarmda değilse başlangıç noktasına geri dön
                Vector3 returnPos = spawnPosition;
                returnPos.y = transform.position.y;
                transform.position = Vector3.MoveTowards(transform.position, returnPos, moveSpeed * 0.5f * Time.deltaTime);
                transform.LookAt(returnPos);
            }
        }

        private void SetAlerted(bool alert)
        {
            isAlerted = alert;
            Material targetEyeMat = alert ? redEyeMat : whiteEyeMat;

            if (leftEyeRenderer != null) leftEyeRenderer.sharedMaterial = targetEyeMat;
            if (rightEyeRenderer != null) rightEyeRenderer.sharedMaterial = targetEyeMat;

            if (alert)
            {
                Debug.Log($"[Portakalcik] ❗ {monsterName} ALARM durumuna geçti! Gözleri KIRMIZI yandı!");
            }
        }

        private void AttackPlayer()
        {
            lastAttackTime = Time.time;
            if (playerController != null)
            {
                playerController.TakeDamage(attackDamage);
            }
        }

        public void TakeDamage(float amount, Vector3 knockbackDir)
        {
            if (isDead) return;

            currentHealth -= amount;
            awareness = 1.0f; // Darbe yediği an doğrudan alarma geçer
            SetAlerted(true);

            // Geriye itilme (Knockback vuruş hissiyatı)
            transform.position += knockbackDir.normalized * 0.4f;

            StartCoroutine(FlashHit());

            if (currentHealth <= 0f)
            {
                Die();
            }
        }

        private System.Collections.IEnumerator FlashHit()
        {
            // Vurulduğunda kırmızı parlayıp geri döner
            if (modelRoot != null)
            {
                Vector3 origScale = modelRoot.localScale;
                modelRoot.localScale = origScale * 1.15f; // Darbe anında esneme
                yield return new WaitForSeconds(0.12f);
                if (modelRoot != null) modelRoot.localScale = origScale;
            }
        }

        private void Die()
        {
            isDead = true;
            Debug.Log($"[Portakalcik] {monsterName} yenildi!");

            if (guardedDoor != null)
            {
                guardedDoor.OpenDoor();
            }

            Destroy(gameObject, 0.15f);
        }

        private void OnGUI()
        {
            if (isDead) return;

            Camera mainCam = Camera.main;
            if (mainCam == null) return;

            // Kafanın 2.4 birim üstündeki dünya koordinatı
            Vector3 headPos = transform.position + Vector3.up * 2.35f;
            Vector3 screenPos = mainCam.WorldToScreenPoint(headPos);

            // Kameranın önündeyse ve çok uzak değilse çiz
            if (screenPos.z > 0 && screenPos.z < 25f)
            {
                float barWidth = 80f;
                float barHeight = 8f;
                float x = screenPos.x - barWidth / 2f;
                float y = Screen.height - screenPos.y;

                // 1. CAN BARI (Health Bar)
                GUI.color = Color.black;
                GUI.DrawTexture(new Rect(x - 1, y - 1, barWidth + 2, barHeight + 2), Texture2D.whiteTexture);

                float hpPercent = Mathf.Clamp01(currentHealth / maxHealth);
                GUI.color = Color.red;
                GUI.DrawTexture(new Rect(x, y, barWidth, barHeight), Texture2D.whiteTexture);
                GUI.color = Color.green;
                GUI.DrawTexture(new Rect(x, y, barWidth * hpPercent, barHeight), Texture2D.whiteTexture);

                // 2. FARKINDALIK / TEPKİ GÖSTERGESİ (Sarı Çember/Bar)
                if (awareness > 0.05f)
                {
                    float awareBarY = y - 12f;
                    GUI.color = Color.black;
                    GUI.DrawTexture(new Rect(x - 1, awareBarY - 1, barWidth + 2, 6), Texture2D.whiteTexture);

                    // Sarıdan kırmızıya geçiş
                    GUI.color = isAlerted ? Color.red : Color.yellow;
                    GUI.DrawTexture(new Rect(x, awareBarY, barWidth * awareness, 4), Texture2D.whiteTexture);

                    GUIStyle textStyle = new GUIStyle();
                    textStyle.fontSize = 11;
                    textStyle.fontStyle = FontStyle.Bold;
                    textStyle.alignment = TextAnchor.MiddleCenter;
                    textStyle.normal.textColor = isAlerted ? Color.red : Color.yellow;

                    string stateText = isAlerted ? "❗ ALARM" : $"⚠️ %{(int)(awareness * 100)}";
                    GUI.Label(new Rect(x - 10, awareBarY - 16, barWidth + 20, 16), stateText, textStyle);
                }

                GUI.color = Color.white;
            }
        }
    }
}
