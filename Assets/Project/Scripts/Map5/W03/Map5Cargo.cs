using UnityEngine;

// Taşınabilen kargo: oyuncunun başına çıkar, bırakılınca fiziğe döner.
// W06: hafif mi ağır mı, bir veri dosyası (Map5CargoType) karar verir.
public class Map5Cargo : MonoBehaviour
{
    // W06: hafif mi ağır mı? Bir veri dosyası söyler (CT_Map5_Hafif / CT_Map5_Agir)
    [SerializeField] private Map5CargoType type;

    private Rigidbody rb;
    private Collider box;      // katı gövde
    private Collider range;    // alma alanı (trigger)
    private float canPickUpAt; // tekrar alınabileceği zaman

    void Awake()
    {
        // Kendi bileşenlerimizi bir kere bul
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
        // W06: Type kutusu boş mu? Kırmızı yazıyla söyle
        if (type == null) Debug.LogError(name + ": Cargo Type yok");
    }

    // W06: bu kargo hakkında iki soru, cevabı türünden gelir
    public int Points()
    {
        return type.Points();
    }

    public float Weight()
    {
        return type.Weight();
    }

    // Soru: "şimdi alınabilir miyim?"
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true;    // fizik dursun
        box.enabled = false;      // çarpışma yok
        range.enabled = false;    // tekrar alınmasın
        // Taşıma noktasının çocuğu ol
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        transform.SetParent(null); // serbest
        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false;    // fizik geri gelsin
        // Oyuncudan uzağa, geriye ve yukarı it
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * 3f, ForceMode.Impulse);
        // 0.5 sn boyunca tekrar alınamasın
        canPickUpAt = Time.time + 0.5f;
    }
}
