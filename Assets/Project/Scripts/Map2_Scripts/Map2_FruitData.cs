using UnityEngine;

// adds Portakalcik > Map 2 > Fruit Data to the Create menu
[CreateAssetMenu(menuName = "Portakalcik/Map 2/Fruit Data")]
public class Map2_FruitData : ScriptableObject
{
    [SerializeField] private Map2_FruitType type;           // which counter in the bag goes up
    [SerializeField] private int amount = 1;                // how many go into the bag per pick
    [SerializeField] private int points = 1;                // score per pick
    [SerializeField] private float regrowTime = 0f;         // seconds until the tree grows a new one; 0 = only once

    // other scripts may read them, never change them
    public Map2_FruitType Type()
    {
        return type;
    }

    public int Amount()
    {
        return amount;
    }

    public int Points()
    {
        return points;
    }

    public float RegrowTime()
    {
        return regrowTime;
    }
}
