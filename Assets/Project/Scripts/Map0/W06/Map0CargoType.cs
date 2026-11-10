using UnityEngine;

// kargo çeşidi: puan ve ağırlık verisi
[CreateAssetMenu(menuName = "Map0/Cargo Type")]
public class Map0CargoType : ScriptableObject
{
    [SerializeField] private int points = 1;    // teslimde puan
    [SerializeField] private float weight = 0f; // oyuncuya ek kütle, kg

    // diğer scriptler okur, değiştiremez
    public int Points() { return points; }

    public float Weight() { return weight; }
}
