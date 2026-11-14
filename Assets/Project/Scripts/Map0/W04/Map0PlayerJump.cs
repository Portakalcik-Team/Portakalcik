using UnityEngine;
using UnityEngine.InputSystem;

public class Map0PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 4f;
    // hangi katmanlar zemin sayılır: Ground seç!
    [SerializeField] private LayerMask groundLayers;

    private Rigidbody rb;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Tuşları kodla tanımla (InputSystem_Actions dosyası projede yok)
        jumpAction = new InputAction("Jump");
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");
        jumpAction.Enable();
    }

    void OnDestroy()
    {
        jumpAction?.Disable();
    }

    void Update()
    {
        // sadece Space / alt tuşa basıldığı karede
        if (!jumpAction.WasPressedThisFrame()) return;
        if (!IsGrounded()) return; // havada zıplama yok

        // yukarı tek bir itme
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // ayağımızın altında Ground var mı?
    bool IsGrounded()
    {
        // ortamızdan 1.1 m aşağı: boyumuzun yarısı + 0.1
        return Physics.Raycast(transform.position, Vector3.down, 1.1f, groundLayers);
    }
}
