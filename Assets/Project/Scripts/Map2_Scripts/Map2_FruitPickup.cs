using UnityEngine;
public class Map2_FruitPickup : MonoBehaviour
{
    [SerializeField] private GameObject fruit;           // the Fruit child that spins and floats
    [SerializeField] private Map2_FruitType fruitType;   // dropdown: Lemon, Tangerine or Orange
    [SerializeField] private int amount = 2;             // how many this tree gives (balance sheet)
    private bool picked = false;                         // each tree gives its fruit once
    void OnTriggerEnter(Collider other)
    {
        // Who? only something that carries a fruit bag - the player
        Map2_FruitBag bag = other.GetComponent<Map2_FruitBag>();
        if (bag == null) return;
        if (picked) return;
        // Act: put the fruit into the bag and remove it from the tree
        picked = true;
        Destroy(fruit);
        bag.Add(fruitType, amount);
    }
}
