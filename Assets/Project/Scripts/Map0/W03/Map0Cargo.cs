using UnityEngine;

public class Map0Cargo : MonoBehaviour
{
    [SerializeField] private Map0CargoType type; // hafif mi ağır mı? veri dosyası karar verir

    private Rigidbody rb;
    private Collider box;      // katı gövde
    private Collider range;    // alma alanı (trigger)
    private float canPickUpAt; // tekrar alınabileceği zaman

    void Awake()
    {
        // kendi bileşenlerimizi bir kere bul
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
        if (type == null) Debug.LogError(name + ": Cargo Type yok");
    }

    public int Points() { return type.Points(); }

    public float Weight() { return type.Weight(); }

    // soru: "şimdi alınabilir miyim?"
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true; // fizik durur
        box.enabled = false;   // çarpışma yok
        range.enabled = false; // tekrar alınmaz
        // taşıma noktasının çocuğu ol
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        transform.SetParent(null); // serbest
        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false;    // fizik geri gelir
        // geriye ve yukarı it, oyuncudan uzaklaşsın
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * 3f, ForceMode.Impulse);
        // 0.5 saniye tekrar alınmasın
        canPickUpAt = Time.time + 0.5f;
    }
}
