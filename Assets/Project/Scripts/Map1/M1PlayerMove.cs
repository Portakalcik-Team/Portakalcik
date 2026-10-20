using UnityEngine;

// Player'a ekle (Rigidbody + Collider gerekli). Sadece yürümeyi yapar: WASD / ok tuşları.
// Yükseklik (Y) hızına dokunmaz, zıplama ve yer çekimi bozulmasın diye.
[RequireComponent(typeof(Rigidbody))]
public class M1PlayerMove : MonoBehaviour
{
    public float speed = 5f;

    Rigidbody rb;
    Vector3 dir;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;   // devrilmesin
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    void Update()
    {
        dir = ReadInput();
    }

    void FixedUpdate()
    {
        Vector3 v = rb.linearVelocity;                          // Unity 6 (eski sürümlerde rb.velocity)
        rb.linearVelocity = new Vector3(dir.x * speed, v.y, dir.z * speed);
    }

    Vector3 ReadInput()
    {
        float x = 0, z = 0;
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        if (k == null) return Vector3.zero;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed)  x -= 1;
        if (k.wKey.isPressed || k.upArrowKey.isPressed)    z += 1;
        if (k.sKey.isPressed || k.downArrowKey.isPressed)  z -= 1;
#else
        x = Input.GetAxisRaw("Horizontal");
        z = Input.GetAxisRaw("Vertical");
#endif
        return new Vector3(x, 0, z).normalized;
    }
}
