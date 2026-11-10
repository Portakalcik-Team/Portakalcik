using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

// Map 7 kamera kontrolcüsü (Cinemachine ile).
// Kamerayı artık Cinemachine Brain hareket ettirir; bu script sadece:
//   - mouse'u okuyup kameraları döndürür (yaw/pitch),
//   - V tuşuyla CM_FirstPerson kamerasını açıp kapatır (ikinci kamera),
//   - oyuncu doğunca kameraların hedeflerini atar,
//   - imleci kilitler (Esc bırakır, sol tık tekrar kilitler).
// Kamera rig'i (Map7_CameraRig) sahnede yoksa çalışma anında kurulur; Inspector'da ayarlanabilir.
public class Map7_ThirdPersonCamera : MonoBehaviour
{
    [Header("Mod")]
    [SerializeField] private bool firstPerson = false;          // true: oyun first person başlar
    [SerializeField] private Key toggleKey = Key.V;             // mod değiştirme tuşu

    [Header("Third person (CM_Follow)")]
    [SerializeField] private float distance = 7f;               // oyuncuya uzaklık
    [SerializeField] private float minPitch = 10f;              // en alçak kamera açısı
    [SerializeField] private float maxPitch = 70f;              // en yüksek kamera açısı
    [SerializeField] private float lookHeight = 0.6f;           // oyuncu merkezinden bakılan nokta yüksekliği

    [Header("First person (CM_FirstPerson)")]
    [SerializeField] private float eyeHeight = 0.6f;            // oyuncu merkezinden göz yüksekliği
    [SerializeField] private float fpMinPitch = -80f;           // en fazla yukarı bakış
    [SerializeField] private float fpMaxPitch = 80f;            // en fazla aşağı bakış
    [SerializeField] private float nearClip = 0.05f;            // first person'da yakın kesme mesafesi

    [Header("Ortak")]
    [SerializeField] private float yaw = 180f;                  // başlangıç yatay açı (180 = oyuncunun karşısından bakar)
    [SerializeField] private float pitch = 30f;                 // başlangıç dikey açı
    [SerializeField] private float mouseSensitivity = 0.15f;    // piksel başına derece
    [SerializeField] private bool lockCursor = true;            // oyun başında imleci kilitle

    // Diğer scriptlerin (örn. meyve kepçesi) hangi modda olduğumuzu öğrenmesi için
    public bool IsFirstPerson => firstPerson;

    private const string RigName = "Map7_CameraRig";

    private Transform target;            // oyuncu
    private Transform lookTarget;        // oyuncunun üstündeki bakış noktası
    private Transform eyePoint;          // oyuncunun gözü
    private Renderer[] bodyRenderers;

    private CinemachineBrain brain;
    private CinemachineCamera cmFollow, cmFirst, cmOverview;
    private CinemachineOrbitalFollow orbital;
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

