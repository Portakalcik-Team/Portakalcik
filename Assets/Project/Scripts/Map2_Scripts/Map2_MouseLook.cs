using UnityEngine;
using UnityEngine.InputSystem;

public class Map2_MouseLook : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;   // the Main Camera, a child of the Player
    [SerializeField] private float mouseSensitivity = 0.15f;   // degrees per mouse pixel
    [SerializeField] private float stickSpeed = 180f;          // degrees per second, gamepad right stick
    [SerializeField] private float maxPitch = 80f;             // how far up and down you can look

    private Rigidbody rb;
    private InputAction lookAction;
    private float yaw;     // left / right: turns the whole body
    private float pitch;   // up / down: tilts only the camera

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lookAction = InputSystem.actions.FindAction("Look");
        yaw = rb.rotation.eulerAngles.y;             // start facing the way the player already faces
        Cursor.lockState = CursorLockMode.Locked;    // hide the mouse, keep it in the game (Esc frees it)
    }

    void Update()
    {
        Vector2 raw = lookAction.ReadValue<Vector2>();

        // a mouse gives pixels moved this frame; a stick gives -1..+1 that must be turned into degrees per second
        bool stick = lookAction.activeControl != null && lookAction.activeControl.device is Gamepad;
        Vector2 look = stick ? raw * stickSpeed * Time.deltaTime : raw * mouseSensitivity;

        yaw += look.x;
        pitch -= look.y;                                  // mouse up = look up
        pitch = Mathf.Clamp(pitch, -maxPitch, maxPitch);

        // up / down: only the camera tilts, the body stays upright
        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    // left / right: the body turns - a Rigidbody is turned in FixedUpdate (Week 4)
    void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
    }
}
