using UnityEngine;
using UnityEngine.InputSystem;

public class Map0PlayerMove : MonoBehaviour
{
    // hareketin üç ayarı, Inspector'da görünür
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 720f;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;  // girişin gösterdiği yön
    private float grip = 1f;    // 1 = normal zemin

    void Start()
    {
        // bir kere bul: Rigidbody
        rb = GetComponent<Rigidbody>();

        // Tuşları kodla tanımla (InputSystem_Actions dosyası projede yok)
        moveAction = new InputAction("Move");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        moveAction.AddBinding("<Gamepad>/leftStick");
        moveAction.Enable();
    }

    void OnDestroy()
    {
        moveAction?.Disable();
    }

    void Update()
    {
        // her karede WASD, oklar ya da sol çubuğu oku
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
    }

    void FixedUpdate()
    {
        // istenen hız ve şu anki hız (yukarı-aşağı hariç)
        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // istenen hıza biraz yaklaş
        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        // gittiğimiz yöne dön
        if (direction == Vector3.zero) return;
        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;
        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }

    // buzlu rampa bunu çağırır
    public void SetGrip(float value)
    {
        grip = value;
    }
}
