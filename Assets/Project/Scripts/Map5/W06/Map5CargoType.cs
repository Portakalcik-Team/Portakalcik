using UnityEngine;

// W06: Kargo türü (ScriptableObject). Sahnede bir objenin üstünde değil, Project'te dosya (.asset) olarak durur.
// Create menüsüne Map5 > Cargo Type ekler
[CreateAssetMenu(menuName = "Map5/Cargo Type")]
public class Map5CargoType : ScriptableObject
{
    [SerializeField] private int points = 1;     // teslim edilince kaç puan
    [SerializeField] private float weight = 0f;  // oyuncuya eklenen kütle (kg)

    // Diğer scriptler okuyabilir, ama asla değiştiremez
    public int Points()
    {
        return points;
    }

    public float Weight()
    {
        return weight;
    }
}
