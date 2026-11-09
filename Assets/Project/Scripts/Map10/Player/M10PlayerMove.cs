using UnityEngine;
using UnityEngine.InputSystem;

public class M10PlayerMove : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float acceleration = 40f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;
    private float grip = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveAction = InputSystem.actions.FindAction("Move");

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Kamerayı Player'ın child'ından çıkar (eğer hala child ise)
        if (cameraTransform != null && cameraTransform.parent == transform)
        {
            cameraTransform.SetParent(null);
        }

        // Kameraya Map10CameraFollow ekle (yoksa)
        if (cameraTransform != null && cameraTransform.GetComponent<Map10CameraFollow>() == null)
        {
            cameraTransform.gameObject.AddComponent<Map10CameraFollow>();
        }
    }

    void Update()
    {
        if (moveAction == null || cameraTransform == null)
            return;

        Vector2 input = moveAction.ReadValue<Vector2>();

        // Kameranın yatay yönlerini al
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // WASD girişini kamera yönüne göre hesapla
        direction = forward * input.y + right * input.x;

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        Vector3 wanted = direction * maxSpeed;

        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);

        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);

        // Hareket yönüne dön
        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion look = Quaternion.LookRotation(direction);
        float turn = turnSpeed * Time.fixedDeltaTime;

        look = Quaternion.RotateTowards(rb.rotation, look, turn);
        rb.MoveRotation(look);
    }

    public void SetGrip(float value)
    {
        grip = value;
    }
}