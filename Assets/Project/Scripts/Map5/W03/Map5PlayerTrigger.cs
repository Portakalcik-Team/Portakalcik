using UnityEngine;

// Oyuncu bu nesnenin içine girince bir mesaj yazar, istenirse nesneyi siler.
// Meyvede, tehlikede ve çıkışta aynı script; sadece Inspector ayarı değişir.
public class Map5PlayerTrigger : MonoBehaviour
{
    [SerializeField] private string message = "Toplandı!";   // Console'a yazılacak mesaj
    [SerializeField] private bool removeAfter = true;        // Mesajdan sonra yok olsun mu?

    // Bir şey bu nesnenin tetik alanına girince çalışır
    void OnTriggerEnter(Collider other)
    {
        // Kim? Sadece "Player" etiketli nesne
        if (!other.CompareTag("Player")) return;

        // Yap: mesajı yaz, istenirse kendini sil
        Debug.Log(message);
        if (removeAfter) Destroy(gameObject);
    }
}
