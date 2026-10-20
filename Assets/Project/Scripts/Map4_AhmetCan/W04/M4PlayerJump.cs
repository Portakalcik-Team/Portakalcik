using UnityEngine;
using UnityEngine.InputSystem;

// Space / gamepad Güney tuşu: sadece yerdeyken zıpla.
public class M4PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 4f;
    // hangi katmanlar zemin sayılır: Ground seç!
    [SerializeField] private LayerMask groundLayers;

    private Rigidbody rb;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpAction = InputSystem.actions.FindAction("Jump");
    }

    void Update()
    {
        // sadece tuşa basıldığı karede
        if (!jumpAction.WasPressedThisFrame()) return;
        if (!IsGrounded()) return;   // havada zıplama yok

        // yukarı doğru tek bir itiş
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // ayağımızın altında Ground var mı?
    bool IsGrounded()
    {
        // ortamızdan 1.1 m aşağı: boyumuzun yarısı + 0.1
        return Physics.Raycast(transform.position, Vector3.down,
                               1.1f, groundLayers);
    }
}