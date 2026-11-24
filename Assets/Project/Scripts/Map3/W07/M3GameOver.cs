using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Map3 W07: oyun sonu paneli. Skoru ve en iyi skoru gosterir, Tekrar Oyna ile sahneyi bastan yukler.
public class M3GameOver : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    // diger haritalarla karismasin diye Map3'e ozel anahtar
    private const string BestKey = "Map3_best";

    public void Show(string message, int score)
    {
        int best = PlayerPrefs.GetInt(BestKey, 0);
        if (score > best) best = score;
        PlayerPrefs.SetInt(BestKey, best);

        resultText.text = message + "\nSkor " + score + "\nEn iyi " + best;
        gameObject.SetActive(true);
    }

    // PlayAgainButton OnClick
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
