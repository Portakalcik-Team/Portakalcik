using UnityEngine;

public class Map0Spinner : MonoBehaviour
{
    // saniyede kaç derece döner
    [SerializeField] private float rotationSpeed = 90f;

    void Update()
    {
        // Y ekseninde döndür, Time.deltaTime ile kare hızından bağımsız
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
