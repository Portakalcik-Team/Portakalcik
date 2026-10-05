using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Portakalcik
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Denge Sistemi - Can (HP)")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;

        [Header("Envanter (Meyveler)")]
        public int mandalinaCount = 6;
        public int portakalCount = 0;
        public int limonCount = 0;

        [Header("Hareket Ayarları")]
        public float walkSpeed = 6.0f;
        public float sprintSpeed = 9.0f;
        public float jumpForce = 5.0f;
        public float gravity = 20.0f;

        [Header("Kamera & Atış")]
        public Transform cameraHolder;
        public float mouseSensitivity = 0.15f;
        public float verticalLookLimit = 85.0f;
        public float shootCooldown = 0.35f;
        public float projectileSpeed = 24.0f;

        [Header("E Tuşuna Basılı Tutma (Hasat)")]
        public float requiredHoldTime = 1.2f;
        private float currentHoldTimer = 0f;
        private FruitTree currentTargetTree = null;

        [Header("Seviye & Süre")]
        public float parTime = 270.0f;
        public float elapsedTime = 0f;
        public bool isLevelComplete = false;
        public bool isGameOver = false;

        // Vuruş Hissiyatı & Kamera Sarsıntısı
        private float hitMarkerTimer = 0f;
        private float screenShakeTimer = 0f;
        private Vector3 originalCameraPos;

        // FPS Küp El Modeli
        private Transform fpsHandRoot;
        private Vector3 handInitialLocalPos = new Vector3(0.32f, -0.28f, 0.55f);
        private float handRecoil = 0f;

        private CharacterController characterController;
        private float verticalVelocity = 0f;
        private float cameraPitch = 0f;
        private float lastShootTime = -99f;
        private string activeMessage = "";
        private float messageTimer = 0f;

        private int totalScore = 0;
        private string finalRank = "";

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            currentHealth = maxHealth;

            if (cameraHolder == null)
            {
                Camera cam = GetComponentInChildren<Camera>();
                if (cam != null) cameraHolder = cam.transform;
            }

            if (cameraHolder != null)
            {
                originalCameraPos = cameraHolder.localPosition;
                BuildCubeHand();
            }
        }

        private void BuildCubeHand()
        {
            if (cameraHolder == null) return;

            // Küplerden FPS El Modeli
            GameObject handObj = new GameObject("FPS_Hand_Model");
            handObj.transform.parent = cameraHolder;
            handObj.transform.localPosition = handInitialLocalPos;
            handObj.transform.localRotation = Quaternion.Euler(15f, -10f, 5f);
            fpsHandRoot = handObj.transform;

            // Ten Rengi Materyal (Kol ve El)
            Material skinMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            skinMat.color = new Color(0.92f, 0.74f, 0.60f); // Açık ten tonu

            // Turuncu Mandalina Materyali
            Material mandalinaMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mandalinaMat.color = new Color(1.0f, 0.5f, 0.0f); // Portakal turuncusu

            // Kol (Forearm)
            CreateHandPart(fpsHandRoot, "Kol", new Vector3(0, -0.1f, -0.2f), new Vector3(0.14f, 0.16f, 0.45f), skinMat);

            // Avuç / El (Palm)
            CreateHandPart(fpsHandRoot, "El", new Vector3(0, 0, 0.08f), new Vector3(0.16f, 0.14f, 0.18f), skinMat);

            // Parmaklar (Fingers)
            CreateHandPart(fpsHandRoot, "Parmaklar", new Vector3(0, 0.04f, 0.20f), new Vector3(0.14f, 0.08f, 0.12f), skinMat);

            // Elde Tutulan Mandalina (Cube/Sphere)
            CreateHandPart(fpsHandRoot, "Eldeki_Mandalina", new Vector3(0, 0.12f, 0.12f), new Vector3(0.16f, 0.16f, 0.16f), mandalinaMat);
        }

        private GameObject CreateHandPart(Transform parent, string name, Vector3 localPos, Vector3 localScale, Material mat)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.parent = parent;
            cube.transform.localPosition = localPos;
            cube.transform.localScale = localScale;
            cube.transform.localRotation = Quaternion.identity;

            Collider c = cube.GetComponent<Collider>();
            if (c != null) Destroy(c);

            Renderer r = cube.GetComponent<Renderer>();
            if (r != null && mat != null) r.sharedMaterial = mat;

            return cube;
        }

        private void Start()
        {
            LockCursor();
        }

        private void Update()
        {
            if (isGameOver || isLevelComplete)
            {
                UnlockCursor();
                return;
            }

            elapsedTime += Time.deltaTime;

            if (messageTimer > 0f)
            {
                messageTimer -= Time.deltaTime;
                if (messageTimer <= 0f) activeMessage = "";
            }

            // Hit Marker Süresi
            if (hitMarkerTimer > 0f) hitMarkerTimer -= Time.deltaTime;

            // Kamera Sarsıntısı (Screen Shake)
            if (screenShakeTimer > 0f)
            {
                screenShakeTimer -= Time.deltaTime;
                if (cameraHolder != null)
                {
                    cameraHolder.localPosition = originalCameraPos + Random.insideUnitSphere * 0.04f;
                }
            }
            else if (cameraHolder != null && cameraHolder.localPosition != originalCameraPos)
            {
                cameraHolder.localPosition = originalCameraPos;
            }

            HandleCursorLock();
            HandleLook();
            HandleMovement();
            HandleHoldToHarvest();
            HandleShooting();
            AnimateHand();
        }

        private void AnimateHand()
        {
            if (fpsHandRoot == null) return;

            // Yürüme Salınımı (Weapon Bobbing)
            float moveMagnitude = characterController.velocity.magnitude;
            float bobX = Mathf.Cos(Time.time * 7f) * 0.015f * (moveMagnitude > 0.1f ? 1f : 0.2f);
            float bobY = Mathf.Sin(Time.time * 14f) * 0.015f * (moveMagnitude > 0.1f ? 1f : 0.2f);

            // Geri Tepme (Recoil recovery)
            handRecoil = Mathf.MoveTowards(handRecoil, 0f, Time.deltaTime * 3.5f);

            fpsHandRoot.localPosition = handInitialLocalPos + new Vector3(bobX, bobY, -handRecoil);
        }

        private void HandleCursorLock()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) UnlockCursor();
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked) LockCursor();
#else
            if (Input.GetKeyDown(KeyCode.Escape)) UnlockCursor();
            if (Input.GetMouseButtonDown(0) && Cursor.lockState != CursorLockMode.Locked) LockCursor();
