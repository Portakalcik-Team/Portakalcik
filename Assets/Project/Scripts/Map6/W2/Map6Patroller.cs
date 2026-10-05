using UnityEngine;

public class Map6Patroller : MonoBehaviour
{
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2.0f; // Saniyede metre hız

    private Vector3 pointA;
    private bool goingToB = true;

    void Start()
    {
        pointA = transform.position; // A = Başladığı yer
    }

    void Update()
    {
        if (pointB == null) return;

        // B'ye gidiyorsa hedef B, değilse A'dır
        Vector3 target = goingToB ? pointB.position : pointA;

        // Hedefe doğru akıcı bir şekilde ilerle
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Hedefe tam ulaştığında yön değiştir
        if (transform.position == target)
        {
            goingToB = !goingToB;
        }
    }
}
