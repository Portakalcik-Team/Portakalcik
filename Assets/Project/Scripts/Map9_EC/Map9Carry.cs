using UnityEngine;

// Seviye 9: oyuncunun eli, portakalı taşır
public class Map9Carry : MonoBehaviour
{
    // başın üstündeki nokta (CarryPoint)
    [SerializeField] private Transform carryPoint;

    private Map9Portakal carried;    // null = eller boş

    public bool IsCarrying()
    {
        return carried != null;
    }

    // oyuncu bir tetikleyiciye girince
    void OnTriggerEnter(Collider other)
    {
        Map9Portakal portakal = other.GetComponent<Map9Portakal>();
        if (portakal == null) return;          // portakal değil
        if (IsCarrying()) return;              // eller dolu
        if (!portakal.CanBePickedUp()) return; // az önce düştü

        carried = portakal;
        portakal.PickUp(carryPoint);
        Debug.Log("Portakal elinde — çıkışa götür!");
    }

    // canavarlar bunu çağırır
    public void DropPortakal()
    {
        if (!IsCarrying()) return;
        carried.Drop();
        carried = null;
        Debug.Log("Portakal düştü! Geri al.");
    }

    // çıkış bunu çağırır: portakalı verir, eller boşalır
    public Map9Portakal HandOver()
    {
        Map9Portakal portakal = carried;
        carried = null;
        return portakal;
    }
}