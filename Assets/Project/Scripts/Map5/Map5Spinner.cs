using UnityEngine;

// Nesneyi kendi etrafında döndürür.
public class Map5Spinner : MonoBehaviour
{
    // Saniyede kaç derece döneceği (Inspector'dan değiştirilebilir)
    [SerializeField] private float rotationSpeed = 90f;

    void Update()
    {
        // Her karede biraz döndür. Time.deltaTime sayesinde hız her bilgisayarda aynı olur.
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
