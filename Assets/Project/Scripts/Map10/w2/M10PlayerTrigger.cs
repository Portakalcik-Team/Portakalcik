using UnityEngine;

public class M10PlayerTrigger : MonoBehaviour
{
    // Inspector'dan her obje için özel olarak ayarlanabilir
    [SerializeField] private string message = "Level Completed!";
    [SerializeField] private bool removeAfter = true;

    // Bir objenin (Player'ın) bu objenin Trigger alanına girdiği an çalışır
    void OnTriggerEnter(Collider other)
    {
        // Giren kişi "Player" etiketine sahip değilse geri dön
        if (!other.CompareTag("Player")) return;

        // Belirlenen mesajı konsola yazdır
        Debug.Log(message);

        // Eğer removeAfter işaretliyse objeyi sahneden sil
        if (removeAfter) Destroy(gameObject);
    }
}
