using UnityEngine;

public class M10andalinaSpawner : MonoBehaviour
{
    // Slayttaki 'Cargo' yerine senin Mandalina Prefab'ını tutacak değişken.
    // [SerializeField] sayesinde bu değişkeni Unity Inspector panelinde göreceğiz.
    [SerializeField] private GameObject mandalinaPrefab; 
    
    // Slayttaki 'Pad' yerine mandalinanın nerede doğacağını belirleyen nokta.
    [SerializeField] private Transform spawnNoktasi; 

    // Kaç saniyede bir mandalina üretecek?
    [SerializeField] private float spawnSuresi = 3f;
    private float zamanlayici = 0f;

    void Update()
    {
        // Time.deltaTime ekleyerek süreyi saniye cinsinden tutuyoruz
        zamanlayici += Time.deltaTime;

        if (zamanlayici >= spawnSuresi)
        {
            MandalinaUret();
            zamanlayici = 0f; // Zamanlayıcıyı sıfırla ki tekrar saysın
        }
    }

    void MandalinaUret()
    {
        // Instantiate fonksiyonu kopyalamaya (spawn etmeye) yarar.
        // Neyi kopyalayacak? -> mandalinaPrefab
        // Nereye kopyalayacak? -> spawnNoktasi.position (MandalinaPad'in koordinatları)
        // Hangi açıyla? -> spawnNoktasi.rotation (MandalinaPad'in dönüş açısı)
        Instantiate(mandalinaPrefab, spawnNoktasi.position, spawnNoktasi.rotation);
    }
}