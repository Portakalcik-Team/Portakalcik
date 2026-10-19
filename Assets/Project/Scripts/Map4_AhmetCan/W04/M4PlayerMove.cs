using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncuyu fizikle (Rigidbody) yürütür ve gittiği yöne döndürür.
public class M4PlayerMove : MonoBehaviour
{
    // Hareketin üç ayarı, Inspector'da görünür
    [SerializeField] private float maxSpeed = 5f;        // en yüksek hız (m/s)
    [SerializeField] private float acceleration = 40f;   // hız ne kadar çabuk değişir
    [SerializeField] private float turnSpeed = 720f;     // saniyede kaç derece döner

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;   // tuşların gösterdiği yön
    private float grip = 1f;     // 1 = normal zemin, buzda küçülür

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");
    }

    void Update()
    {
        // her kare: WASD, ok tuşları veya sol çubuğu oku
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
    }

    void FixedUpdate()
    {
        // istediğimiz hız ve şu anki hız (yukarı/aşağı hariç)
        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // istenen hıza biraz daha yaklaş
        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        // gidilen yöne dön
        if (direction == Vector3.zero) return;
        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;
        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }

    // buzlu zemin bunu çağırır
    public void SetGrip(float value)
    {
        grip = value;
    }
}