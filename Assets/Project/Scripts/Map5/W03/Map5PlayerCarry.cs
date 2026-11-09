using UnityEngine;

// Oyuncu kargoya değince kargoyu başının üstüne alır.
// W06: ağır kargo oyuncuyu da ağırlaştırır (zıplaması zayıflar).
public class Map5PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;   // başın üstündeki nokta

    private Map5Cargo carried;    // null = eller boş
    private Rigidbody rb;         // W06: oyuncunun gövdesi
    private float baseMass;       // W06: eller boşken kütlesi

    void Start()
    {
        // W06: gövdeyi bir kere bul, boş eldeki kütleyi hatırla
        rb = GetComponent<Rigidbody>();
        baseMass = rb.mass;
    }

    public bool IsCarrying()
    {
        return carried != null;
    }

    // Oyuncu bir trigger'a girince çalışır
    void OnTriggerEnter(Collider other)
    {
        Map5Cargo cargo = other.GetComponent<Map5Cargo>();
        if (cargo == null) return;          // kargo değil
        if (IsCarrying()) return;           // eller dolu
        if (!cargo.CanBePickedUp()) return; // az önce bırakıldı

        carried = cargo;
        cargo.PickUp(carryPoint);
        rb.mass = baseMass + cargo.Weight();   // W06: ağır kargo = ağır oyuncu
    }

    // Tehlikeler, canavar ve E tuşu bunu çağırır
    public void DropCargo()
    {
        if (!IsCarrying()) return;

        carried.Drop();
        carried = null;
        rb.mass = baseMass;   // W06: yine hafif
    }

    // Teslim alanı bunu çağırır
    public Map5Cargo HandOver()
    {
        Map5Cargo cargo = carried;
        carried = null;       // eller yine boş
        rb.mass = baseMass;   // W06: yine hafif
        return cargo;
    }
}
