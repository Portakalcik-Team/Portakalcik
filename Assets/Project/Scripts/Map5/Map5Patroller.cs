using UnityEngine;

// Nesneyi A noktası ile B noktası arasında gidip getirir.
public class Map5Patroller : MonoBehaviour
{
    [SerializeField] private Transform pointB;      // Gidilecek B noktası (Inspector'dan sürüklenir)
    [SerializeField] private float speed = 2.0f;    // Saniyede kaç metre gideceği

    private Vector3 pointA;                         // A noktası = başladığımız yer
    private bool goingToB = true;                   // Şu an B'ye mi gidiyoruz?

    void Start()
    {
        // Başladığımız yeri A olarak kaydet
        pointA = transform.position;
    }

    void Update()
    {
        // B'ye gidiyorsak hedef B, değilse hedef A
        Vector3 target = goingToB ? pointB.position : pointA;

        // Hedefe doğru biraz ilerle
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Hedefe vardıysak yön değiştir
        if (transform.position == target)
        {
            goingToB = !goingToB;
        }
    }
}
