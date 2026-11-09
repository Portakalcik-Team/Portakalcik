using UnityEngine;

public class Map0Slippery : MonoBehaviour
{
    // 1 = normal zemin, 0.04 = buz: neredeyse hiç tutunma yok
    [SerializeField] private float grip = 0.04f;

    // bir şey bu yüzeye değmeye başlayınca
    void OnCollisionEnter(Collision hit)
    {
        // Kim? sadece oyuncuda Map0PlayerMove var
        Map0PlayerMove move = hit.collider.GetComponent<Map0PlayerMove>();
        if (move != null) move.SetGrip(grip);
    }

    // değme bitince
    void OnCollisionExit(Collision hit)
    {
        Map0PlayerMove move = hit.collider.GetComponent<Map0PlayerMove>();
        if (move != null) move.SetGrip(1f); // tam tutunma geri gelir
    }
}
