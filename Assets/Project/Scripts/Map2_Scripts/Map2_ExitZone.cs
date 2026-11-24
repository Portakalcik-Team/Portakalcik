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

        // Act: the map is done - show the score and the oranges left in the bag
        finished = true;
        Debug.Log("Map 2 complete! Score: " + bag.Score() + "   Oranges: " + bag.Count(Map2_FruitType.Orange));
    }
}
