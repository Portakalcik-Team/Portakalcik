using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// GameOverPanel objesine ekle. Panel başta kapalı durur (tikini kaldır), tur bitince açılır.
public class M1GameOver : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    // M1Round çağırır
    public void Show(string message, int score)
    {
        // Bu bilgisayarda saklanır. Anahtar "m1_best": diğer haritalarla karışmasın
        int best = PlayerPrefs.GetInt("m1_best", 0);
        if (score > best) best = score;
        PlayerPrefs.SetInt("m1_best", best);

        resultText.text = message + "\nSkor " + score + "\nEn iyi " + best;
        gameObject.SetActive(true);
    }

    // Tekrar oyna butonu çağırır
    public void Restart()
    {
        Time.timeScale = 1f;      // M1Round zamanı durdurmuştu, önce başlat
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
