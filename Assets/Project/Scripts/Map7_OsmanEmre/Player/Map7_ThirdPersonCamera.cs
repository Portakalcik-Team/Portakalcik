using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

// Map 7 kamera kontrolcüsü (Cinemachine ile, sadece First Person + çıkışta sinematik CM_Overview).
// - Normal oynanışta CM_FirstPerson aktiftir; mouse bakışı sürer.
// - Labirentin sonundaki OverviewZone içine girildiğinde CM_Overview (öncelik 20) devreye girer ve çıkışı yukarıdan gösterir.
// - Oyun bittiğinde (Time.timeScale == 0) imleç serbest kalır, kamera dönmez ve tıklama imleci tekrar kilitlemez.
public class Map7_ThirdPersonCamera : MonoBehaviour
{
    [Header("First person (CM_FirstPerson)")]
    [SerializeField] private float eyeHeight = 0.6f;            // oyuncu merkezinden göz yüksekliği
    [SerializeField] private float fpMinPitch = -80f;           // en fazla yukarı bakış
    [SerializeField] private float fpMaxPitch = 80f;            // en fazla aşağı bakış
    [SerializeField] private float nearClip = 0.05f;            // first person'da yakın kesme mesafesi

    [Header("Ortak")]
    [SerializeField] private float yaw = 180f;                  // başlangıç yatay açı
    [SerializeField] private float pitch = 10f;                 // başlangıç dikey açı
    [SerializeField] private float mouseSensitivity = 0.15f;    // piksel başına derece
    [SerializeField] private bool lockCursor = true;            // oyun başında imleci kilitle

    // CM_Overview aktif değilken First Person modundayız (Meyve Kepçesi buna bakar)
    public bool IsFirstPerson => !(cmOverview != null && cmOverview.gameObject.activeInHierarchy);

    private const string RigName = "Map7_CameraRig";

    private Transform target;            // oyuncu
    private Transform eyePoint;          // oyuncunun gözü
    private Renderer[] bodyRenderers;
    private bool lastBodyHidden;

    private CinemachineBrain brain;
    private CinemachineCamera cmFirst, cmOverview;
    private CinemachinePanTilt panTilt;

    void Awake()
    {
        EnsureRig();
    }

    void Start()
    {
        SetCursorLocked(lockCursor);
    }

    // ---------- Rig kurulumu ----------

    // Sahnede Brain, CM_FirstPerson, CM_Overview ve OverviewZone yoksa kurar; eski CM_Follow varsa kapatır.
    [ContextMenu("Setup Cinemachine Rig")]
    public void EnsureRig()
    {
        // 1) Brain: Main Camera'yı Cinemachine kameralarına bağlar
        brain = GetComponent<CinemachineBrain>();
        if (brain == null)
        {
            brain = gameObject.AddComponent<CinemachineBrain>();
            brain.DefaultBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Styles.EaseInOut, 0.5f);
        }

        // 2) Rig kökü
        GameObject rootGO = GameObject.Find(RigName);
        if (rootGO == null) rootGO = new GameObject(RigName);
        Transform root = rootGO.transform;

        // Sahnede önceden kalmış CM_Follow (third person) varsa kapat
        Transform oldFollow = root.Find("CM_Follow");
        if (oldFollow != null) oldFollow.gameObject.SetActive(false);

        // 3) CM_FirstPerson: ana oynanış kamerası (öncelik 10)
        Transform t = root.Find("CM_FirstPerson");
        if (t == null)
        {
            GameObject go = new GameObject("CM_FirstPerson");
            go.transform.SetParent(root, false);
            cmFirst = go.AddComponent<CinemachineCamera>();
            cmFirst.Lens.FieldOfView = 75f;
            cmFirst.Lens.NearClipPlane = nearClip;
            cmFirst.Priority.Value = 10;

            go.AddComponent<CinemachineHardLockToTarget>();          // oyuncunun gözüne kilitlenir
            panTilt = go.AddComponent<CinemachinePanTilt>();         // mouse ile bakış yönü
            panTilt.PanAxis.Value = yaw;
            panTilt.PanAxis.Recentering.Enabled = false;
            panTilt.TiltAxis.Range = new Vector2(fpMinPitch, fpMaxPitch);
            panTilt.TiltAxis.Value = Mathf.Clamp(pitch, fpMinPitch, fpMaxPitch);
            panTilt.TiltAxis.Recentering.Enabled = false;

            go.AddComponent<CinemachineImpulseListener>();           // sarsıntıyı dinler
            go.SetActive(true);
        }
        else
        {
            cmFirst = t.GetComponent<CinemachineCamera>();
            if (cmFirst != null) cmFirst.Priority.Value = 10;
            t.gameObject.SetActive(true);
        }
        if (cmFirst != null) panTilt = cmFirst.GetComponent<CinemachinePanTilt>();

