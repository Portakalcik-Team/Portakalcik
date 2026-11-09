using UnityEngine;

// Map3 W03: oyuncuya eklenir, ayni anda tek kargo tasir.
public class M3PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;  // oyuncunun TasimaNoktasi cocugu
    private M3Cargo carried;                      // null = eller bos
    private Rigidbody rb;
    private float baseMass;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        baseMass = rb.mass;
    }

    public bool IsCarrying()
    {
        return carried != null;
    }

    // oyuncu bir trigger'a girince
    void OnTriggerEnter(Collider other)
    {
        M3Cargo cargo = other.GetComponent<M3Cargo>();
        if (cargo == null) return;               // kargo degil
        if (IsCarrying()) return;                // eller dolu
        if (!cargo.CanBePickedUp()) return;      // az once birakildi

        carried = cargo;
        cargo.PickUp(carryPoint);
        rb.mass = baseMass + cargo.Weight();
    }

    // tehlike ve E tusu cagirir
    public void DropCargo()
    {
        if (!IsCarrying()) return;
        carried.Drop();
        carried = null;
        rb.mass = baseMass;
    }

    // teslim alani cagirir
    public M3Cargo HandOver()
    {
        M3Cargo cargo = carried;
        carried = null;   // eller bos
        rb.mass = baseMass;
        return cargo;
    }
}
