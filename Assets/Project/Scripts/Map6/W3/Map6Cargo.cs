using UnityEngine;

public class Map6Cargo : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void PickUp(Transform carryPoint)
    {
        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;

        if (rb != null) rb.isKinematic = true;
        if (col != null) col.enabled = false;
    }

    public void Drop()
    {
        transform.SetParent(null);

        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;
    }
}