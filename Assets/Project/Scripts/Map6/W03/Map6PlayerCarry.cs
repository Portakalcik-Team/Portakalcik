using UnityEngine;

// Oyuncunun kargo taşıma, düşürme ve teslim etme mekaniği
public class Map6PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;
    private Map6Cargo carried; // Boşsa eller serbest

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
    }

    // Tehlike veya bırakma tuşu çağırır
    public void DropCargo()
    {
        if (!IsCarrying()) return;

        carried.Drop();
        carried = null;
    }

    // Yeşil teslimat alanı çağırır
    public Map6Cargo HandOver()
    {
        Map6Cargo cargo = carried;
        carried = null; // Eller tekrar boş
        return cargo;
    }
}