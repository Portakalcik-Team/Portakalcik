using UnityEngine;                                                          // Unity'nin temel sınıfları
using UnityEngine.InputSystem;                                              // Yeni Input System (Look eylemi, klavye, fare)

// Birinci şahıs bakış: fare sağ-sol oyuncuyu döndürür, yukarı-aşağı kafayı eğer.
public class M4FirstPersonLook : MonoBehaviour                              // Oyuncuya eklenen bakış scripti
{
    // kameranın takip ettiği göz noktası ve fare ayarları
    [SerializeField] private Transform head;                                // Oyuncunun çocuğu "Head"; FPS kamerası buna kilitli
    [SerializeField] private float sensitivity = 0.1f;                      // Fare hassasiyeti: piksel başına derece
    [SerializeField] private float maxPitch = 80f;                          // Yukarı/aşağı en fazla kaç derece bakılır

    private Rigidbody rb;                                                   // Oyuncunun fizik bedeni
    private InputAction lookAction;                                         // "Look" eylemi (fare hareketi)
    private float yaw;     // sağ-sol açı                                   // Oyuncunun baktığı yön (Y ekseni)
    private float pitch;   // yukarı-aşağı açı                              // Kafanın eğimi (X ekseni)

    void Start()                                                            // Oyun başlayınca bir kez çalışır
    {
        rb = GetComponent<Rigidbody>();                                     // Kendi Rigidbody bileşenini bul
        lookAction = InputSystem.actions.FindAction("Look");                // Input ayarlarından "Look" eylemini bul
        yaw = transform.eulerAngles.y;                                      // Sahnedeki başlangıç yönünden başla
        LockCursor(true);                                                   // İmleci ekranın ortasına kilitle ve gizle

        // kendi gövdemiz kameranın önünü kapatmasın: sadece gölgesi kalsın
        foreach (Renderer r in GetComponentsInChildren<Renderer>())         // Oyuncu ve burnu (Nose)
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly; // Görünmez ama gölge düşürür
    }

    void Update()                                                           // Her karede çalışır
    {
        if (Time.timeScale == 0f) return;                                   // Oyun durduysa (Game Over) bakış yok

        // Esc imleci bırakır, tıklama tekrar kilitler
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) LockCursor(false); // Esc: imleç serbest
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) LockCursor(true);         // Sol tık: tekrar kilitle
        if (Cursor.lockState != CursorLockMode.Locked) return;              // İmleç serbestken fare kamerayı çevirmesin

        // fare ne kadar kaydı? (bu karedeki piksel farkı)
        Vector2 delta = lookAction.ReadValue<Vector2>();                    // x = sağ-sol, y = yukarı-aşağı
        yaw += delta.x * sensitivity;                                       // Sağa kaydır → sağa dön
        pitch -= delta.y * sensitivity;                                     // Yukarı kaydır → yukarı bak (X ekseni ters)
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);                    // Takla atmasın: sınırla

        // kafa hemen döner: kamera gecikmesiz takip eder
        head.rotation = Quaternion.Euler(pitch, yaw, 0f);                   // Göz noktasının dünya dönüşü
    }

    void FixedUpdate()                                                      // Sabit aralıkla çalışır (fizik)
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));                     // Gövde sadece sağ-sol döner; yürüme yönü buna göre
    }

    private void LockCursor(bool locked)                                    // İmleci kilitler ya da serbest bırakır
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None; // Kilitli = ortada sabit
        Cursor.visible = !locked;                                           // Kilitliyken gizli
    }
}
