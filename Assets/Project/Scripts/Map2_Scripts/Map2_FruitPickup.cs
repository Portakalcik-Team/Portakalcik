using UnityEngine;
public class Map2_FruitPickup : MonoBehaviour
{
    // Stay, not Enter: fruit regrows, and the player may already stand under the tree when it does
    void OnTriggerStay(Collider other)
    {
        // Who? only something that carries a fruit bag - the player
        Map2_FruitBag bag = other.GetComponent<Map2_FruitBag>();
        if (bag == null) return;

        // does a fruit hang on this tree right now? (the spawner grows a new one later)
        Map2_Fruit fruit = GetComponentInChildren<Map2_Fruit>();
        if (fruit == null) return;

        // Act: put the fruit into the bag, then remove it from the tree
        bag.Add(fruit.Data());
        Destroy(fruit.gameObject);
    }
}
