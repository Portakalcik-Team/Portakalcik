using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Meyve ağacı: oyuncu ağacın yanındayken imleci bir meyvenin üstüne getirip E'ye basınca O meyve toplanır.
// Toplanan meyve bir süre sonra kendiliğinden yeniden yetişir (W06: zamanlayıcılı spawner, Instantiate).
// Meyvenin puanı, yükü ve yetişme süresi Map7_FruitData dosyasından gelir.
public class Map7_FruitPickup : MonoBehaviour
{
    [SerializeField] private GameObject fruit;           // Eski tip tek küre meyve (Boş bıraksan da kendi bulur!)
    [SerializeField] private Map7_FruitType fruitType;   // ARTIK MAP7 OLDU: Limon, Mandalina, Portakal
    [SerializeField] private int amount = 2;             // Görsel meyve grubu yoksa ağaçtaki toplam meyve sayısı
    [SerializeField] private float pickInterval = 0.25f; // Üst üste toplamalar arası bekleme
    [SerializeField] private Map7_FruitData fruitData;   // puan / yük / yetişme süresi (boşsa varsayılanlar)

    private Transform treeFruits;                        // Map7_TreeLook'un kurduğu görsel meyveler (varsa)
    private int fruitsLeft;                              // Ağaçta kalan meyve
    private Map7_FruitBag bagInRange;                    // Menzildeki oyuncunun çantası
    private float cooldown = 0f;
    private Transform aimed;                             // Nişan alınan (imleçle gösterilen) meyve
    private Vector3 aimedBaseScale;
    private string message = "";
    private float messageTimer = 0f;

    // Yeniden yetişme (spawner): toplanan meyvelerin yerleri ve geri sayım
    private GameObject fruitTemplate;                    // kopyalanacak gizli meyve
    private readonly List<Vector3> missingSpots = new List<Vector3>();
    private float regrowTimer = 0f;

    // Varsayılanlar (veri dosyası yoksa)
    private int Points() { return fruitData != null ? fruitData.Points() : 1; }
    private float Weight() { return fruitData != null ? fruitData.Weight() : 0f; }
    private float RegrowDelay() { return fruitData != null ? fruitData.RegrowDelay() : 10f; }
    private bool IsGolden() { return Weight() > 0f; }

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

            // Yeniden yetişme için gizli bir şablon meyve sakla
            if (fruits.childCount > 0 && fruitTemplate == null)
            {
                fruitTemplate = Instantiate(fruits.GetChild(0).gameObject, fruits);
                fruitTemplate.name = "FruitTemplate";
                fruitTemplate.SetActive(false);
            }
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

        // Zamanlayıcı: eksik meyve varsa süre dolunca bir tane yeniden yetişir
        if (missingSpots.Count > 0 && fruitTemplate != null)
        {
            regrowTimer -= Time.deltaTime;
            if (regrowTimer <= 0f)
            {
                Regrow();
                regrowTimer = RegrowDelay();
            }
        }

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

    // Şablondan yeni bir meyve kopyalar ve eksik yerine koyar
    void Regrow()
    {
        Vector3 spot = missingSpots[0];
        missingSpots.RemoveAt(0);

        GameObject f = Instantiate(fruitTemplate, treeFruits);
        f.name = "Fruit_Visual";
        f.transform.localPosition = spot;
        f.transform.localRotation = Quaternion.identity;
        f.SetActive(true);
        fruitsLeft++;
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
            if (!f.gameObject.activeSelf) continue;   // toplanmış veya şablon meyveyi atla

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
            aimedBaseScale = Vector3.zero;

            // Yeri hatırla: zamanlayıcı dolunca aynı yerde yeniden yetişecek
            missingSpots.Add(picked.localPosition);
            if (missingSpots.Count == 1) regrowTimer = RegrowDelay();

            picked.gameObject.SetActive(false);   // hemen gizle (Destroy aynı karenin sonunda olur)
            Destroy(picked.gameObject);
        }
        else if (fruitsLeft <= 0 && fruit != null)
        {
            Destroy(fruit);   // eski tip tek küre meyve
        }

        bagInRange.Add(fruitType, 1);

        // Skor ve yük (Altın meyve yavaşlatır)
        Map7_Score score = bagInRange.GetComponent<Map7_Score>();
        if (score == null) score = bagInRange.GetComponentInParent<Map7_Score>();
        if (score != null) score.Collect(fruitType, Points(), Weight());

        Say("+" + Points() + " " + (IsGolden() ? "Altın " : "") + fruitType + "  (ağaçta " + fruitsLeft + " kaldı)");
    }

    // Map7_LevelSetup çağırır: veri atanmamış ağaca normal veriyi verir
    public void SetDataIfEmpty(Map7_FruitData data)
    {
        if (fruitData == null) fruitData = data;
    }

    // Map7_LevelSetup çağırır: bu ağacı Altın Portakal ağacına çevirir (nadir, değerli, ağır, düşmana yakın)
    public void MakeGolden(Map7_FruitData data, int fruitCount)
    {
        fruitData = data;
        fruitType = Map7_FruitType.Portakal;   // kapı anahtarı ve kepçe için Portakal sayılır
        if (treeFruits == null) return;

        Color gold = new Color(1f, 0.84f, 0f);
        Material mat = Map7_ThrownFruit.MakeMaterial(gold);

        List<Transform> kids = new List<Transform>();
        foreach (Transform f in treeFruits) kids.Add(f);

        int kept = 0;
        foreach (Transform f in kids)
        {
            bool isTemplate = fruitTemplate != null && f.gameObject == fruitTemplate;
            if (!isTemplate && f.gameObject.activeSelf && kept >= fruitCount)
            {
                Destroy(f.gameObject);   // fazla meyveleri kaldır: Altın ağaçta az meyve olur
                continue;
            }

            Renderer r = f.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = mat;
            f.localScale *= 1.25f;
            if (!isTemplate) kept++;
        }
        fruitsLeft = kept;
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
        string name = (IsGolden() ? "Altın " : "") + fruitType;
        if (messageTimer > 0f) text = message;
        else if (bagInRange != null)
            text = fruitsLeft <= 0
                ? "Bu ağaçta meyve kalmadı."
                : (treeFruits == null || aimed != null
                    ? "[E] " + name + " topla  (+" + Points() + " puan" + (IsGolden() ? ", yavaşlatır!" : "") + ")"
                    : "İmleci bir meyvenin üzerine getir  (" + fruitsLeft + " meyve var)");
        if (text == null) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = IsGolden() ? new Color(1f, 0.84f, 0f) : Color.yellow;
        GUI.Label(new Rect(0, Screen.height * 0.62f, Screen.width, 36), text, style);
    }
}
