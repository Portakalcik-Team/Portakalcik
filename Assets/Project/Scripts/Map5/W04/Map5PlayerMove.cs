using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncuyu fizikle (Rigidbody) yürütür ve gittiği yöne döndürür.
public class Map5PlayerMove : MonoBehaviour
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

        // "Move" tuşları: W A S D ve gamepad'in sol çubuğu
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
        // Her karede tuşları oku: x = sol/sağ, y = geri/ileri
        Vector2 input = moveAction.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
    }

    void FixedUpdate()
    {
        // İstediğimiz hız ve şu anki hız (yukarı/aşağı hariç)
        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // İstenen hıza biraz yaklaş
        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        // Gittiğimiz yöne dön
        if (direction == Vector3.zero) return;
        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;
        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }

    // Buz bunu çağırır
    public void SetGrip(float value)
    {
        grip = value;
    }

    void OnDestroy()
    {
        moveAction.Disable();
    }
}
