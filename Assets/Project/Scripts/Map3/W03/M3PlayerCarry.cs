using UnityEngine;

// Map3 W03: oyuncuya eklenir, ayni anda tek kargo tasir.
public class M3PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;  // oyuncunun TasimaNoktasi cocugu
    private M3Cargo carried;                      // null = eller bos

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
    }

    // tehlike ve E tusu cagirir
    public void DropCargo()
    {
        if (!IsCarrying()) return;
        carried.Drop();
        carried = null;
    }

    // teslim alani cagirir
    public M3Cargo HandOver()
    {
        M3Cargo cargo = carried;
        carried = null;   // eller bos
        return cargo;
    }
}
