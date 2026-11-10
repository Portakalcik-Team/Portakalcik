using UnityEngine;

// Seviye 9: portakal türü — puan ve ağırlık burada, kodda değil
[CreateAssetMenu(menuName = "Portakalcik/Map9 Portakal Type")]
public class Map9PortakalType : ScriptableObject
{
    [SerializeField] private int points = 1;      // teslimde kaç puan
    [SerializeField] private float weight = 0f;   // oyuncuya eklenen kg

    // sadece okunur: oyun içinden değiştirilemez
    public int Points()
    {
        return points;
    }

    public float Weight()
    {
        return weight;
    }
}
