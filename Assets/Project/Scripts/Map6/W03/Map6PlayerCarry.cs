using UnityEngine;

// Oyuncunun kargo taşıma, düşürme ve teslim etme mekaniği
public class Map6PlayerCarry : MonoBehaviour
{
    [Header("Kargo Taşıma Özelliği")]
    [SerializeField] private Transform carryPoint;
    private Map6Cargo carried; // Boşsa eller serbest
    private Rigidbody rb;
    private float baseMass = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null) baseMass = rb.mass;
    }

    void Awake()
    {
        if (carryPoint == null)
        {
            Transform[] all = GetComponentsInChildren<Transform>(true);
            foreach (Transform t in all)
            {
                if (t.name == "CarryPoint" || t.name == "TasimaNoktasi")
                {
                    carryPoint = t;
                    break;
                }
            }
            if (carryPoint == null) carryPoint = transform;
        }
    }

    // Oyuncunun elinde kargo var mı?
    public bool IsCarrying()
    {
        return carried != null;
    }

    // Kargo tetikleyici alanına girildiğinde çalışır
    void OnTriggerEnter(Collider other)
    {
        TryPickUp(other);
    }

    // Tetikleyici alanında kalındığında çalışır (bırakma bekleme süresi dolunca tekrar alabilmek için)
    void OnTriggerStay(Collider other)
    {
        TryPickUp(other);
    }

    private void TryPickUp(Collider other)
    {
        if (IsCarrying()) return;            // Eller doluysa alma

        Map6Cargo cargo = other.GetComponent<Map6Cargo>();
        if (cargo == null) cargo = other.GetComponentInParent<Map6Cargo>();

        if (cargo == null) return;           // Kargo değilse dur
        if (!cargo.CanBePickedUp()) return;  // Bekleme süresi dolmadıysa dur

        carried = cargo;
        cargo.PickUp(carryPoint);
        if (rb != null) rb.mass = baseMass + cargo.Weight();
    }

    // Tehlike veya bırakma tuşu çağırır
    public void DropCargo()
    {
        if (!IsCarrying()) return;

        carried.Drop();
        carried = null;
        if (rb != null) rb.mass = baseMass;
    }

    // Yeşil teslimat alanı çağırır
    public Map6Cargo HandOver()
    {
        Map6Cargo cargo = carried;
        carried = null; // Eller tekrar boş
        if (rb != null) rb.mass = baseMass;
        return cargo;
    }
}