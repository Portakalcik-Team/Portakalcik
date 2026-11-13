using UnityEngine;

// PLAYER'a ekle (Rigidbody + Collider gerekli). WASD, baktığın yöne göre yürür.
// Yükseklik (Y) hızına dokunmaz, zıplama bozulmasın.
[RequireComponent(typeof(Rigidbody))]
public class M1PlayerMove : MonoBehaviour
{
    public float speed = 5f;

    // Büyük meyve alınca M1FruitPickup bunu ayarlar: bir süre yavaşlat
    public static float SlowUntil;
    public static float SlowFactor = 1f;

    Rigidbody rb;
    Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;   // devrilmesin
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Update()
    {
        input = ReadInput();
    }

    void FixedUpdate()
    {
        // Kameranın baktığı yönü yatay düzleme indir
        Quaternion yaw = Quaternion.Euler(0f, M1FirstPersonCamera.Yaw, 0f);
        Vector3 dir = yaw * new Vector3(input.x, 0f, input.y);
        if (M1PlayerTrigger.Stopped) dir = Vector3.zero;       // öldün ya da bölüm bitti

        // Başka bir script ya da harita üreticisi Rigidbody'yi kinematic yaptıysa geri düzelt
        if (rb.isKinematic) rb.isKinematic = false;

        Vector3 v = rb.linearVelocity;                        // Unity 6 (eski sürümlerde rb.velocity)
        float s = Time.time < SlowUntil ? speed * SlowFactor : speed;
        rb.linearVelocity = new Vector3(dir.x * s, v.y, dir.z * s);
    }

    Vector2 ReadInput()
    {
        float x = 0, z = 0;
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        if (k == null) return Vector2.zero;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed)  x -= 1;
        if (k.wKey.isPressed || k.upArrowKey.isPressed)    z += 1;
        if (k.sKey.isPressed || k.downArrowKey.isPressed)  z -= 1;
#else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
#endif
        return new Vector2(x, z).normalized;
    }
}
