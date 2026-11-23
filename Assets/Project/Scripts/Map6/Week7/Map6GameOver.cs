using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// Hafta 7: Raunt bitiş panelini gösterir, en yüksek skoru kaydeder ve sahneyi yeniden başlatır
public class Map6GameOver : MonoBehaviour
{
    [Header("Oyun Sonu Paneli Özelliği")]
    [SerializeField] private TMP_Text resultText = null;
    [SerializeField] private string bestScoreKey = "best";

    public void Show(string message, int score)
    {
        int best = PlayerPrefs.GetInt(bestScoreKey, 0);
        if (score > best)
        {
            best = score;
            PlayerPrefs.SetInt(bestScoreKey, best);
            PlayerPrefs.Save();
        }

        if (resultText != null)
        {
            resultText.text = message + "\nScore " + score + "\nBest " + best;
        }

        gameObject.SetActive(true);

        // FPS modunda fare imleci kilitliyse butona tıklanabilmesi için serbest bırak
        Map6MouseLook look = FindAnyObjectByType<Map6MouseLook>();
        if (look != null)
        {
            look.SetMouseLookEnabled(false);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
