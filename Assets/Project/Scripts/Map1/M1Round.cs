using UnityEngine;
using TMPro;

// BOŞ bir objeye ekle ("M1Round" adında). Tur süresi, skor, ekrandaki yazılar, bitiş.
// Skoru artıran herkes M1Round.Instance.AddPoints(n) çağırır. Bitirmek için EndRound("mesaj").
public class M1Round : MonoBehaviour
{
    public static M1Round Instance;

    [SerializeField] private float roundLength = 90f;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private M1GameOver gameOver;

    private int score;
    private float timeLeft;
    private bool over;

    public bool Over { get { return over; } }
    public float TimeLeft { get { return timeLeft; } }

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;          // önceki tur zamanı durdurduysa tekrar başlat
    }

    void Start()
    {
        timeLeft = roundLength;
        ShowHud();
    }

    void Update()
    {
        if (over) return;
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        ShowHud();
        if (timeLeft <= 0f) EndRound("Sure doldu!");
    }

    public void AddPoints(int points)
    {
        if (over) return;
        score += points;
    }

    // Süre bitince, ölünce ya da çıkışa girince çağrılır
    public void EndRound(string message)
    {
        if (over) return;
        over = true;
        M1PlayerTrigger.Stopped = true;     // hareket, kamera, düşman ve fırlatma durur
        Time.timeScale = 0f;                // dünya durur
        Cursor.lockState = CursorLockMode.None;   // buton tıklanabilsin
        Cursor.visible = true;
        if (gameOver != null) gameOver.Show(message, score);
    }

    void ShowHud()
    {
        if (scoreText != null) scoreText.text = "Skor " + score;
        int seconds = Mathf.CeilToInt(timeLeft);
        int minutes = seconds / 60;
        if (timeText != null) timeText.text = minutes + ":" + (seconds % 60).ToString("00");
    }
}
