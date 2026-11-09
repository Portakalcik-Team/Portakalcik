using UnityEngine;

// Seviye 9: oyuncu ağacın yanına gelince meyveler çantaya girer
public class Map9FruitTree : MonoBehaviour
{
    [SerializeField] private Map9Fruit fruit = Map9Fruit.Limon;
    [SerializeField] private int amount = 3;       // Sv.9: Limon 3 · Mandalina 14
    [SerializeField] private GameObject visual;    // ağaçtaki dönen meyve küpü

    private bool harvested;                        // true = ağaç boşaldı

    void OnTriggerEnter(Collider other)
    {
        // Who? sadece çantası olan: oyuncu
        Map9Bag bag = other.GetComponent<Map9Bag>();
        if (bag == null) return;                   // oyuncu değil
        if (harvested) return;                     // ağaç zaten boş

        // Act: meyveleri çantaya koy, ağaç boş görünsün
        bag.Add(fruit, amount);
        harvested = true;
        visual.SetActive(false);
    }
}