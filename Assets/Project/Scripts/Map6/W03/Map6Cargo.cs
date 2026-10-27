using UnityEngine;

// Kargo nesnesinin taşınma ve fiziksel davranışlarını yönetir
public class Map6Cargo : MonoBehaviour
{
    private Rigidbody rb;
    private Collider box;       // Katı çarpışma gövdesi
    private Collider range;     // Alma tetikleyici alanı
    private float canPickUpAt;  // Tekrar alınabilme zamanı

    void Awake()
    {
        // Bileşenleri baştan önbelleğe al
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
    }

    // Kargo şu anda alınabilir mi kontrolü
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    // Kargoyu oyuncunun taşıma noktasına bağla
    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true;          // Fizik hareketini durdur
        if (box != null) box.enabled = false;      // Çarpmaları kapat
        if (range != null) range.enabled = false;  // Tekrar tetiklemeyi kapat

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Kargoyu serbest bırak ve geriye doğru fırlat
    public void Drop()
    {
        transform.SetParent(null);      // Ebeveynden ayrıl
        if (box != null) box.enabled = true;
        if (range != null) range.enabled = true;
        rb.isKinematic = false;         // Fiziği tekrar aç

        // Geriye ve yukarıya hafif itiş ver
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * 3f, ForceMode.Impulse);

        // 0.5 saniye boyunca hemen tekrar alınamasın
        canPickUpAt = Time.time + 0.5f;
    }
}