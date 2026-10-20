using UnityEngine;

using UnityEngine.InputSystem;

public class M10PlayerDrop : MonoBehaviour
{

private M10PlayerCarry carry;

private InputAction dropAction;

void Start()

{

// our own M10PlayerCarry, on this same object

carry = GetComponent<M10PlayerCarry>();

// "Interact" = the E key or the gamepad's North button

dropAction = InputSystem.actions.FindAction("Interact");

}

void Update()

{

// one press = one drop (DropCargo checks the hands)

if (dropAction.WasPressedThisFrame()) carry.DropCargo();

}

}