    // Sahnede Brain, CM_Follow, CM_FirstPerson, CM_Overview ve OverviewZone yoksa kurar; varsa dokunmaz (ayarların korunur).
    [ContextMenu("Setup Cinemachine Rig")]
    public void EnsureRig()
    {
        Camera cam = GetComponent<Camera>();

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

        // 3) CM_Follow: third person (oyuncunun etrafında mouse ile dönen)
        Transform t = root.Find("CM_Follow");
        if (t == null)
        {
            GameObject go = new GameObject("CM_Follow");
            go.transform.SetParent(root, false);
            cmFollow = go.AddComponent<CinemachineCamera>();
            cmFollow.Lens.FieldOfView = 60f;

            orbital = go.AddComponent<CinemachineOrbitalFollow>();
            orbital.OrbitStyle = CinemachineOrbitalFollow.OrbitStyles.Sphere;
            orbital.Radius = distance;
            orbital.TrackerSettings.PositionDamping = new Vector3(0.3f, 0.3f, 0.3f);
            orbital.HorizontalAxis.Value = yaw;
            orbital.HorizontalAxis.Recentering.Enabled = false;     // mouse bırakınca kendiliğinden dönmesin
            orbital.VerticalAxis.Range = new Vector2(minPitch, maxPitch);
            orbital.VerticalAxis.Center = pitch;
            orbital.VerticalAxis.Value = pitch;
            orbital.VerticalAxis.Recentering.Enabled = false;

            // Slayt: Dead Zone, Lookahead, Damping
            CinemachineRotationComposer composer = go.AddComponent<CinemachineRotationComposer>();
            composer.Damping = new Vector2(0.5f, 0.5f);
            composer.Composition.DeadZone.Enabled = true;
            composer.Composition.DeadZone.Size = new Vector2(0.1f, 0.1f);
            composer.Lookahead.Enabled = true;
            composer.Lookahead.Time = 0.4f;
            composer.Lookahead.Smoothing = 10f;
            composer.Lookahead.IgnoreY = true;

            go.AddComponent<CinemachineImpulseListener>();           // sarsıntıyı dinler
        }
        else cmFollow = t.GetComponent<CinemachineCamera>();
        if (cmFollow != null) orbital = cmFollow.GetComponent<CinemachineOrbitalFollow>();

        // 4) CM_FirstPerson: ikinci kamera (V ile açılıp kapanır)
        t = root.Find("CM_FirstPerson");
        if (t == null)
        {
            GameObject go = new GameObject("CM_FirstPerson");
            go.transform.SetParent(root, false);
            cmFirst = go.AddComponent<CinemachineCamera>();
            cmFirst.Lens.FieldOfView = 75f;
            cmFirst.Lens.NearClipPlane = nearClip;

            go.AddComponent<CinemachineHardLockToTarget>();          // oyuncunun gözüne kilitlenir
            panTilt = go.AddComponent<CinemachinePanTilt>();         // mouse ile bakış yönü
            panTilt.PanAxis.Value = yaw;
            panTilt.PanAxis.Recentering.Enabled = false;
            panTilt.TiltAxis.Range = new Vector2(fpMinPitch, fpMaxPitch);
            panTilt.TiltAxis.Value = Mathf.Clamp(pitch, fpMinPitch, fpMaxPitch);
            panTilt.TiltAxis.Recentering.Enabled = false;

            go.AddComponent<CinemachineImpulseListener>();
            go.SetActive(firstPerson);                                // slayt: kapalı başlar, açılınca canlı olur
        }
        else cmFirst = t.GetComponent<CinemachineCamera>();
        if (cmFirst != null) panTilt = cmFirst.GetComponent<CinemachinePanTilt>();

        // 5) CM_Overview + OverviewZone: haritanın sağ alt bölümünde geniş çekim (slayt: kamera bölgesi)
        t = root.Find("CM_Overview");
        if (t == null)
        {
            GameObject go = new GameObject("CM_Overview");
            go.transform.SetParent(root, false);
            go.transform.position = new Vector3(28f, 32f, -52f);
            go.transform.rotation = Quaternion.Euler(60f, 0f, 0f);
            cmOverview = go.AddComponent<CinemachineCamera>();
            cmOverview.Lens.FieldOfView = 60f;
            go.AddComponent<CinemachineImpulseListener>();
            go.SetActive(false);                                      // bölgeye girince açılır
        }
        else cmOverview = t.GetComponent<CinemachineCamera>();

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
        // Esc: imleci bırak, sol tık: tekrar kilitle
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);
        else if (lockCursor && Cursor.lockState != CursorLockMode.Locked &&
                 Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);

        // V: first person <-> third person
        if (Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            firstPerson = !firstPerson;
            if (!firstPerson) pitch = Mathf.Clamp(pitch, minPitch, maxPitch);   // third person sınırına getir
            ApplyMode();
        }
    }

    void LateUpdate()
    {
        // Oyuncu MapGenerator tarafından oyun başında oluşturulduğu için çalışma anında bul.
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p == null) return;
            target = p.transform;
            lookTarget = FindOrCreateChild(target, "Map7_LookTarget", new Vector3(0f, lookHeight, 0f));
            eyePoint = FindOrCreateChild(target, "Map7_EyePoint", new Vector3(0f, eyeHeight, 0f));

            if (cmFollow != null) { cmFollow.Follow = lookTarget; cmFollow.LookAt = lookTarget; }
            if (cmFirst != null) { cmFirst.Follow = eyePoint; }
            ApplyMode();
        }

        // Mouse ile kamerayı döndür (sadece imleç kilitliyken)
        if (Mouse.current != null && Cursor.lockState == CursorLockMode.Locked)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            yaw += delta.x * mouseSensitivity;
            pitch -= delta.y * mouseSensitivity;
        }
        yaw = Mathf.Repeat(yaw + 180f, 360f) - 180f;   // -180..180 aralığında tut
        pitch = firstPerson ? Mathf.Clamp(pitch, fpMinPitch, fpMaxPitch) : Mathf.Clamp(pitch, minPitch, maxPitch);

        // İki kameranın eksenlerini aynı açıyla sür: V'ye basınca bakış yönü değişmez
        if (orbital != null)
        {
            orbital.HorizontalAxis.Value = yaw;
            orbital.VerticalAxis.Value = Mathf.Clamp(pitch, minPitch, maxPitch);
        }
        if (panTilt != null)
        {
            panTilt.PanAxis.Value = yaw;
            panTilt.TiltAxis.Value = Mathf.Clamp(pitch, fpMinPitch, fpMaxPitch);
        }
    }

    // Moda göre: first person kamerasını aç/kapat ve oyuncu gövdesinin görünürlüğü
    void ApplyMode()
    {
        if (cmFirst != null) cmFirst.gameObject.SetActive(firstPerson);   // açılan canlı olur, kapanınca CM_Follow döner

        if (target == null) return;
        if (bodyRenderers == null || bodyRenderers.Length == 0)
            bodyRenderers = target.GetComponentsInChildren<Renderer>(true);

        // First person'da gövde görünmez ama gölgesi kalır (kendi içini görmeyelim)
        foreach (Renderer r in bodyRenderers)
        {
            if (r == null) continue;
            r.shadowCastingMode = firstPerson
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
