using UnityEngine;

public class Map2_ExitZone : MonoBehaviour
{
    private bool finished = false;   // the map ends once

    void OnTriggerEnter(Collider other)
    {
        // Who? only something that carries a fruit bag - the player
        Map2_FruitBag bag = other.GetComponent<Map2_FruitBag>();
        if (bag == null) return;
        if (finished) return;

        // Act: the map is done - oranges left in the bag count as bonus points later
        finished = true;
        Debug.Log("Map 2 complete! Oranges: " + bag.Count(Map2_FruitType.Orange));
    }
}
