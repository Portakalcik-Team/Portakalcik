using UnityEngine;

public class Map10CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 22f, 12f);
    [SerializeField] private float smoothSpeed = 12f;
    [SerializeField] private float pitchAngle = 55f;
    [SerializeField] private float yawAngle = 180f;

    void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) target = player.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(pitchAngle, yawAngle, 0f);
    }
}
