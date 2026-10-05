using UnityEngine;

// Map_3: W02 slayttaki Spinner. Visual cocugunu kendi ekseninde dondurur (ana nesneye degil).
public class map3_spinner : MonoBehaviour
{
    // derece / SANIYE
    [SerializeField] private float rotationSpeed = 90f;

    void Update()
    {
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}