        // 4) CM_Overview + OverviewZone: labirent sonu çıkış bölgesinde sinematik tepe açısı (öncelik 20)
        t = root.Find("CM_Overview");
        if (t == null)
        {
            GameObject go = new GameObject("CM_Overview");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(28f, 32f, -52f);
            go.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
            cmOverview = go.AddComponent<CinemachineCamera>();
            cmOverview.Lens.FieldOfView = 60f;
            cmOverview.Priority.Value = 20;
            go.AddComponent<CinemachineImpulseListener>();
            go.SetActive(false);                                      // bölgeye girince açılır
        }
        else
        {
            cmOverview = t.GetComponent<CinemachineCamera>();
            if (cmOverview != null) cmOverview.Priority.Value = 20;
        }

        if (root.Find("OverviewZone") == null && cmOverview != null)
        {
            GameObject zone = new GameObject("OverviewZone");
            zone.transform.SetParent(root, false);
            zone.transform.position = new Vector3(28f, 2f, -30f);
            BoxCollider box = zone.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.size = new Vector3(20f, 8f, 14f);
            zone.AddComponent<Map7_CameraZone>().Configure(cmOverview.gameObject);
        }
    }

    // ---------- Her kare ----------

    void Update()
    {
        // Oyun bittiyse (Game Over ekranı açık) imleci kilitleme!
        if (Time.timeScale == 0f) return;

        // Esc: imleci bırak, sol tık: tekrar kilitle
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);
        else if (lockCursor && Cursor.lockState != CursorLockMode.Locked &&
                 Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);
    }

    void LateUpdate()
    {
        if (Time.timeScale == 0f) return;

        // Oyuncu MapGenerator tarafından oyun başında oluşturulduğu için çalışma anında bul
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) return;
            target = p.transform;
            eyePoint = FindOrCreateChild(target, "Map7_EyePoint", new Vector3(0f, eyeHeight, 0f));

            if (cmFirst != null) cmFirst.Follow = eyePoint;
            ApplyBodyVisibility(true, true);
        }

        // Overview kamerası açıldığında karakter gövdesi görünsün, First Person'da gizlensin
        bool hideBody = IsFirstPerson;
        if (hideBody != lastBodyHidden)
            ApplyBodyVisibility(hideBody, false);

        // Mouse ile kamerayı döndür (sadece imleç kilitliyken)
        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * mouseSensitivity;
            pitch -= delta.y * mouseSensitivity;
        }
        yaw = Mathf.Repeat(yaw + 180f, 360f) - 180f;
        pitch = Mathf.Clamp(pitch, fpMinPitch, fpMaxPitch);

        if (panTilt != null)
        {
            panTilt.PanAxis.Value = yaw;
            panTilt.TiltAxis.Value = pitch;
        }
    }

    // First person'da gövde görünmez ama gölgesi kalır; sinematik tepe kamerasında gövde görünür
    void ApplyBodyVisibility(bool hideBody, bool force)
    {
        if (target == null) return;
        if (!force && hideBody == lastBodyHidden) return;
        lastBodyHidden = hideBody;

        if (bodyRenderers == null || bodyRenderers.Length == 0)
            bodyRenderers = target.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer r in bodyRenderers)
        {
            if (r == null) continue;
            r.shadowCastingMode = hideBody
                ? UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly
                : UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

    static Transform FindOrCreateChild(Transform parent, string childName, Vector3 localPos)
    {
        Transform c = parent.Find(childName);
        if (c == null)
        {
            c = new GameObject(childName).transform;
            c.SetParent(parent, false);
        }
        c.localPosition = localPos;
        return c;
    }

    void OnDisable()
    {
        SetCursorLocked(false);
    }

    static void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
