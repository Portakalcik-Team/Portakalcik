using UnityEngine;
using UnityEngine.InputSystem;

public class Map2_PlayerMove : MonoBehaviour
{
    // the knobs of movement, shown in the Inspector (turning is Map2_MouseLook's job)
    [SerializeField] private float maxSpeed = 5f;        // walking: the balance sheet's "1.0"
    [SerializeField] private float sprintSpeed = 7f;     // Shift held while moving forward
    [SerializeField] private float acceleration = 40f;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction sprintAction;
    private Vector2 input;       // W/S = y, A/D = x
    private float grip = 1f;     // 1 = normal ground

    // read by Map2_CameraFeel: FOV kick and strafe tilt
    public bool IsSprinting { get; private set; }
    public Vector2 MoveInput => input;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
        sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    void Update()
    {
        // every frame: read WASD, arrows or the left stick
        input = moveAction.ReadValue<Vector2>();

        // sprint only while running forward - not backwards or sideways
        IsSprinting = sprintAction.IsPressed() && input.y > 0f;
    }

    void FixedUpdate()
    {
        // first person: W = where I look, D = to my right
        Vector3 forward = rb.rotation * Vector3.forward;
        Vector3 right = rb.rotation * Vector3.right;
        Vector3 direction = forward * input.y + right * input.x;

        // speed we want, speed we have (not up/down)
        float speed = IsSprinting ? sprintSpeed : maxSpeed;
        Vector3 wanted = direction * speed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // get a little closer to the wanted speed
        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
    }

    // the ice patch calls this
    public void SetGrip(float value)
    {
        grip = value;
    }
}
