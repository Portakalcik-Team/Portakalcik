using UnityEngine;
using UnityEngine.InputSystem;

// E tuşu (veya gamepad'in üst tuşu) taşınan kargoyu bırakır.
public class Map5PlayerDrop : MonoBehaviour
{
    private Map5PlayerCarry carry;
    private InputAction dropAction;

    void Start()
    {
        // Aynı nesnedeki Map5PlayerCarry
        carry = GetComponent<Map5PlayerCarry>();

        // "Interact" tuşu: E ve gamepad'in üst tuşu
        dropAction = new InputAction("Interact", binding: "<Keyboard>/e");
        dropAction.AddBinding("<Gamepad>/buttonNorth");
        dropAction.Enable();
    }

    void Update()
    {
        // Bir basış = bir bırakma (eller boşsa DropCargo hiçbir şey yapmaz)
        if (dropAction.WasPressedThisFrame()) carry.DropCargo();
    }

    void OnDestroy()
    {
        dropAction.Disable();
    }
}
