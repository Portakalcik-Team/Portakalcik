using UnityEngine;
using UnityEngine.InputSystem;

// Seviye 9: Space ile zıplar — sadece yerdeyken
public class Map9PlayerJump : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    // hangi katmanlar zemin sayılır: ground seç!
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

        // yukarı tek bir tekme
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // ayağımızın altında ground var mı?
    bool IsGrounded()
    {
        // pivot ayakta: 10 cm yukarıdan 20 cm aşağı bak
        Vector3 from = transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(from, Vector3.down, 0.2f, groundLayers);
    }
}