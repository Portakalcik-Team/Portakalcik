using UnityEngine;

// PLAYER'a ekle (M1PlayerTrigger ile aynı objede olmalı).
// 1 / 2 / 3 tuşları: mandalina / portakal / limon seç. Sol tık: seçili meyveyi fırlat.
[RequireComponent(typeof(M1PlayerTrigger))]
public class M1PlayerThrow : MonoBehaviour
{
    public float throwSpeed = 18f;
    public float cooldown = 0.3f;
    public int damage = 3;            // Denge Sistemi: mandalina hasarı 3
    public float fruitSize = 0.3f;

    M1PlayerTrigger inv;
    Collider playerCol;
    float nextThrow;

    static readonly Color[] Colors =
    {
        new Color(1f, 0.65f, 0.1f),   // mandalina
        new Color(1f, 0.4f, 0f),      // portakal
        new Color(1f, 0.92f, 0.1f)    // limon
    };

    void Awake()
    {
        inv = GetComponent<M1PlayerTrigger>();
        playerCol = GetComponent<Collider>();
    }

    void Update()
    {
        if (M1PlayerTrigger.Stopped) return;                     // öldün ya da bölüm bitti
        if (Cursor.lockState != CursorLockMode.Locked) return;   // fare serbestken fırlatma

        int pick = PickedKey();
        if (pick >= 0) inv.selectedFruit = pick;

        if (FirePressed() && Time.time >= nextThrow) Throw();
    }

    void Throw()
    {
        nextThrow = Time.time + cooldown;
        int type = inv.selectedFruit;

        if (inv.GetFruit(type) <= 0)
        {
            inv.Show(M1PlayerTrigger.FruitName(type) + " kalmadı!");
            return;
        }

        var cam = Camera.main;
        if (cam == null) return;

        inv.AddFruit(type, -1);

        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // Adı "Firlatilan_..." olsun: ağaç gibi algılanmasın
        go.name = "Firlatilan_" + M1PlayerTrigger.FruitName(type);
        go.transform.position = cam.transform.position + cam.transform.forward * 0.8f;
        go.transform.localScale = Vector3.one * fruitSize;
        go.GetComponent<Renderer>().material.color = Colors[type];

        var rb = go.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = cam.transform.forward * throwSpeed;   // Unity 6 (eski sürümlerde rb.velocity)

        if (playerCol != null) Physics.IgnoreCollision(go.GetComponent<Collider>(), playerCol);

        var p = go.AddComponent<M1FruitProjectile>();
        p.damage = damage;
        p.fruitName = M1PlayerTrigger.FruitName(type);
        p.owner = inv;

        Destroy(go, 5f);   // bir şeye çarpmazsa 5 sn sonra kaybolur
    }

    int PickedKey()
    {
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        if (k == null) return -1;
        if (k.digit1Key.wasPressedThisFrame) return 0;
        if (k.digit2Key.wasPressedThisFrame) return 1;
        if (k.digit3Key.wasPressedThisFrame) return 2;
        return -1;
#else
        if (Input.GetKeyDown(KeyCode.Alpha1)) return 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) return 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) return 2;
        return -1;
#endif
    }

    bool FirePressed()
    {
#if ENABLE_INPUT_SYSTEM
        var m = UnityEngine.InputSystem.Mouse.current;
        return m != null && m.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
}
