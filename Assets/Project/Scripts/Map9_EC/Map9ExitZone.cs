using UnityEngine;

// Seviye 9: çıkış — portakal teslim edilir, puanı Round'a ödenir
public class Map9ExitZone : MonoBehaviour
{
    // skoru artık Round tutuyor
    [SerializeField] private Map9Round round;

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

        // Act: portakalı teslim al, puanını Round'a öde, sahneden kaldır
        Map9Portakal portakal = carry.HandOver();
        round.AddPoints(portakal.Points());
        Destroy(portakal.gameObject);
    }
}
