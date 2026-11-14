using UnityEngine;                                                          // Unity'nin temel sınıfları
using UnityEngine.InputSystem;                                              // Yeni Input System için gerekli

// Oyuncuyu fizikle (Rigidbody) yürütür ve gittiği yöne döndürür.
public class M4PlayerMove : MonoBehaviour                                   // Oyuncuya eklenen hareket scripti
{
    // Hareketin üç ayarı, Inspector'da görünür
    [SerializeField] private float maxSpeed = 5f;        // en yüksek hız (m/s)
    [SerializeField] private float acceleration = 40f;   // hız ne kadar çabuk değişir
    [SerializeField] private float turnSpeed = 720f;     // saniyede kaç derece döner

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
        direction = new Vector3(input.x, 0f, input.y);                      // 2D girdiyi yatay düzlemdeki 3D yöne çevir (Y = 0)
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

        // gidilen yöne dön
        if (direction == Vector3.zero) return;                              // Tuşa basılmıyorsa dönme
        Quaternion look = Quaternion.LookRotation(direction);               // Gidilen yöne bakan hedef dönüş
        float turn = turnSpeed * Time.fixedDeltaTime;                       // Bu adımda dönebileceği en fazla açı
        look = Quaternion.RotateTowards(rb.rotation, look, turn);           // Mevcut dönüşten hedefe doğru bir adım dön
        rb.MoveRotation(look);                                              // Dönüşü fizik üzerinden uygula
    }

    // buzlu zemin bunu çağırır
    public void SetGrip(float value)                                        // Dışarıdan (buz zemin) tutuş değerini ayarlar
    {
        grip = value;                                                       // Yeni tutuş değerini sakla
    }
}
