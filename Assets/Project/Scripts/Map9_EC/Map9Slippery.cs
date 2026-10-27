using UnityEngine;

// Seviye 9: buz — üstündeki oyuncu tutunmasını kaybeder
public class Map9Slippery : MonoBehaviour
{
    // 1 = normal zemin · 0.08 = buz: çok az tutunma
    [SerializeField] private float grip = 0.08f;

    // Detect: bir şey bu yüzeye değmeye başladı
    void OnCollisionEnter(Collision hit)
    {
        // Who? sadece oyuncuda Map9PlayerMove var
        Map9PlayerMove move = hit.collider.GetComponent<Map9PlayerMove>();
        // Act: oyuncu tutunmasını kaybeder
        if (move != null) move.SetGrip(grip);
    }

    // Detect: değme bitti
    void OnCollisionExit(Collision hit)
    {
        Map9PlayerMove move = hit.collider.GetComponent<Map9PlayerMove>();
        if (move != null) move.SetGrip(1f);   // yeniden tam tutunma
    }
}