using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 11f, -7f);
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 0.8f, 0f);
    [SerializeField] private float followSmoothness = 8f;
    [SerializeField] private float rotationSmoothness = 10f;
    [SerializeField] private float fieldOfView = 45f;

    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = false;
        cam.fieldOfView = fieldOfView;
    }

    private void Start()
    {
        FindTarget();
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null) FindTarget();
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition,
            1f - Mathf.Exp(-followSmoothness * Time.deltaTime));

        Vector3 lookPoint = target.position + lookOffset;
        Quaternion desiredRotation = Quaternion.LookRotation(lookPoint - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation,
            1f - Mathf.Exp(-rotationSmoothness * Time.deltaTime));
    }

    private void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
        else
        {
            player = GameObject.Find("Player");
            if (player != null) target = player.transform;
        }
    }

    private void SnapToTarget()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        transform.LookAt(target.position + lookOffset);
    }
}
