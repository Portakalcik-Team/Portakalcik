using UnityEngine;
using UnityEngine.InputSystem;

// "Meyve Kepçesi": uzun saplı ahşap bir kepçe-mancınık. Çantadaki meyveyi kepçeye yükleyip fırlatır.
//  - Sol tık basılı tut: kepçeyi geri çek (güç dolar), bırak: meyveyi fırlat
//  - 1 / 2 / 3 veya fare tekerleği: Limon / Mandalina / Portakal seç
//  - Limon: düşmanı 3 sn dondurur, Mandalina: hafif ve hızlı, Portakal: ağır vurur (2 hasar)
// Third person'da kepçe oyuncunun sol elinde, first person'da kameranın önünde görünür.
public class Map7_FruitLauncher : MonoBehaviour
{
    [SerializeField] private float minSpeed = 7f;        // en zayıf atış hızı
    [SerializeField] private float maxSpeed = 22f;       // tam şarjlı atış hızı
    [SerializeField] private float chargeTime = 1.1f;    // tam şarj süresi (sn)
    [SerializeField] private float arc = 0.12f;          // atışa verilen yukarı eğim

    private Map7_FruitBag bag;
    private Map7_ThirdPersonCamera camScript;
    private Transform cam;

    private Map7_FruitType selected = Map7_FruitType.Limon;
    private bool charging = false;
    private float charge = 0f;      // 0..1
    private float kick = 0f;        // atış anındaki ileri savrulma animasyonu
    private float emptyTimer = 0f;

    private Transform handModel, viewModel;
    private Transform handFruit, viewFruit;
    private Renderer handFruitR, viewFruitR;
    private Vector3 handBasePos, viewBasePos;
    private Quaternion handBaseRot, viewBaseRot;

    void Start()
    {
        bag = GetComponent<Map7_FruitBag>();
        BuildHandModel();
    }

    void Update()
    {
        if (cam == null)
        {
            Camera c = Camera.main;
            if (c != null)
            {
                cam = c.transform;
                camScript = c.GetComponent<Map7_ThirdPersonCamera>();
                BuildViewModel(cam);
            }
        }

        HandleSelection();
        HandleFire();
        UpdateModels();

        if (kick > 0f) kick = Mathf.Max(0f, kick - Time.deltaTime * 4f);
        if (emptyTimer > 0f) emptyTimer -= Time.deltaTime;
    }

    // ---------- Girdi ----------

