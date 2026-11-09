using UnityEngine;

public class Map0Bobber : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.25f; // kaç metre yukarı-aşağı
    [SerializeField] private float bobSpeed = 2f;     // ne kadar hızlı süzülür

    private Vector3 startLocalPosition;

    void Start()
    {
        // başladığımız yeri hatırla
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Sin -1 ile +1 arası gider: -0.25 ile +0.25 metre arası
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}