#endif
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void HandleLook()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            Vector2 mouseDelta = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;
#else
            mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * 10f;
#endif
            transform.Rotate(Vector3.up * mouseDelta.x);

            if (cameraHolder != null)
            {
                cameraPitch -= mouseDelta.y;
                cameraPitch = Mathf.Clamp(cameraPitch, -verticalLookLimit, verticalLookLimit);
                cameraHolder.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            }
        }

        private void HandleMovement()
        {
            Vector2 inputDir = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed) inputDir.y += 1f;
                if (Keyboard.current.sKey.isPressed) inputDir.y -= 1f;
                if (Keyboard.current.aKey.isPressed) inputDir.x -= 1f;
                if (Keyboard.current.dKey.isPressed) inputDir.x += 1f;
            }
#else
            inputDir.x = Input.GetAxisRaw("Horizontal");
            inputDir.y = Input.GetAxisRaw("Vertical");
#endif
            inputDir.Normalize();

            bool isSprinting = false;
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) isSprinting = true;
#else
            if (Input.GetKey(KeyCode.LeftShift)) isSprinting = true;
#endif
            float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
            Vector3 move = (transform.forward * inputDir.y + transform.right * inputDir.x) * currentSpeed;

            if (characterController.isGrounded)
            {
                if (verticalVelocity < 0f) verticalVelocity = -2f;

                bool jumpPressed = false;
#if ENABLE_INPUT_SYSTEM
                if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) jumpPressed = true;
