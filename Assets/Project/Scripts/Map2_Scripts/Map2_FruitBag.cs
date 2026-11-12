using UnityEngine;
public enum Map2_FruitType { Lemon, Tangerine, Orange }   // dropdown in the Inspector
public class Map2_FruitBag : MonoBehaviour
{
    // shown in the Inspector, so you can watch them grow in Play mode
    [SerializeField] private int lemons = 0;       // 1 lemon = 1 question (hint)
    [SerializeField] private int tangerines = 0;   // ammo for throw
    [SerializeField] private int oranges = 0;      // 1 orange opens the exit
    [SerializeField] private int score = 0;        // points picked so far - 0 at every Play

    // Map 2 trees: the fruit's data file says what, how many and how many points
    public void Add(Map2_FruitData data)
    {
        score += data.Points();
        Add(data.Type(), data.Amount());
        Debug.Log("+" + data.Points() + " points   Score: " + score);
    }

    // also used by other maps (Map 4), keep it as it is
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

    public int Score()
    {
        return score;
    }
}
