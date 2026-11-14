using UnityEngine;                                                          // Unity'nin temel sınıflarını (MonoBehaviour, Mathf, Vector3...) kullanmamızı sağlar

public class M4Bobber : MonoBehaviour                                       // Nesneyi yukarı aşağı süzdüren script; bir nesneye eklenebilir (MonoBehaviour)
{
    [SerializeField] private float bobHeight = 0.25f;                       // Süzülme yüksekliği (metre); Inspector'da görünür ve değiştirilebilir
    [SerializeField] private float bobSpeed = 2f;                           // Süzülme hızı; büyüdükçe daha hızlı sallanır
    private Vector3 startLocalPosition;                                     // Nesnenin başlangıçtaki yerel konumu; süzülmenin merkezi

    void Start()                                                            // Oyun başlayınca bir kez çalışır
    {
        startLocalPosition = transform.localPosition;                       // Başlangıç konumunu hatırla
    }

    void Update()                                                           // Her karede çalışır
    {
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;         // Sinüs dalgası: zamanla -bobHeight ile +bobHeight arasında gidip gelen değer
        transform.localPosition = startLocalPosition + Vector3.up * offset; // Başlangıç konumunun üstüne yukarı/aşağı kaydırma ekle
    }
}
