using UnityEngine;

// Map 7 kapısı: katı bir engeldir. Oyuncu portakal toplamışsa kapıya değince yere gömülerek açılır,
// yoksa kapalı kalır ve ekranda ipucu çıkar. (Portakal harcanmaz, anahtar gibi sadece gerekir.)
public class Map7_Door : MonoBehaviour
{
    [SerializeField] private Map7_FruitType requiredFruit = Map7_FruitType.Portakal;
    [SerializeField] private int requiredAmount = 1;
    [SerializeField] private bool consumeFruit = false;   // true yaparsan açarken meyve harcanır
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private float sinkDistance = 3.2f;

    private bool isOpen = false;
    private Vector3 targetPos;
    private Collider col;
    private string message = "";
    private float messageTimer = 0f;

    void Awake()
    {
        col = GetComponent<Collider>();
        // Prefab/sahne kopyalarında trigger kalmış olabilir; kapının engel olması için katı yap.
        if (col != null) col.isTrigger = false;
    }

    void Start()
    {
        targetPos = transform.position;
    }

    void Update()
    {
        if (isOpen && transform.position != targetPos)
            transform.position = Vector3.MoveTowards(transform.position, targetPos, openSpeed * Time.deltaTime);

        if (messageTimer > 0f) messageTimer -= Time.deltaTime;
    }

    void OnCollisionEnter(Collision c) { TryOpen(c.collider); }
    void OnCollisionStay(Collision c) { if (!isOpen && messageTimer <= 0f) TryOpen(c.collider); }

    void TryOpen(Collider other)
    {
        if (isOpen || !other.CompareTag("Player")) return;

        Map7_FruitBag bag = other.GetComponentInParent<Map7_FruitBag>();
        if (bag == null) return;

        if (bag.Count(requiredFruit) >= requiredAmount)
        {
            if (consumeFruit) bag.Use(requiredFruit, requiredAmount);
            Open();
        }
        else
        {
            Say("Kapı kilitli! Açmak için " + requiredAmount + " " + requiredFruit + " topla.");
        }
    }

    void Open()
    {
        isOpen = true;
        targetPos = transform.position - new Vector3(0f, sinkDistance, 0f);
        if (col != null) col.enabled = false;   // açılınca geçilebilsin
        Say("Kapı açıldı!");
    }

    void Say(string text)
    {
        message = text;
        messageTimer = 2f;
        Debug.Log("[Map7 Kapı] " + text);
    }

    void OnGUI()
    {
        if (messageTimer <= 0f || string.IsNullOrEmpty(message)) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 22;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(0, Screen.height * 0.7f, Screen.width, 40), message, style);
    }
}
