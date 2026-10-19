using UnityEngine;

// Oyuncu kargoya değince kargoyu başının üstüne alır.
public class Map5PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;   // başın üstündeki nokta

    private Map5Cargo carried;    // null = eller boş

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
    }

    // Tehlikeler ve E tuşu bunu çağırır
    public void DropCargo()
    {
        if (!IsCarrying()) return;

        carried.Drop();
        carried = null;
    }

    // Teslim alanı bunu çağırır
    public Map5Cargo HandOver()
    {
        Map5Cargo cargo = carried;
        carried = null;   // eller yine boş
        return cargo;
    }
}
