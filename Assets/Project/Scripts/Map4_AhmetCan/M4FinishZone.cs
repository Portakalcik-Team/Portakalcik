using UnityEngine;

public class M4FinishZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Çıkışa Ulaştınız");
    }
}
