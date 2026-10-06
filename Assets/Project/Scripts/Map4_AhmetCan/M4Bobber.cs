using UnityEngine;

public class M4Bobber : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;
    private Vector3 startLocalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
       float offset = Mathf.Sin(Time.time*bobSpeed)*bobHeight;
       transform.localPosition = startLocalPosition + Vector3.up*offset;
    }
}
