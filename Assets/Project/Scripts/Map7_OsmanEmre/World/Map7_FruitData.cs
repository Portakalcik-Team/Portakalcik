using UnityEngine;

// Meyve türünün sayıları bir dosyada durur (W06: ScriptableObject).
// Map7_FD_Normal ve Map7_FD_Golden dosyaları bunu kullanır; sayıyı bir kez değiştir, o türden her ağaç değişir.
// Create menüsüne "Portakalcik > Map7 Fruit Data" ekler.
[CreateAssetMenu(menuName = "Portakalcik/Map7 Fruit Data")]
public class Map7_FruitData : ScriptableObject
{
    [SerializeField] private int points = 1;           // toplayınca verilen puan
    [SerializeField] private float weight = 0f;        // taşırken oyuncuyu yavaşlatan yük (0 = yavaşlatmaz)
    [SerializeField] private float regrowDelay = 10f;  // toplanan meyvenin yeniden yetişme süresi (sn)

    // Diğer scriptler okuyabilir, değiştirmez
    public int Points()
    {
        return points;
    }

    public float Weight()
    {
        return weight;
    }

    public float RegrowDelay()
    {
        return regrowDelay;
    }
}
