using UnityEngine;

// Kargo nesnesinin taşınma ve fiziksel davranışlarını yönetir
public class Map6Cargo : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider box;       // Katı çarpışma gövdesi
    private SphereCollider range;  // Alma tetikleyici alanı
    private float canPickUpAt;     // Tekrar alınabilme zamanı

    void Awake()
    {
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

            // İleriye ve yukarıya hafif itiş ver (baktığı yöne doğru)
            Vector3 push = (transform.forward + Vector3.up * 0.5f).normalized;
            rb.AddForce(push * 3f, ForceMode.Impulse);
        }

        // 0.5 saniye boyunca hemen tekrar alınamasın
        canPickUpAt = Time.time + 0.5f;
    }
}
