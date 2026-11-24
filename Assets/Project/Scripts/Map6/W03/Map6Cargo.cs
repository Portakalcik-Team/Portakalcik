using UnityEngine;

// Kargo nesnesinin taşınma ve fiziksel davranışlarını yönetir
public class Map6Cargo : MonoBehaviour
{
    [Header("Kargo Tipi Özelliği")]
    [SerializeField] private Map6CargoType type = null;

    [Header("Kargo Bırakma Özelliği")]
    [SerializeField] private float dropForce = 3f;
    [SerializeField] private float pickUpCooldown = 0.5f;

    private Rigidbody rb;
    private BoxCollider box;       // Katı çarpışma gövdesi
    private SphereCollider range;  // Alma tetikleyici alanı
    private float canPickUpAt;     // Tekrar alınabilme zamanı

    void Awake()
    {
        if (type == null) Debug.LogError(name + ": no Cargo Type");

        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();

        // Eğer alt nesnelerde (Visual vb.) mükerrer BoxCollider varsa tamamen kapat
        Collider[] allCols = GetComponentsInChildren<Collider>(true);
        foreach (Collider c in allCols)
        {
            if (c != box && c != range)
            {
                c.enabled = false;
            }
        }
    }

    public int Points()
    {
        return type != null ? type.points : 1;
    }

    public float Weight()
    {
        return type != null ? type.weight : 0f;
    }

    // Kargo şu anda alınabilir mi kontrolü
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    // Kargoyu oyuncunun taşıma noktasına bağla
    public void PickUp(Transform carryPoint)
    {
        if (rb != null)
        {
            rb.isKinematic = true;          // Fizik simülasyonunu durdur
            rb.detectCollisions = false;    // Fizik temas hesaplamasını kapat
        }

        // Taşıma anında çarpmaları ve tetikleyicileri kapat (karaktere takılmaması için)
        if (box != null) box.enabled = false;
        if (range != null) range.enabled = false;

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Kargoyu serbest bırak ve fırlat
    public void Drop()
    {
        transform.SetParent(null); // Ebeveynden ayrıl

        // Çarpışma ve alma tetikleyicisini tekrar aç
        if (box != null) box.enabled = true;
        if (range != null) range.enabled = true;

        if (rb != null)
        {
            rb.detectCollisions = true;
            rb.isKinematic = false; // Fiziği tekrar aç

            // İleriye ve yukarıya hafif itiş ver (baktığı yatay yöne doğru)
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            if (flatForward.sqrMagnitude < 0.001f) flatForward = Vector3.forward;
            else flatForward.Normalize();

            Vector3 push = (flatForward + Vector3.up * 0.5f).normalized;
            rb.AddForce(push * dropForce, ForceMode.Impulse);
        }

        // Bırakma sonrası kısa süreliğine hemen tekrar alınamasın
        canPickUpAt = Time.time + pickUpCooldown;
    }
}
