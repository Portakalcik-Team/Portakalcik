using UnityEngine;

public class Map6Spinner : MonoBehaviour
{
    // Derece / saniye cinsinden dönüş hızı
    [SerializeField] private float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
