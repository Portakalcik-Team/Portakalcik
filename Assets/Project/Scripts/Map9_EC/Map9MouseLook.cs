using UnityEngine;
using UnityEngine.InputSystem;

// Seviye 9: fare / sağ çubukla bakış — sağa-sola gövde, yukarı-aşağı kamera
public class Map9MouseLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;        // Main Camera (Player'ın çocuğu)
    [SerializeField] private float mouseSensitivity = 0.15f;   // fare: piksel başına derece
    [SerializeField] private float stickSpeed = 180f;          // gamepad sağ çubuk: saniyede derece
    [SerializeField] private float maxPitch = 80f;             // en fazla yukarı/aşağı bakış

    private Rigidbody rb;
    private InputAction lookAction;
    private float yaw;     // sağa-sola: bütün gövde döner
    private float pitch;   // yukarı-aşağı: sadece kamera eğilir

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lookAction = InputSystem.actions.FindAction("Look");
        yaw = rb.rotation.eulerAngles.y;             // oyuncunun baktığı yönden başla
        Cursor.lockState = CursorLockMode.Locked;    // fare gizlenir, oyunda kalır (Esc serbest bırakır)
    }

    void Update()
    {
        Vector2 raw = lookAction.ReadValue<Vector2>();

        // fare bu karede kaç piksel kaydı; çubuk -1..+1 verir, saniyede dereceye çevrilir
        bool stick = lookAction.activeControl != null && lookAction.activeControl.device is Gamepad;
        Vector2 look = stick ? raw * stickSpeed * Time.deltaTime : raw * mouseSensitivity;

        yaw += look.x;
        pitch -= look.y;                                  // fare yukarı = yukarı bak
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        // yukarı-aşağı: sadece kamera eğilir, gövde dik kalır
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // sağa-sola: gövde döner — Rigidbody FixedUpdate'te döndürülür (Hafta 4)
    void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
    }
}