#else
                if (Input.GetButtonDown("Jump")) jumpPressed = true;
#endif
                if (jumpPressed) verticalVelocity = jumpForce;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            move.y = verticalVelocity;
            characterController.Move(move * Time.deltaTime);
        }

        private void HandleHoldToHarvest()
        {
            // Yakındaki ağacı tespit et
            FruitTree[] trees = Object.FindObjectsByType<FruitTree>(FindObjectsSortMode.None);
            currentTargetTree = null;
            float nearestDist = 3.5f;

            foreach (var t in trees)
            {
                if (t != null && !t.isHarvested)
                {
                    float d = Vector3.Distance(transform.position, t.transform.position);
                    if (d <= nearestDist)
                    {
                        nearestDist = d;
                        currentTargetTree = t;
                    }
                }
            }

            if (currentTargetTree != null)
            {
                bool isHoldingE = false;
#if ENABLE_INPUT_SYSTEM
                if (Keyboard.current != null && Keyboard.current.eKey.isPressed) isHoldingE = true;
#else
                if (Input.GetKey(KeyCode.E)) isHoldingE = true;
#endif
                if (isHoldingE)
                {
                    currentHoldTimer += Time.deltaTime;
                    if (currentHoldTimer >= requiredHoldTime)
                    {
                        // Hasat Tamamlandı!
                        currentHoldTimer = 0f;
                        int yield = currentTargetTree.Harvest();

                        if (currentTargetTree.treeType == TreeType.Mandalina)
                        {
                            mandalinaCount += yield;
                            ShowMessage($"🍊 +{yield} Mandalina Toplandı!", 2.5f);
                        }
                        else if (currentTargetTree.treeType == TreeType.Portakal)
                        {
                            portakalCount += yield;
                            ShowMessage($"🍊 +{yield} Portakal Toplandı! (Çıkış Anahtarı Hazır!)", 3.0f);
                        }
                        else if (currentTargetTree.treeType == TreeType.Limon)
                        {
                            limonCount += yield;
                            ShowMessage($"🍋 +{yield} Limon Toplandı! (Soru/İpucu)", 2.5f);
                        }
                    }
                }
                else
                {
                    // Tuş bırakıldıysa ilerlemeyi sıfırla
                    currentHoldTimer = 0f;
                }
            }
            else
            {
                currentHoldTimer = 0f;
            }
        }

        private void HandleShooting()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;

            bool shootPressed = false;
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) shootPressed = true;
#else
            if (Input.GetMouseButtonDown(0)) shootPressed = true;
