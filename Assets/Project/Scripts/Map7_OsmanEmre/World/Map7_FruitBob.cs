using UnityEngine;

public class Map7_FruitBob : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 90f;    // Saniyedeki dönme açısı
    [SerializeField] private float bobHeight = 0.15f;  // Yukarı-aşağı süzülme mesafesi
    [SerializeField] private float bobSpeed = 2f;      // Süzülme hızı

    private Vector3 startLocalPosition;
    private float timer = 0f;

    void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
        timer += Time.deltaTime;
        float offset = Mathf.Sin(timer * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}