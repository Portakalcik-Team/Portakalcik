using UnityEngine;
using TMPro;

public class Map2_Round : MonoBehaviour
{
    // how long a round lasts, and the two HUD texts
    [SerializeField] private float roundLength = 120f;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    // the end screen
    [SerializeField] private Map2_GameOver gameOver;

    private int score;        // points this round
    private float timeLeft;   // seconds until the end
    private bool over;        // true once the round has ended

    void Start()
    {
        timeLeft = roundLength;   // a full clock
    }

    void Update()
    {
        if (over) return;   // ended: nothing changes any more

        // count down, but never below zero
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        ShowHud();

        if (timeLeft <= 0f) EndRound("Time's up!");
    }

    // the trees call this
    public void AddPoints(int points)
    {
        if (over) return;
        score += points;
    }

    // the exit calls this: every second still on the clock is worth 1 point
    public void ExitReached()
    {
        if (over) return;
        int bonus = Mathf.CeilToInt(timeLeft);
        score += bonus;
        ShowHud();
        EndRound("Map 2 complete!\nTime bonus +" + bonus);
    }

    // time's up - or the exit ends the round
    public void EndRound(string message)
    {
        if (over) return;
        over = true;
        gameOver.Show(message, score);   // first the end screen...
        Time.timeScale = 0f;             // ...then stop the world (no panel = no frozen game)
    }

    // write both numbers on the screen
    void ShowHud()
    {
        scoreText.text = "Score " + score;
        int seconds = Mathf.CeilToInt(timeLeft);
        int minutes = seconds / 60;
        timeText.text = minutes + ":" + (seconds % 60).ToString("00");
    }
}
