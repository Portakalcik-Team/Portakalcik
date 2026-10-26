using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    // Inspector'dan değiştirebilmen için ayarlar
    [SerializeField] private string message = "Toplandi!";
    [SerializeField] private bool removeAfter = true;

    void OnTriggerEnter(Collider other)
    {
        // Kim Geldi? Sadece "Player" etiketine sahip obje mi diye sor
        if (!other.CompareTag("Player")) return;

        // Eylem: Konsola mesaj yaz ve bu meyveyi sahneden sil
        Debug.Log(message);
        
        if (removeAfter) 
        {
            Destroy(gameObject);
        }
    }
}