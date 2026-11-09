using UnityEngine;
public class Map2_Slippery : MonoBehaviour
{
    // 1 = normal ground, 0.1 = ice: very little grip
    [SerializeField] private float grip = 0.1f;
    // Detect: something starts touching this surface
    void OnCollisionEnter(Collision hit)
    {
        // Who? only the player has Map2_PlayerMove
        Map2_PlayerMove move = hit.collider.GetComponent<Map2_PlayerMove>();
        // Act: the player loses its grip
        if (move != null) move.SetGrip(grip);
    }

    // Detect: it stops touching this surface
    void OnCollisionExit(Collision hit)
    {
        Map2_PlayerMove move = hit.collider.GetComponent<Map2_PlayerMove>();
        if (move != null) move.SetGrip(1f);
    }

}