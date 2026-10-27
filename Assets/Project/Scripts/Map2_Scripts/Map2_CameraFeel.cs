using UnityEngine;
using Unity.Cinemachine;

public class Map2_CameraFeel : MonoBehaviour
{
    [SerializeField] private CinemachineCamera vcam;              // FP_Camera
    [SerializeField] private CinemachineImpulseSource impulse;    // on the Player
    [SerializeField] private Map2_PlayerMove move;
    [SerializeField] private Map2_PlayerJump jump;
    [SerializeField] private Rigidbody rb;

    [Header("Sprint FOV (sprint FOV = base FOV turns it off)")]
    [SerializeField] private float baseFov = 65f;
    [SerializeField] private float sprintFov = 71f;
    [SerializeField] private float fovSpeed = 40f;       // degrees per second

    [Header("Strafe tilt (0 turns it off)")]
    [SerializeField] private float maxTilt = 1.5f;       // degrees
    [SerializeField] private float tiltSpeed = 10f;      // degrees per second

    [Header("Landing dip (0 turns it off)")]
    [SerializeField] private float landScale = 0.02f;    // metres of dip per m/s of falling
    [SerializeField] private float minLandSpeed = 2f;    // smaller falls give no dip
    [SerializeField] private float maxDip = 0.12f;       // metres

    private bool wasGrounded = true;
    private float lastFallSpeed = 0f;

    void Update()
    {
        // sprinting: the view widens a little, then narrows back
        float wantedFov = move.IsSprinting ? sprintFov : baseFov;
        vcam.Lens.FieldOfView = Mathf.MoveTowards(vcam.Lens.FieldOfView, wantedFov, fovSpeed * Time.deltaTime);

        // stepping sideways: the view leans a little that way (Dutch = roll)
        float wantedTilt = -move.MoveInput.x * maxTilt;
        vcam.Lens.Dutch = Mathf.MoveTowards(vcam.Lens.Dutch, wantedTilt, tiltSpeed * Time.deltaTime);
    }

    // landing is a physics event, so it is checked at the physics rate
    void FixedUpdate()
    {
        bool grounded = jump.IsGrounded();

        if (!grounded)
        {
            // remember how fast we were falling - on the landing step the collision has already stopped us
            lastFallSpeed = -rb.linearVelocity.y;
        }
        else if (!wasGrounded && lastFallSpeed > minLandSpeed)
        {
            // just landed hard enough: a short dip downwards, bigger for bigger falls
            float dip = Mathf.Min(lastFallSpeed * landScale, maxDip);
            impulse.GenerateImpulseWithVelocity(Vector3.down * dip);
        }

        wasGrounded = grounded;
    }
}
