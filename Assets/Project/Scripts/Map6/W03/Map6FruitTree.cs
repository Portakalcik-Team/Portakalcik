using UnityEngine;

// Ağacın yanına gelindiğinde meyveyi oyuncunun çantasına ekler ve görseli kapatır
public class Map6FruitTree : MonoBehaviour
{
    [Header("Ağaç Meyve Özelliği")]
    [SerializeField] private Map6Fruit fruit = Map6Fruit.Limon;
    [SerializeField] private int amount = 3;
    [SerializeField] private GameObject visual = null;

    private bool harvested = false;

    void OnTriggerEnter(Collider other)
    {
        if (harvested) return;

        Map6Bag bag = other.GetComponent<Map6Bag>();
        if (bag == null) bag = other.GetComponentInParent<Map6Bag>();
        if (bag == null) return;

        bag.Add(fruit, amount);
        harvested = true;

        if (visual != null)
        {
            visual.SetActive(false);
        }
    }
}
