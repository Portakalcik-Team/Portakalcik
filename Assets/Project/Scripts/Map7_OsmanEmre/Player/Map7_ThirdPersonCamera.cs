using UnityEngine;
using UnityEngine.InputSystem;

// Map 7 oyuncu kamerası: third person (oyuncunun etrafında dönen) ve first person (gözden bakış) modları.
// V tuşu iki mod arasında geçiş yapar. Mouse kamerayı döndürür.
// Esc imleci serbest bırakır, ekrana tıklayınca imleç tekrar kilitlenir.
// Hareket kameraya göre yapıldığı için W her zaman ekranda ileri gider (bkz. Map7_PlayerMove).
public class Map7_ThirdPersonCamera : MonoBehaviour
{
    [Header("Mod")]
    [SerializeField] private bool firstPerson = false;          // true: oyun first person başlar
    [SerializeField] private Key toggleKey = Key.V;             // mod değiştirme tuşu

    [Header("Third person")]
    [SerializeField] private float distance = 7f;               // oyuncuya uzaklık
    [SerializeField] private float lookHeight = 1f;             // bakılan noktanın yüksekliği
    [SerializeField] private float minPitch = 10f;              // en alçak kamera açısı
    [SerializeField] private float maxPitch = 70f;              // en yüksek kamera açısı
    [SerializeField] private float followSmoothTime = 0.08f;    // takip yumuşaklığı

    [Header("First person")]
    [SerializeField] private float eyeHeight = 0.6f;            // oyuncu merkezinden göz yüksekliği
    [SerializeField] private float eyeForward = 0.15f;          // gözü biraz öne al (gövdenin içini görmemek için)
    [SerializeField] private float fpMinPitch = -80f;           // en fazla yukarı bakış
    [SerializeField] private float fpMaxPitch = 80f;            // en fazla aşağı bakış
    [SerializeField] private float nearClip = 0.05f;            // first person'da yakın kesme mesafesi

    [Header("Ortak")]
    [SerializeField] private float yaw = 180f;                  // başlangıç yatay açı (180 = oyuncunun karşısından bakar)
    [SerializeField] private float pitch = 30f;                 // başlangıç dikey açı
    [SerializeField] private float mouseSensitivity = 0.15f;    // piksel başına derece
    [SerializeField] private bool lockCursor = true;            // oyun başında imleci kilitle

    // Diğer scriptlerin (örn. meyve kepçesi) hangi modda olduğumuzu öğrenmesi için
    public bool IsFirstPerson => firstPerson;

    private Transform target;
    private Camera cam;
    private Renderer[] bodyRenderers;
    private float defaultNearClip;
    private Vector3 pivot;          // yumuşatılmış bakış noktası
    private Vector3 pivotVelocity;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null) defaultNearClip = cam.nearClipPlane;
        SetCursorLocked(lockCursor);
    }

    void Update()
    {
        // Esc: imleci bırak, sol tık: tekrar kilitle
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);
        else if (lockCursor && Cursor.lockState != CursorLockMode.Locked &&
                 Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);

        // V: first person <-> third person
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            firstPerson = !firstPerson;
            ApplyMode();
        }
    }

    void LateUpdate()
    {
        // Oyuncu MapGenerator tarafından oyun başında oluşturulduğu için çalışma anında bul.
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) return;
            target = p.transform;
            pivot = target.position + Vector3.up * lookHeight;   // ilk karede anında yerleş
            ApplyMode();
        }

        // Mouse ile kamerayı döndür (sadece imleç kilitliyken)
        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * mouseSensitivity;
            pitch -= delta.y * mouseSensitivity;
        }
        pitch = firstPerson ? Mathf.Clamp(pitch, fpMinPitch, fpMaxPitch) : Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);

        if (firstPerson)
        {
            // Gözden bak: kamera oyuncunun kafasında, baktığı yöne
            Vector3 flatForward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
            transform.position = target.position + Vector3.up * eyeHeight + flatForward * eyeForward;
            transform.rotation = rot;
            pivot = target.position + Vector3.up * lookHeight;
        }
        else
        {
            Vector3 wantedPivot = target.position + Vector3.up * lookHeight;
            pivot = Vector3.SmoothDamp(pivot, wantedPivot, ref pivotVelocity, followSmoothTime);
            transform.position = pivot + rot * (Vector3.back * distance);
            transform.rotation = rot;
        }
    }

    // Moda göre: yakın kesme mesafesi ve oyuncu gövdesinin görünürlüğü
    void ApplyMode()
    {
        if (cam != null) cam.nearClipPlane = firstPerson ? nearClip : defaultNearClip;

        if (target == null) return;
        if (bodyRenderers == null || bodyRenderers.Length == 0)
            bodyRenderers = target.GetComponentsInChildren<Renderer>(true);

        // First person'da gövde görünmez ama gölgesi kalır (kendi içini görmeyelim)
        foreach (Renderer r in bodyRenderers)
        {
            if (r == null) continue;
            r.shadowCastingMode = firstPerson
                ? UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly
                : UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

    void OnDisable()
    {
        SetCursorLocked(false);
    }

    static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
