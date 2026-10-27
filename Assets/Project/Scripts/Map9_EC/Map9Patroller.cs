using UnityEngine;

// Seviye 9: canavar A ile B arasında gidip gelir
public class Map9Patroller : MonoBehaviour
{
    [SerializeField] private Transform pointB;   // varış noktası: AYRI bir boş obje
    [SerializeField] private float speed = 2f;   // metre / saniye

    private Vector3 pointA;
    private bool goingToB = true;

    void Start()
    {
        pointA = transform.position;              // A = başladığı yer
    }

    void Update()
    {
        // B'ye mi gidiyor? Evet: hedef B, hayır: hedef A
        Vector3 target = goingToB ? pointB.position : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target)
        {
            goingToB = !goingToB;                 // vardı: geri dön
        }
    }
}