using UnityEngine;

public class Map8_ExitDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Map8_GameManager.Instance != null && Map8_GameManager.Instance.CikisAcikMi())
            {
                Debug.Log("TEBRİKLER! Yeterli puana ulaştın, bölüm tamamlandı!");
            }
            else
            {
                int kalan = Map8_GameManager.Instance.cikisIcinGerekliPuan - Map8_GameManager.Instance.mevcutPuan;
                Debug.Log("Çıkış kilitli! Kapıyı açmak için daha " + kalan + " tane puan lazım.");
            }
        }
    }
}
