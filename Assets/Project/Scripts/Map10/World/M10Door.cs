using UnityEngine;

public class M10Door : MonoBehaviour
{
    [Header("Kapı Ayarları")]
    [SerializeField] private float openDistance = 3.5f;   // Oyuncu bu kadar yaklaşınca açılır
    [SerializeField] private float liftHeight = 3.8f;     // Yukarı kalkma yüksekliği (asla yere batmaz, boşluk oluşturmaz!)
    [SerializeField] private float openSpeed = 4.0f;      // Açılma hızı

    private Vector3 closedPos;
    private Vector3 targetPos;
    private bool isOpen = false;
    private Transform player;
    private Collider col;
    private Renderer rend;

    void Start()
    {
        closedPos = transform.position;
        targetPos = closedPos;
        col = GetComponent<Collider>();
        rend = GetComponent<Renderer>();

        // Güvenlik: Kapının altında garanti sağlam zemin (taban) oluştur (oyuncu asla boşluğa düşmesin)
        EnsureSolidFloor();

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    /// <summary>
    /// Kapının altına sağlam zemin bloğu koyar, böylece kapı ne olursa olsun oyuncu zeminden düşemez.
    /// </summary>
    private void EnsureSolidFloor()
    {
        Transform existingFloor = transform.parent != null ? transform.parent.Find($"Floor_{name}") : null;
        if (existingFloor != null) return;

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = $"Floor_{name}";
        if (transform.parent != null) floor.transform.SetParent(transform.parent);

        // Kapının tam altındaki zemin seviyesine yerleştir
        floor.transform.position = new Vector3(closedPos.x, 0.05f, closedPos.z);
        floor.transform.localScale = new Vector3(3.2f, 0.2f, 3.2f);

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material floorMat = new Material(shader) { color = new Color(0.28f, 0.28f, 0.3f) }; // Koyu taş zemin
        floor.GetComponent<Renderer>().material = floorMat;

        // Katı BoxCollider (asla trigger değil, sağlam basamak)
        BoxCollider floorCol = floor.GetComponent<BoxCollider>();
        if (floorCol != null)
        {
            floorCol.isTrigger = false;
        }
    }

    void Update()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
            return;
        }

        // Oyuncu yaklaştığında kapı yukarı doğru açılsın
        if (!isOpen)
        {
            float dist = Vector3.Distance(transform.position, player.position);
            if (dist <= openDistance)
            {
                Open();
            }
        }

        // Kapı yukarı doğru asansör/kale kapısı gibi kalkar
        if (transform.position != targetPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, openSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponent<Map10MandarinBullet>() != null)
        {
            Open();
        }
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.CompareTag("Player") || hit.gameObject.GetComponent<Map10MandarinBullet>() != null)
        {
            Open();
        }
    }

    public void Open()
    {
        if (isOpen) return;
        isOpen = true;

        // KAPI YUKARI KALKAR (Tabanda asla boşluk oluşmaz!)
        targetPos = closedPos + new Vector3(0f, liftHeight, 0f);

        // Çarpışmayı kaldır ki oyuncu altından rahatça yürüyüp geçebilsin
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Kapı rengini açık yeşile dönüştür (geçiş serbest göstergesi)
        if (rend != null)
        {
            rend.material.color = new Color(0.2f, 0.85f, 0.4f);
        }

        SpawnOpenSparks();
        Debug.Log($"[Map10Door] 🏰 Kısayol Kapısı ({name}) yukarı kalktı! Geçiş tamamen güvenli ve açık.");
    }

    private void SpawnOpenSparks()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        for (int i = 0; i < 8; i++)
        {
            GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            spark.name = "DoorSpark";
            spark.transform.position = transform.position + new Vector3(Random.Range(-1.2f, 1.2f), 0.5f, Random.Range(-0.3f, 0.3f));
            spark.transform.localScale = Vector3.one * Random.Range(0.12f, 0.22f);

            Collider c = spark.GetComponent<Collider>();
            if (c != null) Destroy(c);

            spark.GetComponent<Renderer>().material = new Material(shader) { color = new Color(0.4f, 1f, 0.6f) };

            Rigidbody rb = spark.AddComponent<Rigidbody>();
            rb.mass = 0.05f;
            rb.linearVelocity = (Vector3.up * 2f + Random.insideUnitSphere * 1f) * 2f;

            Destroy(spark, 1.2f);
        }
    }
}
