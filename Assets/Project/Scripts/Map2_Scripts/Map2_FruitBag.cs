using UnityEngine;
public enum Map2_FruitType { Lemon, Tangerine, Orange }   // dropdown in the Inspector
public class Map2_FruitBag : MonoBehaviour
{
    // shown in the Inspector, so you can watch them grow in Play mode
    [SerializeField] private int lemons = 0;       // 1 lemon = 1 question (hint)
    [SerializeField] private int tangerines = 0;   // ammo for throw (Week 5)
    [SerializeField] private int oranges = 0;      // 1 orange opens the exit
    public void Add(Map2_FruitType type, int amount)
    {
        if (type == Map2_FruitType.Lemon) lemons += amount;
        else if (type == Map2_FruitType.Tangerine) tangerines += amount;
        else oranges += amount;
        Debug.Log("+" + amount + " " + type);
        Debug.Log("Bag: " + lemons + " lemon / " + tangerines + " tangerine / " + oranges + " orange");
    }
    public int Count(Map2_FruitType type)
    {
        if (type == Map2_FruitType.Lemon) return lemons;
        if (type == Map2_FruitType.Tangerine) return tangerines;
        return oranges;
    }
}
