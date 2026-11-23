using UnityEngine;
public class Map2_FruitPickup : MonoBehaviour
{
    // the Round keeps the score now
    [SerializeField] private Map2_Round round;

    void Start()
    {
        if (round == null) Debug.LogError(name + ": the Round slot is empty - run Tools > Map 2 > W07 Setup");
    }

    // Stay, not Enter: fruit regrows, and the player may already stand under the tree when it does
    void OnTriggerStay(Collider other)
    {
        if (round == null) return;   // not wired: pay nothing (Start already said so)

        // Who? only something that carries a fruit bag - the player
        Map2_FruitBag bag = other.GetComponent<Map2_FruitBag>();
        if (bag == null) return;

        // does a fruit hang on this tree right now? (the spawner grows a new one later)
        Map2_Fruit fruit = GetComponentInChildren<Map2_Fruit>();
        if (fruit == null) return;

        // Act: the fruit into the bag, its points to the Round, then remove it from the tree
        bag.Add(fruit.Data());
        round.AddPoints(fruit.Data().Points());
        Destroy(fruit.gameObject);
    }
}
