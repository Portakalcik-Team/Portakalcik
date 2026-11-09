using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncunun birinci şahıs (FPS) fare bakışını ve gövde yönelimini yönetir
public class Map6MouseLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;   // Player'ın çocuğu olan Main Camera
    [SerializeField] private float mouseSensitivity = 0.15f; // Piksel başına açı
    [SerializeField] private float stickSpeed = 180f;        // Gamepad sağ çubuk hız derecesi
    [SerializeField] private float maxPitch = 80f;           // Yukarı/aşağı bakış limiti

    private Rigidbody rb;
    private InputAction lookAction;
    private float yaw;     // Sol/sağ: tüm gövdeyi çevirir
    private float pitch;   // Yukarı/aşağı: sadece kamerayı eğer

    void Awake()
    {
        // Kamera referansı boşsa otomatik olarak çocuk kamerayı bul
        if (cameraTransform == null)
        {
            Camera cam = GetComponentInChildren<Camera>();
            if (cam != null) cameraTransform = cam.transform;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // New Input System aksiyonunu kodla oluştur
        lookAction = new InputAction("Look", InputActionType.Value, expectedControlType: "Vector2");
        lookAction.AddBinding("<Mouse>/delta");
        lookAction.AddBinding("<Gamepad>/rightStick");
        lookAction.Enable();

        yaw = rb != null ? rb.rotation.eulerAngles.y : transform.eulerAngles.y;

        // Fare imlecini kilitle ve gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDestroy()
    {
        if (lookAction != null)
        {
            lookAction.Disable();
            lookAction.Dispose();
        }
    }

    void Update()
    {
        Vector2 raw = Vector2.zero;
        if (lookAction != null)
        {
            raw = lookAction.ReadValue<Vector2>();
        }

        // Doğrudan donanım yedeği
        if (raw == Vector2.zero && Mouse.current != null)
        {
            raw = Mouse.current.delta.ReadValue();
        }

        // Fare veya gamepad kontrolü
        bool isStick = lookAction != null && lookAction.activeControl != null && lookAction.activeControl.device is Gamepad;
        Vector2 look = isStick ? raw * stickSpeed * Time.deltaTime : raw * mouseSensitivity;

        yaw += look.x;
        pitch -= look.y;
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        // Dikey eksende yalnızca kamera eğilir, gövde dik kalır
        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        // Esc ile fareyi serbest bırak, tıklamayla tekrar kilitle
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Yatay dönüş: Gövde FixedUpdate içinde fiziksel olarak döner
    void FixedUpdate()
    {
        Quaternion targetRot = Quaternion.Euler(0f, yaw, 0f);
        if (rb != null)
        {
            rb.MoveRotation(targetRot);
        }
        else
        {
            transform.rotation = targetRot;
        }
    }
}
