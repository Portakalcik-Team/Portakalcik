using UnityEngine;                                                          // Unity'nin temel sınıfları (PlayerPrefs de burada)
using UnityEngine.SceneManagement;                                          // Sahne yükleme (SceneManager) için gerekli
using TMPro;                                                                // TextMeshPro sınıfları (TMP_Text için)

// Bitiş ekranı: sonucu ve en iyi skoru gösterir, "Tekrar Oyna" turu yeniden başlatır.
public class M4GameOver : MonoBehaviour                                     // GameOverPanel'e eklenen script
{
    // panelin ortasındaki büyük yazı
    [SerializeField] private TMP_Text resultText;                           // ResultText nesnesi (Inspector'dan atanır)

    // en iyi skorun kayıt adı; diğer haritalarla karışmasın diye M4_
    private const string BestKey = "M4_best";                               // PlayerPrefs'teki anahtar

    // tur bitince Round çağırır
    public void Show(string message, int score)                             // İki girdi: mesaj ("Süre doldu!") ve skor
    {
        // en iyi skor bu bilgisayarda saklanır
        int best = PlayerPrefs.GetInt(BestKey, 0);                          // Kayıtlı en iyiyi oku; hiç yoksa 0
        if (score > best) best = score;                                     // Yeni rekor mu? O zaman en iyi bu skor
        PlayerPrefs.SetInt(BestKey, best);                                  // Kaydet: Stop'tan ve Unity kapanmasından sonra da kalır

        resultText.text = message + "\nSkor " + score + "\nEn iyi " + best; // Tek yazıda üç satır (\n = yeni satır)
        gameObject.SetActive(true);   // panel görünür                      // Bu nesneyi (paneli) aç

        // birinci şahısta imleç kilitli: butona tıklanabilsin diye bırak
        Cursor.lockState = CursorLockMode.None;                             // İmleç serbest
        Cursor.visible = true;                                              // İmleç görünür
    }

    // "Tekrar Oyna" butonu çağırır
    public void Restart()                                                   // public: butonun On Click () listesinde görünür
    {
        // Round zamanı durdurmuştu: önce yeniden başlat
        Time.timeScale = 1f;                                                // timeScale sahneye değil oyuna ait; yeniden yükleme sıfırlamaz
        // bulunduğumuz sahneyi tekrar yükle: taze bir tur
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);         // Map_4'ü baştan yükle; her nesne sıfırdan başlar
    }
}
