using UnityEngine;

public class Map6Spinner : MonoBehaviour
{
    [Header("Dönüş Hızı (Spinner) Özelliği")]
    // Derece / saniye cinsinden dönüş hızı
    [SerializeField] private float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
