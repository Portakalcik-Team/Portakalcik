using UnityEngine;

// On CameraPivot/ViewModel: the hand with the lantern lags a little when you turn
// and swings a little when you walk. The camera itself never moves.
public class Map2_HandSway : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Map2_PlayerMove move;
    [SerializeField] private Map2_PlayerJump jump;
    [SerializeField] private Rigidbody rb;

    [Header("Turning: the hand lags behind (0 turns it off)")]
    [SerializeField] private float swayAmount = 0.01f;    // degrees of lag per degree-per-second of turning
    [SerializeField] private float maxSway = 2f;          // degrees
    [SerializeField] private float swaySpeed = 20f;       // degrees per second it catches up

    [Header("Walking: the hand swings (0 turns it off)")]
    [SerializeField] private float bobAmount = 0.012f;    // metres
    [SerializeField] private float bobSpeed = 1.6f;       // swings per metre walked... roughly
    [SerializeField] private float returnSpeed = 0.05f;   // metres per second back to rest

    private Vector3 restPosition;
    private Quaternion restRotation;
    private Vector2 lastLook;       // pivot pitch and yaw last frame
    private Vector2 sway;           // current lag, degrees
    private Vector3 bob;            // current swing, metres
    private float bobTimer = 0f;    // our own clock for the swing

    void Start()
    {
        restPosition = transform.localPosition;
        restRotation = transform.localRotation;
        lastLook = Look();
    }

    // after Map2_MouseLook has turned the pivot this frame
    void LateUpdate()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        // turning: how fast did the view turn this frame, in degrees per second?
        Vector2 look = Look();
        Vector2 turnSpeed = new Vector2(Mathf.DeltaAngle(lastLook.x, look.x), Mathf.DeltaAngle(lastLook.y, look.y)) / dt;
        lastLook = look;
        Vector2 wantedSway = Vector2.ClampMagnitude(-turnSpeed * swayAmount, maxSway);
        sway = Vector2.MoveTowards(sway, wantedSway, swaySpeed * dt);

        // walking: a small figure-of-eight while moving on the ground
        Vector3 v = rb.linearVelocity;
        float speed = new Vector3(v.x, 0f, v.z).magnitude;
        if (speed > 0.5f && jump.IsGrounded() && move.MoveInput != Vector2.zero)
        {
            bobTimer += dt * speed * bobSpeed;
            bob = new Vector3(Mathf.Sin(bobTimer) * bobAmount, Mathf.Sin(bobTimer * 2f) * bobAmount * 0.5f, 0f);
        }
        else
        {
            bob = Vector3.MoveTowards(bob, Vector3.zero, returnSpeed * dt);
            if (bob == Vector3.zero) bobTimer = 0f;   // next walk starts the swing from the middle, no jump
        }

        transform.localPosition = restPosition + bob;
        transform.localRotation = restRotation * Quaternion.Euler(sway.x, sway.y, 0f);
    }

    private Vector2 Look()
    {
        Vector3 e = cameraPivot.eulerAngles;
        return new Vector2(e.x, e.y);
    }
}
