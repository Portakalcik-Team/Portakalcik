using UnityEngine;
using UnityEngine.InputSystem;

public class M10PlayerDrop : MonoBehaviour
{
    private Map10PlayerCarry carry;
    private InputAction dropAction;

    void Start()
    {
        carry = GetComponent<Map10PlayerCarry>();
        if (InputSystem.actions != null)
        {
            dropAction = InputSystem.actions.FindAction("Interact");
        }
    }

    void Update()
    {
        if (carry == null) carry = GetComponent<Map10PlayerCarry>();
        if (carry == null) return;

        bool dropPressed = false;
        if (dropAction != null && dropAction.WasPressedThisFrame())
        {
            dropPressed = true;
        }
        else if (Keyboard.current != null && Keyboard.current.qKey.wasPressedThisFrame)
        {
            dropPressed = true;
        }

        if (dropPressed)
        {
            carry.DropCargo();
        }
    }
}
