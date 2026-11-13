using UnityEngine;
using UnityEngine.InputSystem;

public class M10PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5.8f; // Canavarın üstünden aşmak için yeterli güç
    [SerializeField] private LayerMask groundLayers;

    private Rigidbody rb;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (InputSystem.actions != null)
        {
            jumpAction = InputSystem.actions.FindAction("Jump");
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
        else
        {
            try
            {
                if (Input.GetKeyDown(KeyCode.Space)) jumpPressed = true;
            }
            catch { }
        }

        if (!jumpPressed) return;

        if (!IsGrounded()) return;

        // Yukarı zıplat
        if (rb != null)
        {
            Vector3 v = rb.linearVelocity;
            v.y = jumpForce;
            rb.linearVelocity = v;
        }
    }

    private bool IsGrounded()
    {
        // 1. Raycast kontrolü
        if (Physics.Raycast(transform.position, Vector3.down, 1.25f))
        {
            return true;
        }

        // 2. Altımızda canavar veya zemin var mı (SphereCast ile geniş taban kontrolü)
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.35f, Vector3.down, out hit, 1.1f))
        {
            if (hit.collider != null && hit.collider.gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }
}
