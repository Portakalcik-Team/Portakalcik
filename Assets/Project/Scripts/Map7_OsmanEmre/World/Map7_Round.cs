using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;

// Map 7 tur yöneticisi (W07): skoru tutar, 90 sn geri sayar, Canvas HUD'a yazar ve turu bitirir.
public class Map7_Round : MonoBehaviour
{
    // Tur süresi ve HUD üzerindeki iki yazı
    [SerializeField] private float roundLength = 90f;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    // Bitiş ekranı
    [SerializeField] private Map7_GameOver gameOver;

    private int score;        // bu turdaki puan
    private float timeLeft;   // bitişe kalan saniye
    private bool over;        // tur bitince true olur

    public int Score() { return score; }
    public bool IsOver() { return over; }

    void Start()
    {
        EnsureUiIfMissing();
        timeLeft = roundLength;   // saat dolu başlar
        ShowHud();
    }

    void Update()
    {
        if (over) return;   // bitti: artık hiçbir şey değişmez

        // Geri say, ama sıfırın altına inme
        timeLeft -= Time.deltaTime;
        if (timeLeft < 0f) timeLeft = 0f;
        ShowHud();

        if (timeLeft <= 0f) EndRound("Süre doldu!");
    }

    // Meyve toplandığında Map7_Score burayı çağırır
    public void AddPoints(int points)
    {
        if (over) return;
        score += points;
    }

    // Süre dolduğunda veya oyuncu çıkışa (Map7_ExitZone) ulaştığında çağrılır
    public void EndRound(string message)
    {
        if (over) return;
        over = true;
        Time.timeScale = 0f;   // dünyayı durdur
        if (gameOver != null) gameOver.Show(message, score);
    }

    // Skor ve kalan süreyi ekrana (Canvas TMP) yazar
    void ShowHud()
    {
        if (scoreText != null) scoreText.text = "Skor " + score;
        if (timeText != null)
        {
            int seconds = Mathf.CeilToInt(timeLeft);
            int minutes = seconds / 60;
            timeText.text = minutes + ":" + (seconds % 60).ToString("00");
        }
    }

    // Sahneye henüz "Tools/Map7/Hafta 7 Kur" uygulanmamışsa çalışma anında otomatik kurar
    public static Map7_Round EnsureInScene()
    {
        Map7_Round existing = FindFirstObjectByType<Map7_Round>();
        if (existing != null)
        {
            existing.EnsureUiIfMissing();
            return existing;
        }

        GameObject go = new GameObject("Map7_Round");
        Map7_Round r = go.AddComponent<Map7_Round>();
        r.EnsureUiIfMissing();
        return r;
    }

    void EnsureUiIfMissing()
    {
        if (scoreText != null && timeText != null && gameOver != null) return;

        // 1) EventSystem (Input System uyumlu)
        EventSystem es = FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (es == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            es = esGo.AddComponent<EventSystem>();
            esGo.AddComponent<InputSystemUIInputModule>();
        }
        else if (es.GetComponent<InputSystemUIInputModule>() == null)
        {
            StandaloneInputModule old = es.GetComponent<StandaloneInputModule>();
            if (old != null) Destroy(old);
            es.gameObject.AddComponent<InputSystemUIInputModule>();
        }

        // 2) Canvas (1920x1080 Scale With Screen Size)
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            GameObject cGo = new GameObject("Canvas", typeof(RectTransform));
            cGo.layer = LayerMask.NameToLayer("UI");
            canvas = cGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = cGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            cGo.AddComponent<GraphicRaycaster>();
        }

        // 3) ScoreText (sol üst) ve TimeText (üst orta)
        if (scoreText == null)
            scoreText = MakeRuntimeText(canvas.transform, "ScoreText", "Skor 0",
                new Vector2(0f, 1f), new Vector2(40f, -40f), new Vector2(400f, 80f), 48, TextAlignmentOptions.MidlineLeft, Color.white);

        if (timeText == null)
            timeText = MakeRuntimeText(canvas.transform, "TimeText", "1:30",
                new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(300f, 80f), 48, TextAlignmentOptions.Center, Color.white);

        // 4) GameOverPanel + ResultText + PlayAgainButton
        if (gameOver == null)
        {
            Transform existingPanel = canvas.transform.Find("GameOverPanel");
            if (existingPanel != null) gameOver = existingPanel.GetComponent<Map7_GameOver>();
        }

        if (gameOver == null)
        {
            GameObject panel = new GameObject("GameOverPanel", typeof(RectTransform));
            panel.layer = LayerMask.NameToLayer("UI");
            panel.transform.SetParent(canvas.transform, false);
            RectTransform prt = (RectTransform)panel.transform;
            prt.anchorMin = Vector2.zero;
            prt.anchorMax = Vector2.one;
            prt.offsetMin = Vector2.zero;
            prt.offsetMax = Vector2.zero;

            Image pImg = panel.AddComponent<Image>();
            pImg.color = new Color32(0, 0, 0, 200);

            TMP_Text resText = MakeRuntimeText(panel.transform, "ResultText", "Süre doldu!\nSkor 0\nEn iyi 0",
                new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(900f, 360f), 72, TextAlignmentOptions.Center, Color.white);

            gameOver = panel.AddComponent<Map7_GameOver>();
            gameOver.Configure(resText);

            GameObject btnGo = new GameObject("PlayAgainButton", typeof(RectTransform));
            btnGo.layer = LayerMask.NameToLayer("UI");
            btnGo.transform.SetParent(panel.transform, false);
            RectTransform brt = (RectTransform)btnGo.transform;
            brt.anchorMin = new Vector2(0.5f, 0.5f);
            brt.anchorMax = new Vector2(0.5f, 0.5f);
            brt.pivot = new Vector2(0.5f, 0.5f);
            brt.anchoredPosition = new Vector2(0f, -260f);
            brt.sizeDelta = new Vector2(420f, 100f);

            Image bImg = btnGo.AddComponent<Image>();
            bImg.color = Color.white;
            Button btn = btnGo.AddComponent<Button>();
            btn.onClick.AddListener(gameOver.Restart);

            TMP_Text btnLabel = MakeRuntimeText(btnGo.transform, "Text (TMP)", "Tekrar oyna",
                new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(420f, 100f), 44, TextAlignmentOptions.Center, new Color32(0x1C, 0x1F, 0x26, 255));
            RectTransform lrt = (RectTransform)btnLabel.transform;
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;

            panel.SetActive(false);   // tur bitene kadar gizli
        }
    }

    static TMP_Text MakeRuntimeText(Transform parent, string name, string content,
        Vector2 anchor, Vector2 pos, Vector2 size, float fontSize, TextAlignmentOptions align, Color color)
    {
        Transform existing = parent.Find(name);
        if (existing != null && existing.GetComponent<TMP_Text>() != null)
            return existing.GetComponent<TMP_Text>();

        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);

        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = align;
        tmp.color = color;
        tmp.raycastTarget = false;
        tmp.outlineWidth = 0.2f;
        tmp.outlineColor = new Color32(0, 0, 0, 200);
        return tmp;
    }
}
