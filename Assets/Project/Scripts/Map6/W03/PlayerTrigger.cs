using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    // Inspector üzerinden her nesne için ayrı mesaj ve silinme kuralı belirlenir
    [SerializeField] private string message = "Collected!";
    [SerializeField] private bool removeAfter = true;

    void OnTriggerEnter(Collider other)
    {
        // Yalnızca "Player" etiketine sahip nesne girdiğinde çalışır
        if (!other.CompareTag("Player")) return;

        // Konsola bildirim yazdırır
        Debug.Log(message);

        // Eğer removeAfter işaretliyse nesneyi sahneden siler (toplanabilir eşyalar için)
        if (removeAfter)
        {
            Destroy(gameObject);
        }
    }
}