using UnityEngine;

// Seviye 9: çıkış — portakalı getiren oyuncu teslim eder, seviye biter
public class Map9ExitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Who? sadece eli olan: oyuncu
        Map9Carry carry = other.GetComponent<Map9Carry>();
        if (carry == null) return;

        // portakal yoksa çıkış kilitli
        if (!carry.IsCarrying())
        {
            Debug.Log("Çıkış kilitli: Portakal_01'deki portakalı getir!");
            return;
        }

        // Act: portakalı teslim al, sahneden kaldır
        Map9Portakal portakal = carry.HandOver();
        Destroy(portakal.gameObject);
        Debug.Log("Portakal teslim edildi! Seviye 9 tamamlandı.");

        // çantada ne birikti?
        Map9Bag bag = other.GetComponent<Map9Bag>();
        if (bag != null) Debug.Log("Çantada: " + bag.Summary());
    }
}