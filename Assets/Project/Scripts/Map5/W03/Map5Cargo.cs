using UnityEngine;

// Taşınabilen kargo: oyuncunun başına çıkar, bırakılınca fiziğe döner.
public class Map5Cargo : MonoBehaviour
{
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
