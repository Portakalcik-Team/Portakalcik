using UnityEngine;

// Map10 W03: goes on the player, carries one cargo at a time.
public class M10PlayerCarry : MonoBehaviour
{
[SerializeField] private Transform carryPoint; // the player's carry point child
private m10Cargo carried; // null = hands empty

public bool IsCarrying()
{
return carried != null;
}

// the player entered a trigger
void OnTriggerEnter(Collider other)
{
m10Cargo cargo = other.GetComponent<m10Cargo>();
if (cargo == null) return; // not cargo
if (IsCarrying()) return; // hands full
if (!cargo.CanBePickedUp()) return; // just dropped

carried = cargo;
cargo.PickUp(carryPoint);
}

// hazards and the E key call this
public void DropCargo()
{
if (!IsCarrying()) return;
carried.Drop();
carried = null;
}

// the delivery zone calls this
public m10Cargo HandOver()
{
m10Cargo cargo = carried;
carried = null; // hands empty again
return cargo;
}
}
