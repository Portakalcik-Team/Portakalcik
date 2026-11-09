using UnityEngine;
public class Map2_MonsterHit : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    void OnTriggerEnter(Collider other)
    {
        // Who? only the player
        if (!other.CompareTag("Player")) return;
        // Act: report the hit - health comes in a later week
        Debug.Log(name + " hit you for " + damage + " damage!");
    }
}