using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncunun ivmeli, fizik tabanlı hareketini ve yönelimini kontrol eder
public class Map6PlayerMove : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private bool firstPersonMode = true; // Birinci şahıs bağıl hareket

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector2 moveInput; // Ham girdi (x = sol/sağ, y = ileri/geri)
    private Vector3 direction; // Oyuncunun gitmek istediği yön
    private float grip = 1f;   // 1 = normal zemin, 0.04 = buz

    void Awake()
    {
        // Değerlerin Inspector sıfırlanmasına karşı güvenlik kontrolü
        if (maxSpeed <= 0f) maxSpeed = 5f;
        if (acceleration <= 0f) acceleration = 40f;
        if (turnSpeed <= 0f) turnSpeed = 720f;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Yeni Input System aksiyonunu kodla oluştur ve bağla
        moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
        
        // WASD bağlaması
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Ok tuşları bağlaması
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        // Gamepad sol çubuk
        moveAction.AddBinding("<Gamepad>/leftStick");

        moveAction.Enable();
    }

    void OnDestroy()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
            moveAction.Dispose();
        }
    }

    void Update()
    {
        Vector2 input = Vector2.zero;

        // InputAction üzerinden girdi oku
        if (moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>();
        }

        // Doğrudan klavye yedeği (odaklanma ve alternatif kontrol garantisi)
        if (input == Vector2.zero)
        {
            Keyboard kb = Keyboard.current;
            if (kb != null)
            {
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) input.y += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) input.y -= 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) input.x -= 1f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) input.x += 1f;
                if (input.sqrMagnitude > 1f) input.Normalize();
            }
        }

        moveInput = input;
        direction = new Vector3(input.x, 0f, input.y);
    }

    void FixedUpdate()
    {
        // Yatay hız kontrolü (First Person modunda baktığı yöne göre, üçüncü şahısta dünya yönünde)
        Vector3 wanted;
        if (firstPersonMode)
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDir = forward * moveInput.y + right * moveInput.x;
            if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();
            wanted = moveDir * maxSpeed;
        }
        else
        {
            wanted = direction * maxSpeed;
        }

        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        // İvmelenme ve zemin tutuşu
        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);
        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        // Hareket yönüne doğru dönüş (yalnızca üçüncü şahıs modunda çalışır; FPS modunda rotasyonu Map6MouseLook yönetir)
        if (!firstPersonMode)
        {
            if (direction == Vector3.zero) return;
            Quaternion look = Quaternion.LookRotation(direction);
            float turn = turnSpeed * Time.fixedDeltaTime;
            look = Quaternion.RotateTowards(rb.rotation, look, turn);
            rb.MoveRotation(look);
        }
    }

    // Buzlu zemin veya rampa tutuşu değiştirmek için çağırır
    public void SetGrip(float value)
    {
        grip = value;
    }
}
