using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncunun zemin kontrolüyle zıplamasını yönetir
public class Map6PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private LayerMask groundLayers; // Sadece Ground katmanı

    private Rigidbody rb;
    private InputAction jumpAction;

    void Awake()
    {
        // Inspector'dan maske boş gelse dahi Layer 3 (Ground) katmanını garanti et
        if (groundLayers.value == 0)
        {
            groundLayers = LayerMask.GetMask("Ground");
            if (groundLayers.value == 0)
            {
                groundLayers = 1 << 3;
            }
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Zıplama aksiyonunu tanımla
        jumpAction = new InputAction("Jump", InputActionType.Button);
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
        jumpAction.Enable();
    }

    void OnDestroy()
    {
        if (jumpAction != null)
        {
            jumpAction.Disable();
            jumpAction.Dispose();
        }
    }

    void Update()
    {
        bool jumpPressed = false;
        if (jumpAction != null && jumpAction.WasPressedThisFrame())
        {
            jumpPressed = true;
        }
        else if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpPressed = true;
        }

        if (!jumpPressed) return;
        if (!IsGrounded()) return;

        // Düşüş ivmesini sıfırlayıp net impuls ver
        Vector3 v = rb.linearVelocity;
        if (v.y < 0f) v.y = 0f;
        rb.linearVelocity = v;

        // Yukarı yönlü anlık kuvvet
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Oyuncunun altında Ground katmanı var mı kontrolü
    bool IsGrounded()
    {
        float rayLength = 1.25f;

        // Merkezden aşağı kontrol
        if (Physics.Raycast(transform.position, Vector3.down, rayLength, groundLayers))
            return true;

        // Eğim ve köşe emniyeti: Dört yönden hafif ofsetli kontrol
        Vector3 p = transform.position;
        if (Physics.Raycast(p + new Vector3(0.25f, 0f, 0f), Vector3.down, rayLength, groundLayers)) return true;
        if (Physics.Raycast(p + new Vector3(-0.25f, 0f, 0f), Vector3.down, rayLength, groundLayers)) return true;
        if (Physics.Raycast(p + new Vector3(0f, 0f, 0.25f), Vector3.down, rayLength, groundLayers)) return true;
        if (Physics.Raycast(p + new Vector3(0f, 0f, -0.25f), Vector3.down, rayLength, groundLayers)) return true;

        return false;
    }
}
