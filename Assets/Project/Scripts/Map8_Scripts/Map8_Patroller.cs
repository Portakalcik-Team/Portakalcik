using UnityEngine;

public class Map8_Patroller : MonoBehaviour
{
    [SerializeField] private float patrolDistance = 5.0f;
    [SerializeField] private float speed = 2.0f;

    private Vector3 pointA;
    private Vector3 pointBPos;
    private bool goingToB = true;

    void Start()
    {
        pointA = transform.position;
        
        pointBPos = pointA + transform.right * patrolDistance;
    }

    void Update()
    {
        Vector3 target = goingToB ? pointBPos : pointA;

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            goingToB = !goingToB;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 start = Application.isPlaying ? pointA : transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start, start + transform.right * patrolDistance);
    }
}
