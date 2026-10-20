using UnityEngine;

public class Map8_Enemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Map8_GameManager.Instance != null)
            {
                Map8_GameManager.Instance.PuanEksilt(1);
            }
        }
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Map8_GameManager.Instance != null)
            {
                Map8_GameManager.Instance.PuanEksilt(1);
            }
        }
    }
    */
}
