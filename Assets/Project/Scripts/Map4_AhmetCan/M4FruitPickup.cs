using UnityEngine;                                                          // Unity'nin temel sınıfları

public class M4FruitPickup : MonoBehaviour                                  // Meyve ağacına eklenen script; oyuncu dokununca meyve verir
{
    [SerializeField] private GameObject fruit;                              // Ağaçtaki dönen ve süzülen meyve nesnesi (toplanınca silinir)
    [SerializeField] private Map2_FruitType fruitType;                      // Meyve türü: Limon, Mandalina veya Portakal (açılır liste)
    [SerializeField] private int amount = 2;                                // Bu ağacın verdiği meyve sayısı
    private bool picked = false;                                            // Bu ağaçtan meyve alındı mı? Her ağaç bir kez verir

    void OnTriggerEnter(Collider other)                                     // Bir şey ağacın tetikleyici alanına girince çalışır
    {
        Map2_FruitBag bag = other.GetComponent<Map2_FruitBag>();            // Giren nesnede meyve çantası var mı? (sadece oyuncuda olur)
        if (bag == null) return;                                            // Çanta yoksa oyuncu değildir, çık
        if (picked) return;                                                 // Meyve zaten alındıysa tekrar verme
        picked = true;                                                      // Artık alındı olarak işaretle
        Destroy(fruit);                                                     // Ağaçtaki meyve nesnesini sahneden kaldır
        bag.Add(fruitType, amount);                                         // Meyveleri oyuncunun çantasına ekle
    }
}
