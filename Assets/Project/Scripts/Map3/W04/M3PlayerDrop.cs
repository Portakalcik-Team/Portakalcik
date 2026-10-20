using UnityEngine;
using UnityEngine.InputSystem;

// Map3 W04: E / Kuzey tusu ile tasinan kargoyu birakir.
public class M3PlayerDrop : MonoBehaviour
{
    private M3PlayerCarry carry;
    private InputAction dropAction;

    void Start()
    {
        // ayni nesnedeki M3PlayerCarry
        carry = GetComponent<M3PlayerCarry>();

        dropAction = new InputAction("Interact");
        dropAction.AddBinding("<Keyboard>/e");
        dropAction.AddBinding("<Gamepad>/buttonNorth");
        dropAction.Enable();
    }

    void Update()
    {
        // bir basis = bir birakma (eller bossa DropCargo bir sey yapmaz)
        if (dropAction.WasPressedThisFrame()) carry.DropCargo();
    }

    void OnDestroy()
    {
        if (dropAction != null) dropAction.Disable();
    }
}
