using UnityEngine;

public class Map0Patroller : MonoBehaviour
{
    [SerializeField] private Transform pointB;    // ayrı bir boş nesne, çocuk değil
    [SerializeField] private float speed = 2.0f;  // saniyede metre

    private Vector3 pointA;
    private bool goingToB = true;

    void Start()
    {
        // A = başladığımız yer
        pointA = transform.position;
    }

    void Update()
    {
        // B'ye mi gidiyoruz? O zaman hedef B, değilse A
        Vector3 target = goingToB ? pointB.position : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (transform.position == target)
        {
            goingToB = !goingToB; // vardık: geri dön
        }
    }
}
