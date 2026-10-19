using UnityEngine;
using UnityEngine.InputSystem;

public class Map2_PlayerMove : MonoBehaviour
{
    // the knobs of movement, shown in the Inspector (turning is Map2_MouseLook's job)
    [SerializeField] private float maxSpeed = 5f;        // the balance sheet's "1.0"
    [SerializeField] private float acceleration = 40f;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector2 input;       // W/S = y, A/D = x
    private float grip = 1f;     // 1 = normal ground

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // every frame: read WASD, arrows or the left stick
        input = moveAction.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // first person: W = where I look, D = to my right
        Vector3 forward = rb.rotation * Vector3.forward;
        Vector3 right = rb.rotation * Vector3.right;
        Vector3 direction = forward * input.y + right * input.x;

        // speed we want, speed we have (not up/down)
        Vector3 wanted = direction * maxSpeed;
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
