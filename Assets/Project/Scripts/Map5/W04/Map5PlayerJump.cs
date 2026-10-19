using UnityEngine;
using UnityEngine.InputSystem;

// Space (veya gamepad'in alt tuşu) ile zıplatır, ama sadece yerdeyken.
public class Map5PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 4f;   // yukarı tekme gücü
    // Hangi katmanlar zemin sayılır: Ground seçili olmalı!
    [SerializeField] private LayerMask groundLayers;

    private Rigidbody rb;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // "Jump" tuşu: Space ve gamepad'in alt tuşu
        jumpAction = new InputAction("Jump", binding: "<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
        jumpAction.Enable();
    }

    void Update()
    {
        // Sadece tuşa basıldığı karede
        if (!jumpAction.WasPressedThisFrame()) return;
        if (!IsGrounded()) return;   // havada zıplamak yok

        // Yukarı doğru tek bir tekme
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Ayağımızın altında zemin var mı?
    bool IsGrounded()
    {
        // Ortamızdan 1 m aşağı: boyumuzun yarısı (0.9) + 0.1
        return Physics.Raycast(transform.position, Vector3.down,
                               1f, groundLayers);
    }

    void OnDestroy()
    {
        jumpAction.Disable();
    }
}
