using UnityEngine;
using UnityEngine.InputSystem;

// Seviye 9: oyuncu WASD / sol çubukla yürür ve döner
public class Map9PlayerMove : MonoBehaviour
{
    // hareketin üç düğmesi, Inspector'da
    [SerializeField] private float maxSpeed = 4f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 720f;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;   // girdinin gösterdiği yön
    private float grip = 1f;     // 1 = normal zemin

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // her kare: WASD, oklar ya da sol çubuk
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
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

        // gittiği yöne dön
        if (direction == Vector3.zero) return;
        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;
        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }

    // buz bunu çağırır
    public void SetGrip(float value)
    {
        grip = value;
    }
}