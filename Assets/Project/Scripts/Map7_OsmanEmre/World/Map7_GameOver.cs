using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Map 7 bitiş paneli (W07): sonuç mesajı, en iyi skor (PlayerPrefs) ve "Tekrar oyna" butonu.
public class Map7_GameOver : MonoBehaviour
{
    // Panelin ortasındaki büyük sonuç yazısı
    [SerializeField] private TMP_Text resultText;

    // Diğer haritaların skoruyla karışmasın diye Map7 öneki
    private const string BestKey = "Map7_best";

    public void Configure(TMP_Text text)
    {
        resultText = text;
    }

    // Tur bitince Map7_Round bunu çağırır
    public void Show(string message, int score)
    {
        // En iyi skor bu bilgisayarda saklanır (Stop ve Unity kapanmasından sonra da kalır)
        int best = PlayerPrefs.GetInt(BestKey, 0);
        if (score > best) best = score;
        PlayerPrefs.SetInt(BestKey, best);

        if (resultText != null)
            resultText.text = message + "\nSkor " + score + "\nEn iyi " + best;

        gameObject.SetActive(true);   // panel görünür olur

        // First person'da fare kilitliydi: "Tekrar oyna" butonuna tıklanabilsin diye serbest bırak
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // "Tekrar oyna" butonunun On Click () olayı bunu çağırır
    public void Restart()
    {
        // Round zamanı durdurmuştu (Time.timeScale = 0): sahneyi yüklemeden önce tekrar başlat
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
