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
        [Tooltip("Denge dokümanına göre temel oyuncu canı 100 HP")]
        public float maxHealth = 100f;
        public float currentHealth = 100f;

        [Header("Envanter (Meyveler)")]
        [Tooltip("Canavarlara atılan mühimmat (vuruş başı 3 hasar)")]
        public int mandalinaCount = 6; // Başlangıç mandalinası
        [Tooltip("Çıkış kapısını açmak için en az 1 portakal şart")]
        public int portakalCount = 0;
        [Tooltip("Soru ve ipucu hakkı")]
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

        [Header("Seviye & Süre (Harita 6: Par Süresi 270s)")]
        public float parTime = 270.0f; // 4.5 dakika
        public float elapsedTime = 0f;
        public bool isLevelComplete = false;
        public bool isGameOver = false;

        private CharacterController characterController;
        private float verticalVelocity = 0f;
        private float cameraPitch = 0f;
        private float lastShootTime = -99f;
        private string activeMessage = "";
        private float messageTimer = 0f;

        // Skor Değişkenleri
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

            HandleCursorLock();
            HandleLook();
            HandleMovement();
            HandleInteraction();
            HandleShooting();
        }

        private void HandleCursorLock()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                UnlockCursor();
            }
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor();
            }
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
            if (Mouse.current != null)
            {
                mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity;
            }
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

        private void HandleInteraction()
        {
            // Yakındaki ağaçları kontrol et
            FruitTree[] trees = Object.FindObjectsByType<FruitTree>(FindObjectsSortMode.None);
            FruitTree nearestTree = null;
            float nearestDist = 3.5f;

            foreach (var t in trees)
            {
                if (t != null && !t.isHarvested)
                {
                    float d = Vector3.Distance(transform.position, t.transform.position);
                    if (d <= nearestDist)
                    {
                        nearestDist = d;
                        nearestTree = t;
                    }
                }
            }

            if (nearestTree != null)
            {
                ShowMessage($"[E] {nearestTree.treeType} Ağacını Hasat Et (+{nearestTree.fruitYield})", 0.1f);

                bool interactPressed = false;
#if ENABLE_INPUT_SYSTEM
                if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) interactPressed = true;
#else
                if (Input.GetKeyDown(KeyCode.E)) interactPressed = true;
#endif
                if (interactPressed)
                {
                    int yield = nearestTree.Harvest();
                    if (nearestTree.treeType == TreeType.Mandalina)
                    {
                        mandalinaCount += yield;
                        ShowMessage($"🍊 +{yield} Mandalina Cephanesi Toplandı!", 2.0f);
                    }
                    else if (nearestTree.treeType == TreeType.Portakal)
                    {
                        portakalCount += yield;
                        ShowMessage($"🍊 +{yield} Portakal Toplandı! (Çıkış Anahtarı Hazır!)", 3.0f);
                    }
                    else if (nearestTree.treeType == TreeType.Limon)
                    {
                        limonCount += yield;
                        ShowMessage($"🍋 +{yield} Limon Toplandı! (Soru/İpucu)", 2.5f);
                    }
                }
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
                ShowMessage("⚠️ Mandalina kalmadı! Ağaçlardan topla.", 1.5f);
                return;
            }

            lastShootTime = Time.time;
            mandalinaCount--;

            // Mandalina Mermisi Oluştur
            Transform spawnPoint = cameraHolder != null ? cameraHolder : transform;
            Vector3 spawnPos = spawnPoint.position + spawnPoint.forward * 0.8f;

            GameObject proj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            proj.name = "Mandalina_Projectile";
            proj.transform.position = spawnPos;
            proj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

            // Turuncu Mandalina Materyali
            Renderer r = proj.GetComponent<Renderer>();
            if (r != null)
            {
                r.material.color = new Color(1.0f, 0.5f, 0.0f); // Portakal turuncusu
            }

            Rigidbody rb = proj.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.linearVelocity = spawnPoint.forward * projectileSpeed;

            proj.AddComponent<MandalinaProjectile>();
        }

        public void TakeDamage(float amount)
        {
            if (isGameOver || isLevelComplete) return;

            currentHealth -= amount;
            ShowMessage($"💥 Canavar saldırdı! -{amount:0} Can", 1.5f);

            if (currentHealth <= 0f)
            {
                currentHealth = 0f;
                isGameOver = true;
                UnlockCursor();
                Debug.Log("[Portakalcik] OYUN BİTTİ (GAME OVER)");
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

            // Denge dokümanına göre puan hesabı (Seviye 6)
            int completionScore = 500 + (6 * 100); // 1100 Puan
            int timeBonus = Mathf.Max(0, (int)((parTime - elapsedTime) * 3));
            int mandarinBonus = mandalinaCount * 20;
            int orangeBonus = Mathf.Max(0, (portakalCount - 1) * 30);
            int lemonBonus = limonCount * 50;

            totalScore = completionScore + timeBonus + mandarinBonus + orangeBonus + lemonBonus;

            if (totalScore >= 1800) finalRank = "S (Efsanevi)";
            else if (totalScore >= 1400) finalRank = "A (Harika)";
            else if (totalScore >= 1000) finalRank = "B (İyi)";
            else finalRank = "C (Orta)";

            Debug.Log($"[Portakalcik] SEVİYE 6 TAMAMLANDI! Toplam Puan: {totalScore}, Derece: {finalRank}");
        }

        private void OnGUI()
        {
            // Ortada Nişangah (Crosshair)
            if (!isGameOver && !isLevelComplete)
            {
                float cx = Screen.width / 2f;
                float cy = Screen.height / 2f;
                GUI.Box(new Rect(cx - 3, cy - 3, 6, 6), GUIContent.none);
            }

            // Sol Üst: Can ve Envanter Bilgisi
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.alignment = TextAnchor.UpperLeft;
            boxStyle.fontSize = 15;
            boxStyle.fontStyle = FontStyle.Bold;

            GUILayout.BeginArea(new Rect(20, 20, 280, 150), boxStyle);
            GUILayout.Space(5);

            // Can Barı
            Color origColor = GUI.color;
            GUI.color = currentHealth > 30f ? Color.green : Color.red;
            GUILayout.Label($"❤️ CAN: {currentHealth:0} / {maxHealth:0} HP");
            GUI.color = origColor;

            GUILayout.Label($"🍊 Mandalina (Cephane): {mandalinaCount}");
            GUILayout.Label($"🍊 Portakal (Anahtar): {portakalCount} / 1 {(portakalCount >= 1 ? "✅" : "❌")}");
            GUILayout.Label($"🍋 Limon (Soru): {limonCount}");
            GUILayout.EndArea();

            // Sağ Üst: Süre Bilgisi
            int min = (int)(elapsedTime / 60);
            int sec = (int)(elapsedTime % 60);
            GUIStyle timeStyle = new GUIStyle(GUI.skin.box);
            timeStyle.fontSize = 15;
            timeStyle.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(Screen.width - 220, 20, 200, 45), $"⏱️ Süre: {min:00}:{sec:00}\n🎯 Hedef: 04:30", timeStyle);

            // Orta Ekran Bildirim / Etkileşim Mesajı
            if (!string.IsNullOrEmpty(activeMessage) && !isGameOver && !isLevelComplete)
            {
                GUIStyle msgStyle = new GUIStyle();
                msgStyle.fontSize = 20;
                msgStyle.alignment = TextAnchor.MiddleCenter;
                msgStyle.fontStyle = FontStyle.Bold;
                msgStyle.normal.textColor = Color.yellow;
                GUI.Label(new Rect(0, Screen.height / 2f + 40, Screen.width, 35), activeMessage, msgStyle);
            }

            // ZAFER EKRANI (Level Complete)
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

            // YENİLGİ EKRANI (Game Over)
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
