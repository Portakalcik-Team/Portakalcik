using UnityEngine;
using UnityEngine.InputSystem;

// E tuşuna basıldığında taşınan kargoyu serbest bırakır
public class Map6PlayerDrop : MonoBehaviour
{
    private Map6PlayerCarry carry;
    private InputAction dropAction;

    void Start()
    {
        carry = GetComponent<Map6PlayerCarry>();

        // Bırakma (Interact) aksiyonunu kodla tanımla
        dropAction = new InputAction("Interact", InputActionType.Button);
        dropAction.AddBinding("<Keyboard>/e");
        dropAction.AddBinding("<Gamepad>/buttonNorth");
        dropAction.Enable();
    }

    void OnDestroy()
    {
        if (dropAction != null)
        {
            dropAction.Disable();
            dropAction.Dispose();
        }
    }

    void Update()
    {
        bool dropPressed = false;
        if (dropAction != null && dropAction.WasPressedThisFrame())
        {
            dropPressed = true;
        }
        else if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            dropPressed = true;
        }

        if (dropPressed && carry != null)
        {
            carry.DropCargo();
        }
    }
}
