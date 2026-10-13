using UnityEngine;
using UnityEngine.InputSystem;

public class Map6PlayerCarry : MonoBehaviour
{
    [SerializeField] private Transform carryPoint;

    private Map6Cargo currentCargo;
    private Map6Cargo nearbyCargo;

    void Update()
    {
        // Yeni Input System ile klavyeden E tuşunu kontrol etme
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (currentCargo != null)
            {
                currentCargo.Drop();
                currentCargo = null;
            }
            else if (nearbyCargo != null)
            {
                currentCargo = nearbyCargo;
                currentCargo.PickUp(carryPoint);
                nearbyCargo = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Map6Cargo cargo = other.GetComponent<Map6Cargo>() ?? other.GetComponentInParent<Map6Cargo>();
        if (cargo != null)
        {
            nearbyCargo = cargo;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Map6Cargo cargo = other.GetComponent<Map6Cargo>() ?? other.GetComponentInParent<Map6Cargo>();
        if (cargo != null && nearbyCargo == cargo)
        {
            nearbyCargo = null;
        }
    }
}