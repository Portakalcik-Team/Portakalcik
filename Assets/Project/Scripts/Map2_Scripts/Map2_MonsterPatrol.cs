using UnityEngine;
public class Map2_MonsterPatrol : MonoBehaviour
{
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;      // metres per second
    [SerializeField] private float waitTime = 1f;   // seconds at each end = turning time
    private Vector3 pointA;
    private bool goingToB = true;
    private float waitTimer = 0f;
    private float turnedSoFar = 0f;
    void Start()
    {
        pointA = transform.position;     // A = where the monster starts
        transform.LookAt(pointB.position);   // face B before walking
    }
    void Update()
    {
        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            float step = 180f / waitTime * Time.deltaTime;   // degrees this frame
            if (turnedSoFar + step > 180f)
            {
                step = 180f - turnedSoFar;   // never turn more than 180
            }
            transform.Rotate(0f, step, 0f);
            turnedSoFar += step;
            return;                          // no walking while turning
        }
        Vector3 target = goingToB ? pointB.position : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target)
        {
            goingToB = !goingToB;   // next target
            waitTimer = waitTime;   // start waiting...
            turnedSoFar = 0f;       // ...and turning from 0 degrees
        }
    }
}
