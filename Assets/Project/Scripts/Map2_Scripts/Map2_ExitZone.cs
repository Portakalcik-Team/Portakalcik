using UnityEngine;

public class Map2_ExitZone : MonoBehaviour
{
    // the Round ends the map and pays the time bonus
    [SerializeField] private Map2_Round round;

    void Start()
    {
        if (round == null) Debug.LogError(name + ": the Round slot is empty - run Tools > Map 2 > W07 Setup");
    }

    void OnTriggerEnter(Collider other)
    {
        if (round == null) return;

        // Who? only something that carries a fruit bag - the player
        if (other.GetComponent<Map2_FruitBag>() == null) return;

        // Act: the map is done (the Round makes sure it ends only once)
        round.ExitReached();
    }
}
