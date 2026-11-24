using UnityEngine;
using UnityEngine.InputSystem;

public class Map10CameraFollow : MonoBehaviour
{
    public static Map10CameraFollow Instance { get; private set; }

    [Header("FPV Hedef ve Göz Seviyesi")]
    [SerializeField] private Transform target;
    [SerializeField] private float eyeHeight = 1.35f;
    [SerializeField] private float eyeForwardOffset = 0.15f;

    [Header("Fare Hassasiyeti")]
    [SerializeField] private float mouseSensitivity = 2.2f;
    [SerializeField] private float minPitch = -75f;
    [SerializeField] private float maxPitch = 75f;

    [Header("Eldeki Silah Ayarları")]
    [SerializeField] private bool showWeapon = true;
    [SerializeField] private Vector3 gunOffset = new Vector3(0.24f, -0.22f, 0.48f);

    private float pitch = 0f;
    private float yaw = 0f;
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0.4f;

    // Eldeki silah parçaları ve animasyon
    private Transform gunContainer;
    private Transform gunBarrel;
    private Transform gunAmmoDisplay;
    private float recoilAmount = 0f;
    private float bobTimer = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }

        if (target != null)
        {
            yaw = target.eulerAngles.y;
        }

        // Duvarların uzakta kaybolmaması ve ufkun gözükmemesi için görüş mesafesini artır
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.farClipPlane = 200f;
        }

        // Fare imlecini kilitle ve gizle (FPS modu)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Eldeki silahı oluştur
        if (showWeapon)
        {
            BuildFPSWeapon();
        }
    }

    public void Shake(float duration = 1.2f, float magnitude = 0.4f)
    {
        shakeTimer = duration;
        shakeMagnitude = magnitude;
    }

    /// <summary>
    /// Ateş edildiğinde silahın geriye tepme efekti
    /// </summary>
    public void TriggerGunRecoil()
    {
        recoilAmount = 0.12f;
    }

    private void BuildFPSWeapon()
    {
        // Varsa eskisi silinsin
        Transform old = transform.Find("FPSGunContainer");
        if (old != null) Destroy(old.gameObject);

        GameObject container = new GameObject("FPSGunContainer");
        gunContainer = container.transform;
        gunContainer.SetParent(transform);
        gunContainer.localPosition = gunOffset;
        gunContainer.localRotation = Quaternion.Euler(0f, -5f, 0f);

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material metalMat = new Material(shader) { color = new Color(0.25f, 0.28f, 0.32f) }; // Silah gövdesi
        Material orangeMat = new Material(shader) { color = new Color(1f, 0.5f, 0f) };        // Turuncu mandalina detayları
        Material gripMat = new Material(shader) { color = new Color(0.18f, 0.12f, 0.08f) };    // Kabza kahverengisi

        // 1. Silah Gövdesi (Body)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "GunBody";
        body.transform.SetParent(gunContainer);
        body.transform.localPosition = new Vector3(0f, 0f, 0f);
        body.transform.localScale = new Vector3(0.08f, 0.1f, 0.28f);
        StripCollider(body);
        body.GetComponent<Renderer>().material = metalMat;

        // 2. Namlu (Barrel)
        GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        barrel.name = "GunBarrel";
        gunBarrel = barrel.transform;
        gunBarrel.SetParent(gunContainer);
        gunBarrel.localPosition = new Vector3(0f, 0.02f, 0.16f);
        gunBarrel.localScale = new Vector3(0.065f, 0.1f, 0.065f);
        gunBarrel.localRotation = Quaternion.Euler(90f, 0f, 0f);
        StripCollider(barrel);
        barrel.GetComponent<Renderer>().material = metalMat;

        // 3. Mandalina Yükleme Yuvası / Şarjör
        GameObject chamber = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        chamber.name = "AmmoChamber";
        gunAmmoDisplay = chamber.transform;
        gunAmmoDisplay.SetParent(gunContainer);
        gunAmmoDisplay.localPosition = new Vector3(0f, 0.07f, -0.02f);
        gunAmmoDisplay.localScale = new Vector3(0.065f, 0.065f, 0.065f);
        StripCollider(chamber);
        chamber.GetComponent<Renderer>().material = orangeMat;

        // 4. Kabza (Grip)
        GameObject grip = GameObject.CreatePrimitive(PrimitiveType.Cube);
        grip.name = "GunGrip";
        grip.transform.SetParent(gunContainer);
        grip.transform.localPosition = new Vector3(0f, -0.09f, -0.05f);
        grip.transform.localScale = new Vector3(0.065f, 0.12f, 0.07f);
        grip.transform.localRotation = Quaternion.Euler(20f, 0f, 0f);
        StripCollider(grip);
        grip.GetComponent<Renderer>().material = gripMat;
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
        // Escape tuşuna basınca fare kilidini aç / kapat
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        // Fare girdisi ile kamera bakışı
        if (Cursor.lockState == CursorLockMode.Locked && Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue() * mouseSensitivity * 0.1f;

            yaw += mouseDelta.x;
            pitch -= mouseDelta.y;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            // Karakterin rotasyonunu yatayda (yaw) döndür
            if (target != null)
            {
                target.rotation = Quaternion.Euler(0f, yaw, 0f);
            }
        }

        // Silah süzülmesi ve tepme (Bobbing & Recoil)
        UpdateWeaponAnimation();
    }

    private void UpdateWeaponAnimation()
    {
        if (gunContainer == null) return;

        // Geriye tepme sönümlenmesi
        recoilAmount = Mathf.Lerp(recoilAmount, 0f, Time.deltaTime * 12f);

        // Yürüme salınımı (Bobbing)
        Vector3 bobOffset = Vector3.zero;
        if (target != null)
        {
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
                if (speed > 0.1f)
                {
                    bobTimer += Time.deltaTime * 8f;
                    bobOffset.x = Mathf.Cos(bobTimer) * 0.015f;
                    bobOffset.y = Mathf.Sin(bobTimer * 2f) * 0.012f;
                }
            }
        }

        // Silah pozisyonunu güncelle
        Vector3 targetPos = gunOffset + bobOffset - Vector3.forward * recoilAmount;
        gunContainer.localPosition = Vector3.Lerp(gunContainer.localPosition, targetPos, Time.deltaTime * 18f);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Göz seviyesi FPV pozisyonu
        Vector3 eyePos = target.position + Vector3.up * eyeHeight + target.forward * eyeForwardOffset;

        // Kamera sarsıntısı varsa ekle
        if (shakeTimer > 0f)
        {
            eyePos += Random.insideUnitSphere * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }

        transform.position = eyePos;
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void OnGUI()
    {
        // Ekranın tam ortasına şık küçük bir Nişangah (Crosshair)
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            float centerX = Screen.width / 2f;
            float centerY = Screen.height / 2f;
            float size = 4f;

            GUI.color = new Color(1f, 0.8f, 0.2f, 0.85f); // Tatlı sarı/turuncu nokta
            GUI.DrawTexture(new Rect(centerX - size / 2f, centerY - size / 2f, size, size), Texture2D.whiteTexture);

            // Minik artı çizgileri
            float lineLen = 6f;
            float lineThick = 1.5f;
            GUI.DrawTexture(new Rect(centerX - 10f - lineLen, centerY - lineThick / 2f, lineLen, lineThick), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(centerX + 10f, centerY - lineThick / 2f, lineLen, lineThick), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(centerX - lineThick / 2f, centerY - 10f - lineLen, lineThick, lineLen), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(centerX - lineThick / 2f, centerY + 10f, lineThick, lineLen), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
