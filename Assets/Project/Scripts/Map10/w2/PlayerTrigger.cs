using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    // Inspector'dan her obje için özel olarak ayarlayabileceğin kısımlar
    [SerializeField] private string message = "Level Completed!";
    [SerializeField] private bool removeAfter = true;

    // Bir objenin (Player'ın) bu objenin Trigger alanına girdiği an çalışan fonksiyon
    void OnTriggerEnter(Collider other)
    {
        // Giren kişi "Player" etiketine sahip değilse hiçbir şey yapma (geri dön)
        if (!other.CompareTag("Player")) return;

        // Giren kişi Player ise, belirlediğimiz mesajı konsola yazdır
        Debug.Log(message);

        // Eğer removeAfter kutucuğu işaretliyse, temas edilen bu objeyi (örn: Limon) sahneden sil
        if (removeAfter) Destroy(gameObject);
    }
}