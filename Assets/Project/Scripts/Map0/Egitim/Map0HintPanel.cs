using UnityEngine;
using UnityEngine.UI;

// Ekranın altındaki ipucu kartı ve sağ üstteki ilerleme göstergesi.
// Nesneleri Map0TutorialVisuals kurar, bu script sadece doldurur ve canlandırır.
public class Map0HintPanel : MonoBehaviour
{
    public static Map0HintPanel Instance { get; private set; }

    [Header("Kart")]
    [SerializeField] private CanvasGroup cardGroup;
    [SerializeField] private RectTransform card;
    [SerializeField] private Image accentBar;
    [SerializeField] private Text stepLabel;
    [SerializeField] private Text titleText;
    [SerializeField] private Text bodyText;

    [Header("Canavar bilgisi")]
    [SerializeField] private GameObject monsterBlock;
    [SerializeField] private Text[] statValues = new Text[4]; // hız, can, hasar, vuruş
    [SerializeField] private Text tipText;

    [Header("İlerleme")]
    [SerializeField] private Text progressText;
    [SerializeField] private RectTransform progressFill;
    [SerializeField] private Image progressFillImage;

    [Header("Renkler")]
    [SerializeField] private Color defaultAccent = new Color(1f, 0.62f, 0.11f);  // portakal
    [SerializeField] private Color doneColor = new Color(0.36f, 0.78f, 0.42f);   // yeşil

    [Header("Animasyon")]
    [SerializeField] private float fadeSpeed = 10f;   // ne kadar çabuk açılır
    [SerializeField] private float slideDistance = 28f; // kapalıyken kaç piksel aşağıda

    private Map0TutorialHint current;   // şu an gösterilen ipucu
    private float targetAlpha;          // 1 = açık, 0 = kapalı
    private float cardBaseY;            // kartın açık hâldeki yüksekliği
    private float targetFill;           // 0..1 ilerleme
    private float shownFill;

    void Awake()
    {
        Instance = this;
        cardBaseY = card.anchoredPosition.y;
        cardGroup.alpha = 0f;
    }

    void Start()
    {
        // ilerlemeyi dinle; Start sırası belli olmadığı için bir kere de elle oku
        Map0TutorialProgress progress = Map0TutorialProgress.Instance;
        if (progress == null) return;
        progress.Changed += OnProgressChanged;
        OnProgressChanged(progress.Seen, progress.Total);
    }

    void OnDestroy()
    {
        if (Map0TutorialProgress.Instance != null)
            Map0TutorialProgress.Instance.Changed -= OnProgressChanged;
        if (Instance == this) Instance = null;
    }

    public void Show(Map0TutorialHint hint)
    {
        // başka bir ipucundan geçiyorsak kart kısaca söner, yenisi parlar
        if (current != null && current != hint) cardGroup.alpha *= 0.35f;
        current = hint;

        Map0MonsterData monster = hint.Monster;
        Color accent = monster != null ? monster.color : defaultAccent;
        accentBar.color = accent;

        stepLabel.text = monster != null
            ? "CANAVAR  ·  İLK KEZ " + monster.firstLevel.ToUpper()
            : "ADIM " + hint.StepNumber;
        titleText.text = hint.Title;
        bodyText.text = hint.Body;

        monsterBlock.SetActive(monster != null);
        if (monster != null)
        {
            statValues[0].text = monster.speedRatio.ToString("0.0") + "×";
            statValues[1].text = monster.hpLevel1.ToString();
            statValues[2].text = monster.damageLevel1.ToString();
            statValues[3].text = monster.HitsToKill().ToString();
            foreach (Text value in statValues) value.color = Lighten(accent);
            tipText.text = "İpucu: " + monster.tip;
        }

        targetAlpha = 1f;
    }

    public void Hide(Map0TutorialHint hint)
    {
        // komşu alana geçtiysek eskisinin çıkışı yenisini kapatmasın
        if (hint != current) return;
        current = null;
        targetAlpha = 0f;
    }

    void OnProgressChanged(int seen, int total)
    {
        if (total <= 0) return;
        targetFill = (float)seen / total;
        bool done = seen >= total;
        progressText.text = done ? "Tamamlandı!" : seen + " / " + total;
        progressFillImage.color = done ? doneColor : defaultAccent;
    }

    void Update()
    {
        // zamandan bağımsız yumuşak geçiş (oyun durdurulsa da çalışsın)
        float t = 1f - Mathf.Exp(-fadeSpeed * Time.unscaledDeltaTime);

        cardGroup.alpha = Mathf.Lerp(cardGroup.alpha, targetAlpha, t);
        float y = cardBaseY - (1f - cardGroup.alpha) * slideDistance;
        card.anchoredPosition = new Vector2(card.anchoredPosition.x, y);

        shownFill = Mathf.Lerp(shownFill, targetFill, t);
        progressFill.anchorMax = new Vector2(shownFill, 1f);
    }

    // koyu kartta okunur olsun diye rengi biraz aç
    static Color Lighten(Color c)
    {
        return Color.Lerp(c, Color.white, 0.45f);
    }
}
