using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Map0GameOver : MonoBehaviour
{
    const string BestKey = "Map0_best"; // Map0'a özel en iyi skor anahtarı

    [SerializeField] private TMP_Text resultText; // sonuç ekranındaki yazı

    // Sonuç ekranını doldur ve göster
    public void Show(string message, int score)
    {
        int best = PlayerPrefs.GetInt(BestKey, 0); // kayıtlı en iyi skor
        if (score > best) best = score;
        PlayerPrefs.SetInt(BestKey, best);
        resultText.text = message + "\nSkor " + score + "\nEn iyi " + best;
        gameObject.SetActive(true);
    }

    // Sahneyi baştan yükle
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
