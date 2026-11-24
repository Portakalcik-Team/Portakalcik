using UnityEngine;
using TMPro;

// Map3 W07: turun skoru ve saati. HUD'u gunceller, sure bitince turu kapatir.
public class M3Round : MonoBehaviour
{
    [SerializeField] private float roundLength = 90f;   // tur suresi (sn)
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private M3GameOver gameOver;

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

        // saat geri sayar
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        ShowHud();

        if (timeLeft <= 0f) EndRound("Süre bitti!");
    }

    // teslim alani cagirir
    public void AddPoints(int points)
    {
        score += points;
        ShowHud();
    }

    public int Score()
    {
        return score;
    }

    public void EndRound(string message)
    {
        if (over) return;      // tur bir kez biter
        over = true;
        Time.timeScale = 0f;   // oyunu dondur
        gameOver.Show(message, score);
    }

    void ShowHud()
    {
        scoreText.text = "Skor " + score;

        int seconds = Mathf.CeilToInt(timeLeft);
        int minutes = seconds / 60;
        timeText.text = minutes + ":" + (seconds % 60).ToString("00");
    }
}
