using System.Collections.Generic;
using UnityEngine;

// Map 6 meyve türleri (Limon ve Mandalina)
public enum Map6Fruit
{
    Limon,
    Mandalina
}

// Oyuncunun topladığı meyveleri saklayan çanta bileşeni
public class Map6Bag : MonoBehaviour
{
    private readonly Dictionary<Map6Fruit, int> counts = new Dictionary<Map6Fruit, int>();

    public void Add(Map6Fruit fruit, int amount)
    {
        if (amount <= 0) return;

        if (!counts.ContainsKey(fruit))
        {
            counts[fruit] = 0;
        }

        counts[fruit] += amount;
        Debug.Log("Toplandı: +" + amount + " " + fruit + " | " + Summary());
    }

    public int GetCount(Map6Fruit fruit)
    {
        return counts.TryGetValue(fruit, out int count) ? count : 0;
    }

    public string Summary()
    {
        return "Çanta -> Limon: " + GetCount(Map6Fruit.Limon) + ", Mandalina: " + GetCount(Map6Fruit.Mandalina);
    }
}
