using UnityEngine;

public class M10Slippery : MonoBehaviour
{
    // 1 = normal zemin, 0.04 = buz: neredeyse hiç tutunma yok
    [SerializeField] private float grip = 0.04f;

    // Bir şey bu yüzeye çarptığında
    void OnCollisionEnter(Collision hit)
    {
        // Sadece oyuncunun M10PlayerMove bileşeni varsa etki eder
        M10PlayerMove move = hit.collider.GetComponent<M10PlayerMove>();
        if (move != null) move.SetGrip(grip);
    }

    // Bu yüzeyden ayrıldığında
    void OnCollisionExit(Collision hit)
    {
        M10PlayerMove move = hit.collider.GetComponent<M10PlayerMove>();
        if (move != null) move.SetGrip(1f); // tam tutunma geri gelir
    }
}
