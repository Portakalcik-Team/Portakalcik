using UnityEngine;

// Üstüne basan oyuncunun tutuşunu azaltır: buz gibi kayar.
public class Map5Slippery : MonoBehaviour
{
    // 1 = normal zemin, 0.04 = buz: neredeyse hiç tutuş yok
    [SerializeField] private float grip = 0.04f;

    // Algıla: bir şey bu yüzeye değmeye başladı
    void OnCollisionEnter(Collision hit)
    {
        // Kim? Sadece oyuncuda Map5PlayerMove var
        Map5PlayerMove move = hit.collider.GetComponent<Map5PlayerMove>();
        // Yap: oyuncu tutuşunu kaybeder
        if (move != null) move.SetGrip(grip);
    }

    // Algıla: artık değmiyor
    void OnCollisionExit(Collision hit)
    {
        Map5PlayerMove move = hit.collider.GetComponent<Map5PlayerMove>();
        if (move != null) move.SetGrip(1f);   // tutuş yeniden tam
    }
}
