using UnityEngine;                                                          // Unity'nin temel sınıfları
using UnityEngine.InputSystem;                                              // Yeni Input System için gerekli

// Oyuncuyu fizikle (Rigidbody) yürütür; yön, oyuncunun baktığı yöne göredir (birinci şahıs).
// Dönmeyi M4FirstPersonLook (fare) yapar.
public class M4PlayerMove : MonoBehaviour                                   // Oyuncuya eklenen hareket scripti
{
    // Hareketin iki ayarı, Inspector'da görünür
    [SerializeField] private float maxSpeed = 5f;        // en yüksek hız (m/s)
    [SerializeField] private float acceleration = 40f;   // hız ne kadar çabuk değişir

    private Rigidbody rb;                                                   // Oyuncunun fizik bedeni
    private InputAction moveAction;                                         // "Move" tuş eylemi (WASD / ok / çubuk)
    private Vector3 direction;   // tuşların gösterdiği yön                 // Her karede okunan hareket yönü
    private float grip = 1f;     // 1 = normal zemin, buzda küçülür         // Zemin tutuşu; küçükse hız zor değişir (kayma)

    void Start()                                                            // Oyun başlayınca bir kez çalışır
    {
        rb = GetComponent<Rigidbody>();                                     // Kendi Rigidbody bileşenini bul
        moveAction = InputSystem.actions.FindAction("Move");                // Input ayarlarından "Move" eylemini bul
    }

    void Update()                                                           // Her karede çalışır (girdi okuma)
    {
        // her kare: WASD, ok tuşları veya sol çubuğu oku
        Vector2 input = moveAction.ReadValue<Vector2>();                    // Tuşlardan 2 boyutlu yön değerini oku
        // W ileri, S geri, A/D yana: oyuncunun baktığı yöne göre
        direction = transform.right * input.x + transform.forward * input.y; // Gövde sadece Y'de döndüğü için yön yatay kalır
    }

    void FixedUpdate()                                                      // Sabit aralıkla çalışır (fizik hesapları)
    {
        // istediğimiz hız ve şu anki hız (yukarı/aşağı hariç)
        Vector3 wanted = direction * maxSpeed;                              // Gitmek istediğimiz hız vektörü
        Vector3 v = rb.linearVelocity;                                      // Şu anki tam hız
        Vector3 flat = new Vector3(v.x, 0f, v.z);                           // Sadece yatay hız (düşme/zıplama hızını karıştırma)

        // istenen hıza biraz daha yaklaş
        float step = acceleration * grip * Time.fixedDeltaTime;             // Bu fizik adımında hızın değişebileceği miktar
        flat = Vector3.MoveTowards(flat, wanted, step);                     // Yatay hızı istenen hıza bir adım yaklaştır
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);               // Yeni yatay hızı uygula, dikey hızı koru
    }

    // buzlu zemin bunu çağırır
    public void SetGrip(float value)                                        // Dışarıdan (buz zemin) tutuş değerini ayarlar
    {
        grip = value;                                                       // Yeni tutuş değerini sakla
    }
}
