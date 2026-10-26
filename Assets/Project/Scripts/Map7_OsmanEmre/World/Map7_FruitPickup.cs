using UnityEngine;

public class Map7_FruitPickup : MonoBehaviour
{
    [SerializeField] private GameObject fruit;           // Dönen meyve küresi (Boş bıraksan da kendi bulur!)
    [SerializeField] private Map7_FruitType fruitType;   // ARTIK MAP7 OLDU: Limon, Mandalina, Portakal
    [SerializeField] private int amount = 2;             
    private bool picked = false;                         

    void Start()
    {
        // Eğer Inspector'dan Sphere'i sürükleyemediysen, ağacın içindeki küreyi otomatik bulur!
        if (fruit == null)
        {
            var bob = GetComponentInChildren<Map7_FruitBob>();
            if (bob != null) fruit = bob.gameObject;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // ARTIK MAP2 DEĞİL, SENİN MAP7_FRUITBAG ÇANTANI ARIYOR!
        Map7_FruitBag bag = other.GetComponentInParent<Map7_FruitBag>();
        if (bag == null) bag = other.GetComponentInChildren<Map7_FruitBag>();
        
        if (bag == null) return;
        if (picked) return;

        // Meyveyi çantaya koy ve küreyi ağaçtan sil
        picked = true;
        if (fruit != null)
        {
            Destroy(fruit);
        }
        bag.Add(fruitType, amount);
    }
}