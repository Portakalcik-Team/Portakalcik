using UnityEngine;

// Map_3: W03 slayttaki Cargo. Tasinabilir mandalina kasasi.
// Kok nesnede: Rigidbody + kati BoxCollider + toplama alani SphereCollider (Is Trigger).
// Donme/suzulme sadece Visual cocugunda calisir.
public class Cargo : MonoBehaviour
{
    [SerializeField] private float dropForce = 3f;        // slayttaki 3f
    [SerializeField] private float pickUpDelay = 0.5f;    // dusurdukten sonra bekleme

    private Rigidbody rb;
    private BoxCollider box;        // kati govde
    private SphereCollider range;   // toplama trigger'i
    private float canPickUpAt;      // yeniden alinabilecegi zaman
    private bool carried;
    private bool delivered;

    void Awake()   // kendi bilesenlerimizi bir kez bul
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        range = GetComponent<SphereCollider>();
        if (rb == null || box == null || range == null)
        {
            Debug.LogError(name + ": Cargo icin kok nesnede Rigidbody, BoxCollider ve SphereCollider gerekli.", this);
        }
    }

    // "Su an alinabilir miyim?"
    public bool CanBePickedUp()
    {
        return !delivered && !carried && rb != null && Time.time >= canPickUpAt;
    }

    public void PickUp(Transform carryPoint)
    {
        if (carryPoint == null)
        {
            Debug.LogError(name + ": CarryPoint bos, kasa alinamadi.", this);
            return;
        }
        carried = true;
        rb.isKinematic = true;     // fizik durur
        box.enabled = false;       // oyuncuya carpmasin
        range.enabled = false;     // tekrar alinmasin

        // CarryPoint'in cocugu ol
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop()
    {
        carried = false;
        transform.SetParent(null);   // serbest
        MoveOutOfWalls();             // duvarin/zeminin icinde kalmasin

        box.enabled = true;
        range.enabled = true;
        rb.isKinematic = false;      // fizik yeniden

        // oyuncunun baktigi yonun tersine ve yukari it
        Vector3 push = -transform.forward + Vector3.up;
        rb.AddForce(push * dropForce, ForceMode.Impulse);

        // 0.5 sn yeniden alinamaz
        canPickUpAt = Time.time + pickUpDelay;
    }

    // DeliveryZone cagirir: ayni kasa ikinci kez teslim edilemesin
    public void MarkDelivered()
    {
        delivered = true;
    }

    // Kasa birakildigi yerde bir duvara veya zemine gomuluyse disari it
    private void MoveOutOfWalls()
    {
        Vector3 center = transform.TransformPoint(box.center);
        Vector3 half = Vector3.Scale(box.size, transform.lossyScale) * 0.5f;
        Collider[] hits = Physics.OverlapBox(center, half, transform.rotation, ~0, QueryTriggerInteraction.Ignore);
        foreach (Collider hit in hits)
        {
            if (hit == box || hit.attachedRigidbody != null) continue;   // kendisi ve oyuncu gibi govdeler haric
            box.enabled = true;   // ComputePenetration etkin collider ister
            if (Physics.ComputePenetration(box, transform.position, transform.rotation,
                                           hit, hit.transform.position, hit.transform.rotation,
                                           out Vector3 direction, out float distance))
            {
                transform.position += direction * (distance + 0.01f);
            }
        }
    }
}
