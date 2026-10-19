using UnityEngine;

public class Map7_FruitPickup : MonoBehaviour
{
    [SerializeField] private GameObject fruit;           // Dönen meyve küresi (Boş bıraksan da kendi bulur!)
    [SerializeField] private Map7_FruitType fruitType;   // ARTIK MAP7 OLDU: Limon, Mandalina, Portakal
    [SerializeField] private int amount = 2;             
    private bool picked = false;                         
    private GameObject treeFruits;                       // Map7_TreeLook'un kurduğu görsel meyveler (varsa)

    void Start()
    {
        // Ağacın gövde (Trunk) alt objesinde de aynı script var; çift toplamayı önlemek için
        // üstte zaten bir Map7_FruitPickup varsa bu kopya kendini kapatır.
        if (transform.parent != null && transform.parent.GetComponentInParent<Map7_FruitPickup>() != null)
        {
            enabled = false;
            return;
        }

        // Eğer Inspector'dan Sphere'i sürükleyemediysen, ağacın içindeki küreyi otomatik bulur!
        if (fruit == null)
        {
            var bob = GetComponentInChildren<Map7_FruitBob>();
            if (bob != null) fruit = bob.gameObject;
        }

        // Yeni ağaç görünümündeki meyve grubunu bul
        Transform fruits = transform.Find("Visual/Crown/Fruits");
        if (fruits != null) treeFruits = fruits.gameObject;
    }

    void OnTriggerEnter(Collider other)
    {
        // Sadece oyuncu toplayabilir. (Eskiden başka ağaçlar da çantası olduğu için ağacın meyvesini
        // oyun başında kendi kendine toplayıp kaybolmasına neden olabiliyordu.)
        if (!other.CompareTag("Player")) return;

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
        if (treeFruits != null)
        {
            Destroy(treeFruits);
        }
        bag.Add(fruitType, amount);
    }
}