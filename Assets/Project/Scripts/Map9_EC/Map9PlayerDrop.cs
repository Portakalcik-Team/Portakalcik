using UnityEngine;
using UnityEngine.InputSystem;

// Seviye 9: E tuşu elindeki portakalı bırakır
public class Map9PlayerDrop : MonoBehaviour
{
    private Map9Carry carry;
    private InputAction dropAction;

    void Start()
    {
        // aynı objedeki Map9Carry (Hafta 3)
        carry = GetComponent<Map9Carry>();
        // "Interact" = E tuşu ya da gamepad'in üst düğmesi
        dropAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        // bir basış = bir bırakma (DropPortakal elleri kontrol eder)
        if (dropAction.WasPressedThisFrame()) carry.DropPortakal();
    }
}