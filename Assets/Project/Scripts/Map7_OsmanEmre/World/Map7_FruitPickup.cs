using UnityEngine;
using UnityEngine.InputSystem;

// Meyve ağacı: oyuncu ağacın yanındayken E'ye her basışta TEK meyve toplanır.
// Ağaçtaki görsel meyveler (Map7_TreeLook) teker teker kaybolur; hepsi bitince ağaç boş kalır.
public class Map7_FruitPickup : MonoBehaviour
{
    [SerializeField] private GameObject fruit;           // Eski tip tek küre meyve (Boş bıraksan da kendi bulur!)
    [SerializeField] private Map7_FruitType fruitType;   // ARTIK MAP7 OLDU: Limon, Mandalina, Portakal
    [SerializeField] private int amount = 2;             // Görsel meyve grubu yoksa ağaçtaki toplam meyve sayısı
    [SerializeField] private float pickInterval = 0.25f; // Üst üste toplamalar arası bekleme

    private Transform treeFruits;                        // Map7_TreeLook'un kurduğu görsel meyveler (varsa)
    private int fruitsLeft;                              // Ağaçta kalan meyve
    private Map7_FruitBag bagInRange;                    // Menzildeki oyuncunun çantası
    private float cooldown = 0f;
    private Transform aimed;                             // Nişan alınan (imleçle gösterilen) meyve
    private Vector3 aimedBaseScale;
    private string message = "";
    private float messageTimer = 0f;

    void Start()
    {
        // Ağacın gövde (Trunk) alt objesinde de aynı script var; çift toplamayı önlemek için
        // üstte zaten bir Map7_FruitPickup varsa bu kopya kendini kapatır.
        if (transform.parent != null && transform.parent.GetComponentInParent<Map7_FruitPickup>() != null)
        {
            enabled = false;
            return;
        }

        // Eğer Inspector'dan Sphere'i sürükleyemediysen, ağacın içindeki küreyi otomatik bulur!
        if (fruit == null)
        {
            var bob = GetComponentInChildren<Map7_FruitBob>();
            if (bob != null) fruit = bob.gameObject;
        }

        // Yeni ağaç görünümündeki meyve grubunu bul
        Transform fruits = transform.Find("Visual/Crown/Fruits");
        if (fruits != null)
        {
            treeFruits = fruits;
            fruitsLeft = fruits.childCount;
        }
        else
        {
            fruitsLeft = amount;
        }
    }

    void Update()
    {
        if (cooldown > 0f) cooldown -= Time.deltaTime;
        if (messageTimer > 0f) messageTimer -= Time.deltaTime;

        if (bagInRange == null)
        {
            SetAimed(null);
            return;
        }

        // İmleç (ekran ortası) hangi meyvenin üstündeyse onu bul ve vurgula
        if (treeFruits != null) SetAimed(FindAimedFruit());

        if (cooldown > 0f) return;
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            CollectOne();
    }

    // Kameranın ekran ortasından çıkan ışına en yakın meyveyi döndürür (yoksa null).
    Transform FindAimedFruit()
    {
        Camera cam = Camera.main;
        if (cam == null) return null;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Transform best = null;
        float bestT = float.MaxValue;
        foreach (Transform f in treeFruits)
        {
            if (!f.gameObject.activeSelf) continue;   // toplanmış (silinmek üzere) meyveyi atla

            Vector3 oc = f.position - ray.origin;
            float along = Vector3.Dot(oc, ray.direction);
            if (along < 0f || along > 25f) continue;

            // Işının meyve merkezine uzaklığı, meyvenin yarıçapı + biraz tolerans içindeyse "üzerinde"
            float distSqr = oc.sqrMagnitude - along * along;
            Renderer r = f.GetComponent<Renderer>();
            float radius = (r != null ? Mathf.Max(r.bounds.extents.x, r.bounds.extents.y, r.bounds.extents.z) : 0.15f) + 0.12f;
            if (distSqr <= radius * radius && along < bestT) { bestT = along; best = f; }
        }
        return best;
    }

    // Nişan alınan meyveyi büyütüp vurgular; öncekini eski boyuta döndürür.
    void SetAimed(Transform f)
    {
        if (f == aimed) return;
        if (aimed != null) aimed.localScale = aimedBaseScale;
        aimed = f;
        if (aimed != null)
        {
            aimedBaseScale = aimed.localScale;
            aimed.localScale = aimedBaseScale * 1.35f;
        }
    }

    void CollectOne()
    {
        if (fruitsLeft <= 0)
        {
            Say("Bu ağaçta meyve kalmadı.");
            return;
        }

        // Ağaçta görsel meyveler varsa sadece imleçle gösterilen meyve toplanır
        if (treeFruits != null && aimed == null)
        {
            Say("Toplamak için imleci bir meyvenin üzerine getir.");
            return;
        }

        fruitsLeft--;
        cooldown = pickInterval;

        if (treeFruits != null)
        {
            Transform picked = aimed;
            SetAimed(null);
            picked.gameObject.SetActive(false);   // hemen gizle (Destroy aynı karenin sonunda olur)
            Destroy(picked.gameObject);
        }
        else if (fruitsLeft <= 0 && fruit != null)
        {
            Destroy(fruit);   // eski tip tek küre meyve
        }

        bagInRange.Add(fruitType, 1);
        Say("+1 " + fruitType + "  (ağaçta " + fruitsLeft + " kaldı)");
    }

    void OnTriggerEnter(Collider other)
    {
        // Sadece oyuncu toplayabilir. (Eskiden başka ağaçlar da çantası olduğu için ağacın meyvesini
        // oyun başında kendi kendine toplayıp kaybolmasına neden olabiliyordu.)
        if (!other.CompareTag("Player")) return;

        // ARTIK MAP2 DEĞİL, SENİN MAP7_FRUITBAG ÇANTANI ARIYOR!
        Map7_FruitBag bag = other.GetComponentInParent<Map7_FruitBag>();
        if (bag == null) bag = other.GetComponentInChildren<Map7_FruitBag>();
        if (bag != null) bagInRange = bag;
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        bagInRange = null;
        SetAimed(null);
    }

    void Say(string text)
    {
        message = text;
        messageTimer = 1.5f;
    }

    void OnGUI()
    {
        string text = null;
        if (messageTimer > 0f) text = message;
        else if (bagInRange != null)
            text = fruitsLeft <= 0
                ? "Bu ağaçta meyve kalmadı."
                : (treeFruits == null || aimed != null
                    ? "[E] " + fruitType + " topla  (" + fruitsLeft + " meyve var)"
                    : "İmleci bir meyvenin üzerine getir  (" + fruitsLeft + " meyve var)");
        if (text == null) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.yellow;
        GUI.Label(new Rect(0, Screen.height * 0.62f, Screen.width, 36), text, style);
    }
}
