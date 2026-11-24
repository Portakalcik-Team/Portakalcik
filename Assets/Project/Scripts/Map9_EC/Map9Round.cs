using UnityEngine;
using TMPro;

// Seviye 9: bir tur — skor, geri sayım ve bitiş (Hafta 7)
public class Map9Round : MonoBehaviour
{
    // tur süresi, hedef skor ve iki HUD yazısı
    [SerializeField] private float roundLength = 90f;
    [SerializeField] private int targetScore = 5;      // denge dokümanı
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    // bitiş ekranı
    [SerializeField] private Map9GameOver gameOver;

    private int score;        // bu turdaki puan
    private float timeLeft;   // bitişe kalan saniye
    private bool over;        // tur bitince true

    void Start()
    {
        timeLeft = roundLength;   // saat dolu başlar
    }

    void Update()
    {
        if (over) return;   // bitti: artık hiçbir şey değişmez

        // geri say, ama sıfırın altına inme
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        ShowHud();

        // süre bitti: hedefe ulaştıysa kazandı, ulaşmadıysa kaybetti
        if (timeLeft <= 0f)
        {
            if (score >= targetScore) EndRound("Seviye tamamlandı!");
            else EndRound("Süre doldu!");
        }
    }

    // çıkış bunu çağırır
    public void AddPoints(int points)
    {
        if (over) return;
        score += points;
    }

    // süre bitti — ya da başka bir script turu bitiriyor
    public void EndRound(string message)
    {
        if (over) return;
        over = true;
        Time.timeScale = 0f;   // dünyayı durdur
        gameOver.Show(message, score);
    }

    // iki sayıyı ekrana yaz
    void ShowHud()
    {
        scoreText.text = "Skor " + score + "/" + targetScore;
        int seconds = Mathf.CeilToInt(timeLeft);
        int minutes = seconds / 60;
        timeText.text = minutes + ":" + (seconds % 60).ToString("00");
    }
}
