using UnityEngine;
using UnityEngine.InputSystem;

// Map3 W04: Space / Guney tusu ile sadece yerdeyken ziplar.
public class M3PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 4f;
    // hangi katmanlar zemin: Inspector'da Ground sec
    [SerializeField] private LayerMask groundLayers;
    // isin boyu: oyuncu boyu 1.8, yarisi 0.9 + 0.1
    [SerializeField] private float rayLength = 1.0f;

    private Rigidbody rb;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        jumpAction = new InputAction("Jump");
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
        jumpAction.Enable();
    }

    void Update()
    {
        // sadece tusa basildigi karede
        if (!jumpAction.WasPressedThisFrame()) return;
        if (!IsGrounded()) return;   // havada ziplama yok

        // yukari tek bir itme
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // ayagimizin altinda Ground var mi?
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, rayLength, groundLayers);
    }

    void OnDestroy()
    {
        if (jumpAction != null) jumpAction.Disable();
    }
}
