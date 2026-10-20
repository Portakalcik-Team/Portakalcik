using UnityEngine;
using UnityEngine.InputSystem;

// Map3 W04: oyuncuyu WASD / sol cubuk ile fizikle yurutur ve dondurur.
public class M3PlayerMove : MonoBehaviour
{
    // hareketin uc ayari (Inspector'da)
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 720f;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;   // tuslarin gosterdigi yon
    private float grip = 1f;     // 1 = normal zemin

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // InputSystem_Actions dosyasi yok: tuslari kodla tanimla
        moveAction = new InputAction("Move");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddBinding("<Gamepad>/leftStick");
        moveAction.Enable();
    }

    void Update()
    {
        // her kare tuslari oku
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
    }

    void FixedUpdate()
    {
        // istenen hiz ve simdiki hiz (yukari/asagi haric)
        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // istenen hiza biraz yaklas
        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        // gidilen yone don
        if (direction == Vector3.zero) return;
        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;
        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }

    // buzlu zemin cagirir
    public void SetGrip(float value)
    {
        grip = value;
    }

    void OnDestroy()
    {
        if (moveAction != null) moveAction.Disable();
    }
}
