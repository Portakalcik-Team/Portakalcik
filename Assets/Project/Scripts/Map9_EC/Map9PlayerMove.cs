using UnityEngine;
using UnityEngine.InputSystem;

// Seviye 9: oyuncu WASD / sol çubukla yürür — baktığı yöne göre (birinci şahıs)
public class Map9PlayerMove : MonoBehaviour
{
    // hareketin iki düğmesi (dönüşü artık Map9MouseLook yapıyor)
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float acceleration = 40f;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;   // gitmek istenen yön
    private float grip = 1f;     // 1 = normal zemin

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // W/S = baktığın yön, A/D = yana kay
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = transform.right * input.x + transform.forward * input.y;
    }

    void FixedUpdate()
    {
        // istenen hız ve şu anki hız (yukarı/aşağı hariç)
        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // istenen hıza biraz daha yaklaş
        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
    }

    // buz bunu çağırır
    public void SetGrip(float value)
    {
        grip = value;
    }
}