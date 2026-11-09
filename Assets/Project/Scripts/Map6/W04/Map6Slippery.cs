using UnityEngine;

// Katı buzlu yüzeylerde oyuncunun zemin tutuşunu düşürür
public class Map6Slippery : MonoBehaviour
{
    [SerializeField] private float grip = 0.04f; // Buz tutuş katsayısı

    // Yüzeye temas başladığında
    void OnCollisionEnter(Collision hit)
    {
        Map6PlayerMove move = hit.collider.GetComponent<Map6PlayerMove>();
        if (move == null) move = hit.collider.GetComponentInParent<Map6PlayerMove>();

        if (move != null) move.SetGrip(grip);
    }

    // Yüzeyden ayrıldığında
    void OnCollisionExit(Collision hit)
    {
        Map6PlayerMove move = hit.collider.GetComponent<Map6PlayerMove>();
        if (move == null) move = hit.collider.GetComponentInParent<Map6PlayerMove>();

        if (move != null) move.SetGrip(1f); // Normal tutuşa geri dön
    }
}
