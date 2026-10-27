using UnityEngine;

// Seviye 9: çantaya giren meyveler (portakal çantaya girmez, elde taşınır)
public enum Map9Fruit { Limon, Mandalina }

// Seviye 9: oyuncunun çantası — toplanan meyveler burada birikir
public class Map9Bag : MonoBehaviour
{
    private int limon;        // çantadaki limon
    private int mandalina;    // çantadaki mandalina

    // ağaçlar bunu çağırır
    public void Add(Map9Fruit fruit, int amount)
    {
        if (fruit == Map9Fruit.Limon) limon += amount;
        if (fruit == Map9Fruit.Mandalina) mandalina += amount;

        // ör.  +3 Limon   (Çanta: Limon 3 · Mandalina 0)
        Debug.Log("+" + amount + " " + fruit + "   (Çanta: " + Summary() + ")");
    }

    // çantada ne var? çıkış da bunu yazar
    public string Summary()
    {
        return "Limon " + limon + " · Mandalina " + mandalina;
    }
}