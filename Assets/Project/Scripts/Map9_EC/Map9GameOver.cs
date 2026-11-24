using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Seviye 9: bitiş paneli — sonuç, en iyi skor ve "Tekrar oyna" (Hafta 7)
public class Map9GameOver : MonoBehaviour
{
    // panelin ortasındaki büyük yazı
    [SerializeField] private TMP_Text resultText;

    // en iyi skor bu isimle saklanır — diğer haritaların skoruyla karışmasın
    private const string BestKey = "Map9_best";

    // tur bitince Round bunu çağırır
    public void Show(string message, int score)
    {
        // en iyi skor bu bilgisayarda saklanır
        int best = PlayerPrefs.GetInt(BestKey, 0);
        if (score > best) best = score;
        PlayerPrefs.SetInt(BestKey, best);

        resultText.text = message + "\nSkor " + score + "\nEn iyi " + best;
        gameObject.SetActive(true);   // panel görünür

        // birinci şahıs: fare kilitli — butona tıklanabilsin diye serbest bırak
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // "Tekrar oyna" butonu bunu çağırır
    public void Restart()
    {
        // Round zamanı durdurdu: önce yeniden başlat
        Time.timeScale = 1f;
        // içinde olduğumuz sahneyi tekrar yükle: yepyeni bir tur
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
