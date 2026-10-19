using UnityEngine;

// Map_3: W03 slayttaki PlayerCarry. Oyuncuya eklenir, ayni anda tek kasa tasir.
public class PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;   // Inspector'da oyuncunun CarryPoint cocugu
    private Cargo carried;                           // null = eller bos

    void Awake()
    {
        if (carryPoint == null)
        {
            Debug.LogError(name + ": PlayerCarry'nin Carry Point alani bos.", this);
        }
    }

    public bool IsCarrying()
    {
        return carried != null;
    }

    // oyuncu bir trigger'a girdiginde calisir
    void OnTriggerEnter(Collider other)
    {
        Cargo cargo = other.GetComponentInParent<Cargo>();
        if (cargo == null) return;                 // kasa degil
        if (IsCarrying()) return;                  // eller dolu
        if (!cargo.CanBePickedUp()) return;        // yeni dusuruldu
        if (carryPoint == null) return;            // hata Awake'te yazildi

        carried = cargo;
        cargo.PickUp(carryPoint);
    }

    // tehlikeler cagirir
    public void DropCargo()
    {
        if (!IsCarrying()) return;
        carried.Drop();
        carried = null;
    }

    // yesil alan cagirir
    public Cargo HandOver()
    {
        Cargo cargo = carried;
        carried = null;   // eller yeniden bos
        return cargo;
    }
}