#endif
            if (!shootPressed) return;
            if (Time.time < lastShootTime + shootCooldown) return;

            if (mandalinaCount <= 0)
            {
                ShowMessage("⚠️ Mandalina kalmadı! Ağaçlardan [E] ile topla.", 1.5f);
                return;
            }

            lastShootTime = Time.time;
            mandalinaCount--;

            // El Geri Tepme Hareketi
            handRecoil = 0.14f;

            // Mermi Fırlat
            Transform spawnPoint = cameraHolder != null ? cameraHolder : transform;
            Vector3 spawnPos = spawnPoint.position + spawnPoint.forward * 0.75f;

            GameObject proj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            proj.name = "Mandalina_Projectile";
            proj.transform.position = spawnPos;
            proj.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            Renderer r = proj.GetComponent<Renderer>();
            if (r != null) r.material.color = new Color(1.0f, 0.5f, 0.0f);

            Rigidbody rb = proj.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.linearVelocity = spawnPoint.forward * projectileSpeed;

            proj.AddComponent<MandalinaProjectile>();
        }

        public void TriggerHitFeedback()
        {
            // Vuruş Hissiyatı: Nişangah Kırmızı X (Hit Marker) ve Hafif Kamera Sarsıntısı
            hitMarkerTimer = 0.22f;
            screenShakeTimer = 0.08f;
        }

        public void TakeDamage(float amount)
        {
            if (isGameOver || isLevelComplete) return;

            currentHealth -= amount;
            screenShakeTimer = 0.15f; // Hasar yiyince sarsıntı
            ShowMessage($"💥 Canavar saldırdı! -{amount:0} Can", 1.5f);

            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                isGameOver = true;
                UnlockCursor();
            }
        }

        public void ShowMessage(string msg, float duration = 2.0f)
        {
            activeMessage = msg;
            messageTimer = duration;
        }

        public void CompleteLevel()
        {
            if (isLevelComplete) return;
            isLevelComplete = true;
            UnlockCursor();

            int completionScore = 500 + (6 * 100);
            int timeBonus = Mathf.Max(0, (int)((parTime - elapsedTime) * 3));
            int mandarinBonus = mandalinaCount * 20;
            int orangeBonus = Mathf.Max(0, (portakalCount - 1) * 30);
            int lemonBonus = limonCount * 50;

            totalScore = completionScore + timeBonus + mandarinBonus + orangeBonus + lemonBonus;

            if (totalScore >= 1800) finalRank = "S (Efsanevi)";
            else if (totalScore >= 1400) finalRank = "A (Harika)";
            else if (totalScore >= 1000) finalRank = "B (İyi)";
            else finalRank = "C (Orta)";
        }

        private void OnGUI()
        {
            // 1. Ortada Nişangah (Crosshair) & HIT MARKER (Vuruş Bildirimi)
            if (!isGameOver && !isLevelComplete)
            {
                float cx = Screen.width / 2f;
                float cy = Screen.height / 2f;

                if (hitMarkerTimer > 0f)
                {
                    // Kırmızı Vuruş Çaprazı (Hit Marker)
                    GUI.color = Color.red;
                    GUI.DrawTexture(new Rect(cx - 7, cy - 7, 4, 4), Texture2D.whiteTexture);
                    GUI.DrawTexture(new Rect(cx + 4, cy - 7, 4, 4), Texture2D.whiteTexture);
                    GUI.DrawTexture(new Rect(cx - 7, cy + 4, 4, 4), Texture2D.whiteTexture);
                    GUI.DrawTexture(new Rect(cx + 4, cy + 4, 4, 4), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }
                else
                {
                    // Standart Nokta Nişangah
                    GUI.Box(new Rect(cx - 3, cy - 3, 6, 6), GUIContent.none);
                }
            }

            // 2. Sol Üst: Can ve Envanter Bilgisi
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.alignment = TextAnchor.UpperLeft;
            boxStyle.fontSize = 15;
            boxStyle.fontStyle = FontStyle.Bold;

            GUILayout.BeginArea(new Rect(20, 20, 280, 150), boxStyle);
            GUILayout.Space(5);
            Color origColor = GUI.color;
            GUI.color = currentHealth > 30f ? Color.green : Color.red;
            GUILayout.Label($"❤️ CAN: {currentHealth:0} / {maxHealth:0} HP");
            GUI.color = origColor;
            GUILayout.Label($"🍊 Mandalina (Cephane): {mandalinaCount}");
            GUILayout.Label($"🍊 Portakal (Anahtar): {portakalCount} / 1 {(portakalCount >= 1 ? "✅" : "❌")}");
            GUILayout.Label($"🍋 Limon (Soru): {limonCount}");
            GUILayout.EndArea();

            // 3. Sağ Üst: Süre Bilgisi
            int min = (int)(elapsedTime / 60);
            int sec = (int)(elapsedTime % 60);
            GUIStyle timeStyle = new GUIStyle(GUI.skin.box);
            timeStyle.fontSize = 15;
            timeStyle.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(Screen.width - 220, 20, 200, 45), $"⏱️ Süre: {min:00}:{sec:00}\n🎯 Hedef: 04:30", timeStyle);

            // 4. E Tuşuna Basılı Tutma (Hasat İlerleme Barı)
            if (currentTargetTree != null && !isGameOver && !isLevelComplete)
            {
                float barW = 220f;
                float barH = 18f;
                float bx = (Screen.width - barW) / 2f;
                float by = Screen.height / 2f + 50f;

                GUIStyle hintStyle = new GUIStyle();
                hintStyle.fontSize = 17;
                hintStyle.alignment = TextAnchor.MiddleCenter;
                hintStyle.fontStyle = FontStyle.Bold;
                hintStyle.normal.textColor = Color.yellow;

                if (currentHoldTimer > 0f)
                {
                    float pct = Mathf.Clamp01(currentHoldTimer / requiredHoldTime);
                    GUI.Label(new Rect(0, by - 28, Screen.width, 24), $"Toplanıyor... %{(int)(pct * 100)}", hintStyle);

                    GUI.color = Color.black;
                    GUI.DrawTexture(new Rect(bx - 2, by - 2, barW + 4, barH + 4), Texture2D.whiteTexture);
                    GUI.color = new Color(1.0f, 0.55f, 0.0f); // Turuncu dolum
                    GUI.DrawTexture(new Rect(bx, by, barW * pct, barH), Texture2D.whiteTexture);
                    GUI.color = Color.white;
                }
                else
                {
                    GUI.Label(new Rect(0, by - 10, Screen.width, 24), $"[E] Basılı Tutarak {currentTargetTree.treeType} Topla", hintStyle);
                }
            }
            else if (!string.IsNullOrEmpty(activeMessage) && !isGameOver && !isLevelComplete)
            {
                GUIStyle msgStyle = new GUIStyle();
                msgStyle.fontSize = 19;
                msgStyle.alignment = TextAnchor.MiddleCenter;
                msgStyle.fontStyle = FontStyle.Bold;
                msgStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(0, Screen.height / 2f + 40, Screen.width, 35), activeMessage, msgStyle);
            }

            // ZAFER EKRANI
            if (isLevelComplete)
            {
                float panelW = 420;
                float panelH = 340;
                Rect winRect = new Rect((Screen.width - panelW) / 2f, (Screen.height - panelH) / 2f, panelW, panelH);
                GUI.Box(winRect, "🎉 TEBRİKLER! HARİTA 6 TAMAMLANDI! 🎉");
                GUILayout.BeginArea(new Rect(winRect.x + 20, winRect.y + 40, panelW - 40, panelH - 50));
                GUILayout.Label($"🏆 Toplam Skor: {totalScore} Puan");
                GUILayout.Label($"🎖️ Başarı Derecesi: {finalRank}");
                GUILayout.Space(10);
                GUILayout.Label($"⏱️ Geçen Süre: {min:00}:{sec:00}");
                GUILayout.Label($"🍊 Kalan Mandalina Bonusu: +{mandalinaCount * 20} Puan");
                GUILayout.Label($"🍊 Ekstra Portakal Bonusu: +{Mathf.Max(0, (portakalCount - 1) * 30)} Puan");
                GUILayout.Label($"🍋 Limon Bonusu: +{limonCount * 50} Puan");
                GUILayout.Space(15);
                if (GUILayout.Button("Tekrar Oyna", GUILayout.Height(35)))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }
                GUILayout.EndArea();
            }

            // YENİLGİ EKRANI
            if (isGameOver)
            {
                float panelW = 380;
                float panelH = 220;
                Rect overRect = new Rect((Screen.width - panelW) / 2f, (Screen.height - panelH) / 2f, panelW, panelH);
                GUI.Box(overRect, "💀 OYUN BİTTİ (Canavarlar Seni Yendi!)");
                GUILayout.BeginArea(new Rect(overRect.x + 20, overRect.y + 50, panelW - 40, panelH - 60));
                GUILayout.Label("Canın tükendi! Denge dokümanına göre -380 puan ceza uygulandı.");
                GUILayout.Space(20);
                if (GUILayout.Button("Yeniden Başla", GUILayout.Height(40)))
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                }
                GUILayout.EndArea();
            }
        }
    }
}
