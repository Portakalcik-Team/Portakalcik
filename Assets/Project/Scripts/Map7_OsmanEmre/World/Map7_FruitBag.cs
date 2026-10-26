using UnityEngine;

public enum Map7_FruitType { Limon, Mandalina, Portakal }   // Inspector'daki açılır menü

public class Map7_FruitBag : MonoBehaviour
{
    // Play modunda sayılarının artışını Inspector'dan izleyebilirsin
    [SerializeField] private int limon = 0;       // 1 limon = 1 ipucu/soru
    [SerializeField] private int mandalina = 0;   // Fırlatma mermisi (Hafta 5)
    [SerializeField] private int portakal = 0;    // 1 portakal çıkış kapısını açar

    public void Add(Map7_FruitType type, int amount)
    {
        if (type == Map7_FruitType.Limon) limon += amount;
        else if (type == Map7_FruitType.Mandalina) mandalina += amount;
        else portakal += amount;

        Debug.Log("+" + amount + " " + type);
        Debug.Log("Bag: " + limon + " limon / " + mandalina + " mandalina / " + portakal + " portakal");
    }

    public int Count(Map7_FruitType type)
    {
        if (type == Map7_FruitType.Limon) return limon;
        if (type == Map7_FruitType.Mandalina) return mandalina;
        return portakal;
    }

    public bool Use(Map7_FruitType type, int amount = 1)
    {
        if (Count(type) < amount) return false;

        if (type == Map7_FruitType.Limon) limon -= amount;
        else if (type == Map7_FruitType.Mandalina) mandalina -= amount;
        else portakal -= amount;

        return true;
    }
}