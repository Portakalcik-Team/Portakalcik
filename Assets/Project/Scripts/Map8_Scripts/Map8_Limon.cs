using UnityEngine;

public class Map8_Limon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Map8_GameManager.Instance != null)
            {
                Map8_GameManager.Instance.PuanEkle(1);
            }
            gameObject.SetActive(false);
        }
    }
}

