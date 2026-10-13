using UnityEngine;
public class Map2_ExitZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Who? only something that carries a fruit bag - the player
        Map2_FruitBag bag = other.GetComponent<Map2_FruitBag>();
        if (bag == null) return;
        // the balance sheet: the exit needs 1 orange
        if (bag.Count(Map2_FruitType.Orange) < 1)
        {
            Debug.Log("You need an Orange to open the exit!");
            return;
        }
        // Act: the map is done - loading the next map comes later
        Debug.Log("Map 2 complete!");
    }
}
