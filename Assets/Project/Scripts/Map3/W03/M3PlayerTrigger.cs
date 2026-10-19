using UnityEngine;

// Map_3: W03 slayttaki PlayerTrigger (Types B ve C) yaklasimi.
// Tek script, uc is: normal mandalina, kirmizi tehlike ve yesil hedef.
// Sadece mesaj ve removeAfter degisir. Oyuncuya degil, etkilesim nesnesine eklenir.
public class M3PlayerTrigger : MonoBehaviour
{
    // ikisi de her nesne icin Inspector'dan ayarlanir
    [SerializeField] private string message = "Mandalina toplandı!";
    [SerializeField] private bool removeAfter = true;

    // ayni fizik adiminda birden fazla oyuncu collider'i girerse mesaj bir kez yazilsin
    private float lastTriggerTime = -1f;
    private bool removed;

    void OnTriggerEnter(Collider other)
    {
        // Who? sadece Player tag'li oyuncu (collider alt nesnede olsa bile kok nesneye bak)
        Transform player = other.attachedRigidbody != null ? other.attachedRigidbody.transform : other.transform;
        if (!player.CompareTag("Player")) return;
        if (removed) return;                              // bir kez toplandi
        if (Mathf.Approximately(lastTriggerTime, Time.fixedTime)) return;
        lastTriggerTime = Time.fixedTime;

        // Act: mesaji yaz, istenirse nesneyi collider'lariyla birlikte kaldir
        Debug.Log(message, this);   // Console'da mesaja tiklayinca nesne Hierarchy'de isaretlenir

        if (removeAfter)
        {
            removed = true;
            gameObject.SetActive(false);   // ayni karede ikinci kez toplanmasin
            Destroy(gameObject);
        }
    }
}
