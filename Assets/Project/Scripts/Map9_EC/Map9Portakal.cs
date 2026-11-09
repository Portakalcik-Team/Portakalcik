using UnityEngine;

// Seviye 9: elde taşınan portakal, çıkışta teslim
public class Map9Portakal : MonoBehaviour
{
    private Rigidbody rb;
    private Collider box;        // katı gövde
    private Collider range;      // alma alanı (trigger)
    private float canPickUpAt;   // tekrar alınabileceği an

    void Awake()
    {
        // kendi bileşenlerimizi bir kez bul, sakla
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
    }

    // bir soru: "şu an alınabilir miyim?"
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    // oyuncu alınca: CarryPoint'in çocuğu ol
    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true;     // fizik dursun, düşmesin
        box.enabled = false;       // oyuncuya çarpmasın
        range.enabled = false;     // tekrar alınmasın
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // canavar vurunca: serbest kal, yere düş
    public void Drop()
    {
        transform.SetParent(null); // kimsenin çocuğu değil
        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false;    // fizik geri gelsin
        // geriye ve yukarı it, oyuncudan uzaklaşsın
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * 3f, ForceMode.Impulse);
        // 0.5 sn boyunca tekrar alınamasın
        canPickUpAt = Time.time + 0.5f;
    }
}