    void HandleSelection()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) selected = Map7_FruitType.Limon;
            if (Keyboard.current.digit2Key.wasPressedThisFrame) selected = Map7_FruitType.Mandalina;
            if (Keyboard.current.digit3Key.wasPressedThisFrame) selected = Map7_FruitType.Portakal;
        }
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (scroll > 0.1f) selected = (Map7_FruitType)(((int)selected + 1) % 3);
            else if (scroll < -0.1f) selected = (Map7_FruitType)(((int)selected + 2) % 3);
        }
    }

    void HandleFire()
    {
        if (Mouse.current == null || bag == null) return;

        // İmleç kilitli değilken (menüye tıklarken vb.) atış yapma
        bool locked = Cursor.lockState == CursorLockMode.Locked;

        if (!charging && locked && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (bag.Count(selected) > 0) { charging = true; charge = 0f; }
            else emptyTimer = 1.2f;
        }

        if (charging)
        {
            charge = Mathf.Clamp01(charge + Time.deltaTime / chargeTime);
            if (Mouse.current.leftButton.wasReleasedThisFrame || !locked)
            {
                Fire();
                charging = false;
                charge = 0f;
            }
        }
    }

    void Fire()
    {
        if (cam == null || !bag.Use(selected, 1)) return;

        bool fp = camScript != null && camScript.IsFirstPerson;

        // Nereye nişan alındığını bul (oyuncunun kendisini yok say)
        Vector3 aimPoint = cam.position + cam.forward * 40f;
        RaycastHit[] hits = Physics.RaycastAll(cam.position, cam.forward, 60f, ~0, QueryTriggerInteraction.Ignore);
        float best = float.MaxValue;
        foreach (RaycastHit h in hits)
        {
            if (h.transform.IsChildOf(transform)) continue;
            if (h.distance < best) { best = h.distance; aimPoint = h.point; }
        }

        // Fırlatma noktası: first person'da kameranın önü, third person'da oyuncunun eli
        Vector3 muzzle;
        if (fp) muzzle = cam.position + cam.forward * 0.7f + cam.right * 0.25f - cam.up * 0.2f;
        else
        {
            Vector3 flat = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            muzzle = transform.position + Vector3.up * 0.4f + flat * 0.8f;
        }

        Vector3 dir = (aimPoint - muzzle).normalized;
        dir = (dir + Vector3.up * arc).normalized;

        float speed = Mathf.Lerp(minSpeed, maxSpeed, charge);
        if (selected == Map7_FruitType.Mandalina) speed *= 1.15f;   // hafif ve hızlı
        if (selected == Map7_FruitType.Portakal) speed *= 0.9f;     // ağır

        Map7_ThrownFruit.Spawn(selected, muzzle, dir * speed, gameObject);
        kick = 1f;
    }

    // ---------- Modeller ----------

    void BuildHandModel()
    {
        // Third person: sol elin ucuna (ArmL varsa), yoksa oyuncunun yanına
        Transform arm = null;
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
            if (t.name == "ArmL") { arm = t; break; }

        Transform root = new GameObject("FruitLadle_Hand").transform;
        if (arm != null)
        {
            root.SetParent(arm, false);
            root.localPosition = new Vector3(0f, -0.46f, 0.05f);
            root.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            root.SetParent(transform, false);
            root.localPosition = new Vector3(-0.45f, -0.1f, 0.2f);
        }
        root.localScale = Vector3.one * 0.8f;
        BuildLadle(root, out handFruit);
        handFruitR = handFruit.GetComponent<Renderer>();
        handModel = root;
        handBasePos = root.localPosition;
        handBaseRot = root.localRotation;
    }

    void BuildViewModel(Transform camTransform)
    {
        // First person: kameranın sağ-alt köşesinde
        Transform root = new GameObject("FruitLadle_View").transform;
        root.SetParent(camTransform, false);
        root.localPosition = new Vector3(0.32f, -0.3f, 0.5f);
        root.localRotation = Quaternion.Euler(-8f, -6f, 0f);
        root.localScale = Vector3.one * 0.9f;
        BuildLadle(root, out viewFruit);
        viewFruitR = viewFruit.GetComponent<Renderer>();
        viewModel = root;
        viewBasePos = root.localPosition;
        viewBaseRot = root.localRotation;

        // Kameraya bağlı model gölge atmasın
        foreach (Renderer r in root.GetComponentsInChildren<Renderer>())
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }

    // Kepçe: uzun ahşap sap + yeşil sargı + çukur kepçe kısmı + içinde yüklü meyve (yerel +Z ileri)
    void BuildLadle(Transform root, out Transform fruit)
    {
        Color wood = new Color(0.5f, 0.33f, 0.16f);
        Color dark = new Color(0.33f, 0.2f, 0.09f);
        Color wrap = new Color(0.25f, 0.6f, 0.25f);

        Transform handle = Part(PrimitiveType.Cylinder, "Handle", new Vector3(0f, 0f, 0.35f), new Vector3(0.05f, 0.45f, 0.05f), wood, root);
        handle.localRotation = Quaternion.Euler(90f, 0f, 0f);
        Transform grip = Part(PrimitiveType.Cylinder, "Grip", new Vector3(0f, 0f, 0.02f), new Vector3(0.075f, 0.1f, 0.075f), wrap, root);
        grip.localRotation = Quaternion.Euler(90f, 0f, 0f);
        Part(PrimitiveType.Sphere, "Bowl", new Vector3(0f, 0.02f, 0.85f), new Vector3(0.3f, 0.12f, 0.3f), dark, root);
        Part(PrimitiveType.Sphere, "BowlInner", new Vector3(0f, 0.06f, 0.85f), new Vector3(0.25f, 0.07f, 0.25f), wood, root);

        fruit = Part(PrimitiveType.Sphere, "LoadedFruit", new Vector3(0f, 0.17f, 0.85f), Vector3.one * 0.2f, Color.white, root);
        fruit.GetComponent<Renderer>().sharedMaterial = Map7_ThrownFruit.FruitMaterial(selected);
    }

    Transform Part(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Color color, Transform parent)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        Destroy(go.GetComponent<Collider>());
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = Map7_ThrownFruit.MakeMaterial(color);
        return go.transform;
    }

    void UpdateModels()
    {
        bool fp = camScript != null && camScript.IsFirstPerson;
        if (handModel != null) handModel.gameObject.SetActive(!fp);
        if (viewModel != null) viewModel.gameObject.SetActive(fp);

        // Yüklü meyve seçime göre değişir; çantada yoksa kepçe boş görünür
        bool loaded = bag != null && bag.Count(selected) > 0;
        ApplyFruit(handFruit, handFruitR, loaded);
        ApplyFruit(viewFruit, viewFruitR, loaded);

        // Geri çekme + atış animasyonu
        float pull = charging ? charge : 0f;
        Vector3 pullOffset = new Vector3(0f, 0.05f * pull, -0.18f * pull);
        float tilt = -30f * pull + 55f * kick;   // geri çekerken yukarı, atarken ileri savrul

        if (handModel != null)
        {
            handModel.localPosition = handBasePos + pullOffset * 0.8f;
            handModel.localRotation = handBaseRot * Quaternion.Euler(tilt, 0f, 0f);
        }
        if (viewModel != null)
        {
            viewModel.localPosition = viewBasePos + pullOffset;
            viewModel.localRotation = viewBaseRot * Quaternion.Euler(tilt, 0f, 0f);
        }
    }

    void ApplyFruit(Transform fruit, Renderer r, bool loaded)
    {
        if (fruit == null || r == null) return;
        fruit.gameObject.SetActive(loaded);
        if (!loaded) return;
        r.sharedMaterial = Map7_ThrownFruit.FruitMaterial(selected);
        fruit.localScale = Map7_ThrownFruit.FruitScale(selected) * 0.95f;
    }

    // ---------- Ekran yazıları ----------

    void OnGUI()
    {
        // Nişangâh
        float cx = Screen.width / 2f, cy = Screen.height / 2f;
        GUI.color = new Color(1f, 1f, 1f, 0.85f);
        GUI.DrawTexture(new Rect(cx - 2, cy - 2, 4, 4), Texture2D.whiteTexture);
        GUI.color = Color.white;

        // Şarj çubuğu
        if (charging)
        {
            float w = 160f, h = 12f;
            float x = cx - w / 2f, y = cy + 40f;
            GUI.color = Color.black;
            GUI.DrawTexture(new Rect(x - 2, y - 2, w + 4, h + 4), Texture2D.whiteTexture);
            GUI.color = Color.Lerp(Color.yellow, new Color(1f, 0.35f, 0f), charge);
            GUI.DrawTexture(new Rect(x, y, w * charge, h), Texture2D.whiteTexture);
            GUI.color = Color.white;
        }

        if (bag == null) return;

        // Alt orta: meyve seçimi ve adetler
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 18;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;

        string[] names = { "Limon", "Mandalina", "Portakal" };
        float slotW = 160f;
        float startX = cx - slotW * 1.5f;
        float by = Screen.height - 70f;
        for (int i = 0; i < 3; i++)
        {
            Map7_FruitType t = (Map7_FruitType)i;
            Rect r = new Rect(startX + i * slotW, by, slotW - 8f, 34f);
            GUI.color = (t == selected) ? new Color(0f, 0f, 0f, 0.75f) : new Color(0f, 0f, 0f, 0.4f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = Color.white;
            style.normal.textColor = (t == selected) ? Map7_ThrownFruit.FruitColor(t) : new Color(1f, 1f, 1f, 0.7f);
            GUI.Label(r, (i + 1) + "  " + names[i] + "  x" + bag.Count(t), style);
        }

        GUIStyle hint = new GUIStyle(GUI.skin.label);
        hint.fontSize = 13;
        hint.alignment = TextAnchor.MiddleCenter;
        hint.normal.textColor = new Color(1f, 1f, 1f, 0.8f);
        GUI.Label(new Rect(0, by + 36f, Screen.width, 22f),
            "Meyve Kepçesi: sol tık basılı tut, bırakınca fırlat  |  1/2/3 veya tekerlek: meyve seç  |  E: topla", hint);

        if (emptyTimer > 0f)
        {
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(0, cy + 60f, Screen.width, 30f), "Çantada " + selected + " kalmadı!", style);
        }
    }
}
