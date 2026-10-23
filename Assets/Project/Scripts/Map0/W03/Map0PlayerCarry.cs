using UnityEngine;

public class Map0PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint; // başın üstündeki nokta

    private Map0Cargo carried; // null = eller boş

    public bool IsCarrying()
    {
        return carried != null;
    }

    // oyuncu bir trigger'a girince çalışır
    void OnTriggerEnter(Collider other)
    {
        Map0Cargo cargo = other.GetComponent<Map0Cargo>();
        if (cargo == null) return;           // kargo değil
        if (IsCarrying()) return;            // eller dolu
        if (!cargo.CanBePickedUp()) return;  // az önce düştü

        carried = cargo;
        cargo.PickUp(carryPoint);
    }

    // tehlikeler bunu çağırır
    public void DropCargo()
    {
        if (!IsCarrying()) return;

        carried.Drop();
        carried = null;
    }

    // teslim alanı bunu çağırır
    public Map0Cargo HandOver()
    {
        Map0Cargo cargo = carried;
        carried = null; // eller yine boş
        return cargo;
    }
}
