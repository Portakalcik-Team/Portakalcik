using UnityEngine;
using UnityEngine.InputSystem;

public class Map0PlayerDrop : MonoBehaviour
{
    private Map0PlayerCarry carry;
    private InputAction dropAction;

    void Start()
    {
        // aynı nesnedeki Map0PlayerCarry
        carry = GetComponent<Map0PlayerCarry>();

        // Tuşları kodla tanımla (InputSystem_Actions dosyası projede yok)
        dropAction = new InputAction("Interact");
        dropAction.AddBinding("<Keyboard>/e");
        dropAction.AddBinding("<Gamepad>/buttonNorth");
        dropAction.Enable();
    }

    void OnDestroy()
    {
        dropAction?.Disable();
    }

    void Update()
    {
        // bir basış = bir bırakma (eller boşsa DropCargo bir şey yapmaz)
        if (dropAction.WasPressedThisFrame()) carry.DropCargo();
    }
}
