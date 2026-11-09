using UnityEngine;
using UnityEngine.InputSystem;

public class Map2_MouseLook : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;            // Player/CameraPivot - the Cinemachine camera follows it
    [SerializeField] private float mouseSensitivity = 0.15f;   // degrees per mouse pixel
    [SerializeField] private float stickSpeed = 180f;          // degrees per second, gamepad right stick
    [SerializeField] private float maxPitch = 80f;             // how far up and down you can look

    private Rigidbody rb;
    private InputAction lookAction;
    private float yaw;     // left / right
    private float pitch;   // up / down

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

        // the view turns every frame, so looking around is smooth (the camera brain runs after this, in LateUpdate)
        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // the body only turns left / right, at the physics rate - PlayerMove walks the way it faces
    void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));
    }
}
