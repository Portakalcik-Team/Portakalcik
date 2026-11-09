using UnityEngine;                                                          // Unity'nin temel sınıfları

// Meyve türü: sayılar sahnede değil dosyada durur (Create > Portakalcik > Map4 Meyve Türü)
[CreateAssetMenu(menuName = "Portakalcik/Map4 Meyve Türü")]                 // Create menüsüne yeni bir dosya türü ekler
public class M4FruitType : ScriptableObject                                 // Bir nesneye değil, dosyaya (.asset) kaydedilen veri sınıfı
{
    [SerializeField] private int points = 1;       // toplayınca taşınan puan   // Bu meyvenin puanı; tam sayı
    [SerializeField] private float weight = 0f;    // oyuncuya eklenen kütle (kg) // Bu meyvenin oyuncuya eklediği ağırlık

    // diğer scriptler okuyabilir, değiştiremez
    public int Points()                                                     // Puanı soran fonksiyon
    {
        return points;                                                      // Puan değerini döndür
    }

    public float Weight()                                                   // Ağırlığı soran fonksiyon
    {
        return weight;                                                      // Ağırlık değerini döndür
    }
}
