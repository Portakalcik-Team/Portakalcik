using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// GameOverPanel objesine ekle. Panel başta kapalı durur (tikini kaldır), tur bitince açılır.
public class M10GameOver : MonoBehaviour
{
    [SerializeField] private TMP_Text resultText;

    // M1Round çağırır
    public void Show(string message, int score)
    {
        // Bu bilgisayarda saklanır. Anahtar "m10_best": diğer haritalarla karışmasın
        int best = PlayerPrefs.GetInt("m10_best", 0);
        if (score > best) best = score;
        PlayerPrefs.SetInt("m10_best", best);

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
