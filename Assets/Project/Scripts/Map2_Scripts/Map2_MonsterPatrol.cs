using UnityEngine;

public class Map2_MonsterPatrol : MonoBehaviour
{
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;      // metres per second
    [SerializeField] private float waitTime = 1f;   // seconds at each end = turning time

    private Rigidbody rb;            // kinematic: our code moves it, the player bumps into it
    private Vector3 pointA;
    private bool goingToB = true;
    private float waitTimer = 0f;
    private float turnedSoFar = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pointA = rb.position;                  // A = where the monster starts
        transform.LookAt(pointB.position);     // face B before walking
    }

    // a Rigidbody is moved in FixedUpdate (Week 4) - never with transform
    void FixedUpdate()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.fixedDeltaTime;
            float step = 180f / waitTime * Time.fixedDeltaTime;   // degrees this physics step
            if (turnedSoFar + step > 180f)
            {
                step = 180f - turnedSoFar;   // never turn more than 180
            }
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, step, 0f));
            turnedSoFar += step;
            return;                          // no walking while turning
        }

        Vector3 target = goingToB ? pointB.position : pointA;
        Vector3 next = Vector3.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
        rb.MovePosition(next);

        if (next == target)
        {
            goingToB = !goingToB;   // next target
            waitTimer = waitTime;   // start waiting...
            turnedSoFar = 0f;       // ...and turning from 0 degrees
        }
    }
}
