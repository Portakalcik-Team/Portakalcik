using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Map2_GameOver : MonoBehaviour
{
    // the big text in the middle of the panel
    [SerializeField] private TMP_Text resultText;

    // the Round calls this when the round ends
    public void Show(string message, int score)
    {
        // the best score is kept on this computer - "Map2_best", so other maps keep their own
        int best = PlayerPrefs.GetInt("Map2_best", 0);
        if (score > best) best = score;
        PlayerPrefs.SetInt("Map2_best", best);

        resultText.text = message + "\nScore " + score + "\nBest " + best;
        gameObject.SetActive(true);   // the panel appears

        // first person locks the mouse - free it, or the button can't be clicked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // the Play again button calls this
    public void Restart()
    {
        // the Round stopped time: start it again first
        Time.timeScale = 1f;
        // load the scene we are in once more: a fresh round (Map2_MouseLook locks the mouse again)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
