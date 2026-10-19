using UnityEngine;

// Map3 W03: tasinabilir kargo kutusu.
// Kok nesnede: Rigidbody, kati BoxCollider, alma alani SphereCollider (Is Trigger).
public class M3Cargo : MonoBehaviour
{
    [SerializeField] private float dropForce = 3f;      // birakinca itme gucu
    [SerializeField] private float pickUpDelay = 0.5f;  // birakinca bu kadar sn alinamaz

    private Rigidbody rb;
    private BoxCollider box;        // kati govde
    private SphereCollider range;   // alma alani
    private float canPickUpAt;      // tekrar alinabilecegi zaman

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
    }

    // simdi alinabilir mi?
    public bool CanBePickedUp()
    {
        return Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        rb.isKinematic = true;    // fizik dursun
        box.enabled = false;      // oyuncuya carpmasin
        range.enabled = false;    // tekrar alinmasin

        // tasima noktasinin cocugu ol
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        transform.SetParent(null);  // serbest kal
        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false;     // fizik geri gelsin

        // geriye ve yukari dogru it
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * dropForce, ForceMode.Impulse);

        canPickUpAt = Time.time + pickUpDelay;
    }
}
