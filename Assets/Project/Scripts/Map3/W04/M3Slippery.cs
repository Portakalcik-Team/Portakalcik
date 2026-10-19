using UnityEngine;

// Map3 W04: buzlu zemin. Ustune basan oyuncunun tutunmasini azaltir.
public class M3Slippery : MonoBehaviour
{
    // 1 = normal zemin, 0.04 = buz
    [SerializeField] private float grip = 0.04f;

    // degmeye basladi
    void OnCollisionEnter(Collision hit)
    {
        // Kim? sadece oyuncuda M3PlayerMove var
        M3PlayerMove move = hit.collider.GetComponent<M3PlayerMove>();
        if (move != null) move.SetGrip(grip);
    }

    // degme bitti
    void OnCollisionExit(Collision hit)
    {
        M3PlayerMove move = hit.collider.GetComponent<M3PlayerMove>();
        if (move != null) move.SetGrip(1f);   // tam tutunma
    }
}
