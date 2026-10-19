using UnityEngine;

// Map5'in canavarı (Labirent Hayaleti, Quaternius Ghost modeli): iki nokta arasında devriye gezer ve hep oyuncuya bakar.
// Oyuncuya değerse kargosunu düşürür ve onu geri iter.
public class Map5Canavar : MonoBehaviour
{
    [SerializeField] private Transform pointB;        // devriyenin B noktası
    [SerializeField] private Transform player;        // bakacağı oyuncu
    [SerializeField] private float speed = 2.5f;      // saniyede kaç metre
    [SerializeField] private float pushForce = 10f;   // oyuncuyu ne kadar iter

    private Vector3 pointA;          // A noktası = başladığı yer
    private bool goingToB = true;

    void Start()
    {
        pointA = transform.position;
    }

    void Update()
    {
        // A ile B arasında gidip gel
        Vector3 target = goingToB ? pointB.position : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target) goingToB = !goingToB;

        // Hep oyuncuya bak (sadece yatayda dön)
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look != Vector3.zero) transform.rotation = Quaternion.LookRotation(look);
    }

    // Algıla: bir şey canavara değdi
    void OnTriggerEnter(Collider other)
    {
        // Kim? Sadece oyuncu (Map5PlayerCarry onda var)
        Map5PlayerCarry carry = other.GetComponent<Map5PlayerCarry>();
        if (carry == null) return;

        // Yap: kargoyu düşür, oyuncuyu geriye ve biraz yukarı it
        carry.DropCargo();
        Vector3 push = other.transform.position - transform.position;
        push.y = 0f;
        other.attachedRigidbody.AddForce((push.normalized + Vector3.up * 0.5f) * pushForce, ForceMode.Impulse);
        Debug.Log("Canavar seni yakaladı!");
    }
}
