using UnityEngine;

// Player'a ekle (Rigidbody + Collider gerekli). Sadece zıplamayı yapar: Space.
// Yerdeyken zıplar. Zıplama yüksekliği duvar yüksekliğinden (3) az olmalı, yoksa labirent atlanır.
[RequireComponent(typeof(Rigidbody))]
public class PlayerJump : MonoBehaviour
{
    public float jumpHeight = 1f;      // zıplama yüksekliği (birim)
    public float groundCheckExtra = 0.15f;

    Rigidbody rb;
    Collider col;
    bool jumpPressed;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        rb.useGravity = true;          // zıplayınca geri inebilsin
    }

    void Update()
    {
        if (JumpKeyDown()) jumpPressed = true;
    }

    void FixedUpdate()
    {
        if (jumpPressed && IsGrounded())
        {
            // v = sqrt(2 * g * h)
            float v = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);
            Vector3 vel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(vel.x, v, vel.z);
        }
        jumpPressed = false;
    }

    bool IsGrounded()
    {
        Bounds b = col != null ? col.bounds : new Bounds(transform.position, Vector3.one);
        // Collider'ın tabanından biraz aşağıya ışın at; trigger'ları (ağaç, exit) zemin sayma
        return Physics.Raycast(b.center, Vector3.down, b.extents.y + groundCheckExtra,
                               ~0, QueryTriggerInteraction.Ignore);
    }

    bool JumpKeyDown()
    {
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        return k != null && k.spaceKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Space);
#endif
    }
}
