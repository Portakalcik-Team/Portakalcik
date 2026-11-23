using UnityEngine;
using TMPro;

// Hafta 7: Raunt süresini, puanı ve HUD metinlerini yönetir; süre bitince oyunu sonlandırır
public class Map6Round : MonoBehaviour
{
    [Header("Raunt Süresi Özelliği")]
    [SerializeField] private float roundLength = 90f;

    [Header("HUD Metin Özelliği")]
    [SerializeField] private TMP_Text scoreText = null;
    [SerializeField] private TMP_Text timeText = null;

    [Header("Bitiş Ekranı Özelliği")]
    [SerializeField] private Map6GameOver gameOver = null;

    private int score;
    private float timeLeft;
    private bool over;

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

        if (timeLeft <= 0f)
        {
            EndRound("Time's up!");
        }
    }

    public void AddPoints(int points)
    {
        if (over) return;

        score += points;
        ShowHud();
    }

    void EndRound(string message)
    {
        over = true;
        Time.timeScale = 0f;
        if (gameOver != null)
        {
            gameOver.Show(message, score);
        }
    }

    void ShowHud()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score " + score;
        }

        if (timeText != null)
        {
            int secs = Mathf.CeilToInt(timeLeft);
            timeText.text = (secs / 60) + ":" + (secs % 60).ToString("00");
        }
    }
}
