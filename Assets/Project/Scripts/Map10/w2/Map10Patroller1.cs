using UnityEngine;
public class Map10Patroller : MonoBehaviour
{
[SerializeField] private Transform pointB;
[SerializeField] private float speed = 2.0f; // metres per second
private Vector3 pointA;
private bool goingToB = true;
void Start()
{
pointA = transform.position; // A = where we start
}
void Update()
{
Vector3 target = goingToB ? pointB.position : pointA; // going to B? then B, else A
transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
if (transform.position == target)
{
goingToB = !goingToB; // arrived: turn around
}
}
}