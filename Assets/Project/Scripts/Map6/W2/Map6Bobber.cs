using UnityEngine;

public class Map6Bobber : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.25f; // Yukarı ve aşağı hareket mesafesi (metre)
    [SerializeField] private float bobSpeed = 2f;     // Süzülme hızı
    private Vector3 startLocalPosition;               // Başlangıç konumunu hatırlar

    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Sinüs dalgası ile süzülme hareketi
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}
