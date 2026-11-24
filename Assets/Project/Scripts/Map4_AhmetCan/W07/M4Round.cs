using UnityEngine;                                                          // Unity'nin temel sınıfları
using TMPro;                                                                // TextMeshPro sınıfları (TMP_Text için)

// Bir tur: skoru tutar, saati geri sayar, ikisini ekrana (HUD) yazar, süre bitince turu bitirir.
public class M4Round : MonoBehaviour                                        // Sahnedeki boş "Round" nesnesine eklenen script
{
    // bir turun süresi ve HUD'daki iki yazı
    [SerializeField] private float roundLength = 90f;                       // Tur süresi (saniye); test için 10 yapılabilir
    [SerializeField] private TMP_Text scoreText;                            // Sol üstteki skor yazısı (ScoreText)
    [SerializeField] private TMP_Text timeText;                             // Üst ortadaki süre yazısı (TimeText)
    // bitiş ekranı
    [SerializeField] private M4GameOver gameOver;                           // GameOverPanel'deki M4GameOver scripti

    private int score;        // bu turdaki puan                            // Teslim edilen toplam puan; her turda 0'dan başlar
    private float timeLeft;   // bitişe kalan saniye                        // Geri sayan saat
    private bool over;        // tur bitti mi?                              // false = oynanıyor, true = tur bitti

    void Start()                                                            // Oyun başlayınca bir kez çalışır
    {
        timeLeft = roundLength;   // saat dolu başlar                       // Saati tur süresiyle doldur
    }

    void Update()                                                           // Her karede çalışır
    {
        if (over) return;   // bittiyse hiçbir şey değişmez                 // Tur bittiyse saat 0:00'da kalır

        // geri say, ama sıfırın altına inme
        timeLeft -= Time.deltaTime;                                         // Son kareden beri geçen süreyi düş
        if (timeLeft < 0f) timeLeft = 0f;                                   // Eksiye düşerse tam 0 yap
        ShowHud();                                                          // İki sayıyı da ekrana yaz
        if (timeLeft <= 0f) EndRound("Süre doldu!");                        // Saat sıfırlandıysa turu bitir
    }

    // çıkış bölgesi teslim edilen puanı buraya verir
    public void AddPoints(int points)                                       // Diğer scriptler çağırabilir (public)
    {
        if (over) return;                                                   // Tur bittikten sonra puan yok
        score += points;                                                    // Puanı skora ekle; HUD bir sonraki karede gösterir
    }

    // süre doldu ya da başka bir script turu bitiriyor
    public void EndRound(string message)                                    // Saat çağırır; ileride başka şeyler de çağırabilir
    {
        if (over) return;                                                   // Zaten bittiyse ikinci kez bitirme
        over = true;                                                        // Turun bittiğini hatırla
        Time.timeScale = 0f;   // dünyayı durdur                            // Bütün oyun durur: fizik, hareket, saat
        gameOver.Show(message, score);                                      // Mesajı ve skoru bitiş ekranına ver
    }

    // iki sayıyı ekrana yaz
    void ShowHud()                                                          // Sadece Round kendisi çağırır
    {
        scoreText.text = "Skor " + score;                                   // Yazı + sayı: "Skor 4"
        int seconds = Mathf.CeilToInt(timeLeft);                            // Yukarı yuvarla: 89.4 → 90
        int minutes = seconds / 60;                                         // Tam dakika: 90 / 60 = 1
        timeText.text = minutes + ":" + (seconds % 60).ToString("00");      // dakika:saniye, iki haneli → "1:30"
    }
}
