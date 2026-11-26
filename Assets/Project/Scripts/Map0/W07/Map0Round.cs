using UnityEngine;
using TMPro;

public class Map0Round : MonoBehaviour
{
    [SerializeField] private float roundLength = 90f; // tur süresi (saniye)
    [SerializeField] private TMP_Text scoreText; // ekrandaki skor yazısı
    [SerializeField] private TMP_Text timeText; // ekrandaki süre yazısı
    [SerializeField] private Map0GameOver gameOver; // tur bitince açılacak ekran

    private int score; // şimdiye kadarki puan
    private float timeLeft; // kalan süre
    private bool over; // tur bitti mi

    // Turu başlat: süreyi kur ve HUD'u göster
    void Start()
    {
        timeLeft = roundLength;
        ShowHud();
    }

    // Her karede süreyi azalt ve HUD'u güncelle
    void Update()
    {
        if (over) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        ShowHud();
        if (timeLeft <= 0f) EndRound("Süre doldu!");
    }

    // Puan ekle (tur bittiyse yok say)
    public void AddPoints(int points)
    {
        if (over) return;
        score += points;
    }

    // Turu bitir: oyunu durdur ve sonuç ekranını aç
    public void EndRound(string message)
    {
        if (over) return;
        over = true;
        Time.timeScale = 0f;
        gameOver.Show(message, score);
    }

    // Skor ve süre yazılarını güncelle
    void ShowHud()
    {
        scoreText.text = "Skor " + score;
        int seconds = Mathf.CeilToInt(timeLeft);
        int minutes = seconds / 60;
        timeText.text = minutes + ":" + (seconds % 60).ToString("00");
    }
}
