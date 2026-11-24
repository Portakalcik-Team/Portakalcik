using UnityEngine;

[CreateAssetMenu(menuName = "Map3/Cargo Type")]
public class Map3_CargoType : ScriptableObject
{
    [SerializeField] private int points = 1;
    [SerializeField] private float weight;

    public int Points()
    {
        return points;
    }

    public float Weight()
    {
        return weight;
    }
}
