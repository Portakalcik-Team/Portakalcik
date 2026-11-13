using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class Map10Tree : MonoBehaviour
{
    private static readonly List<Map10Tree> allTrees = new List<Map10Tree>();

    [Header("Ağaç Meyve Ayarları")]
    [SerializeField] private int maxFruits = 5;
    [SerializeField] private float interactDistance = 3.2f;

    [Header("Görsel Renkler")]
    [SerializeField] private Color trunkColor = new Color(0.42f, 0.25f, 0.12f); // Ağaç kabuğu kahverengisi
    [SerializeField] private Color leavesColor = new Color(0.18f, 0.65f, 0.22f); // Taze yaprak yeşili
    [SerializeField] private Color fruitColor = new Color(1f, 0.52f, 0.05f);   // Olgun mandalina turuncusu

    private int currentFruits;
    private readonly List<GameObject> fruitObjects = new List<GameObject>();
    private Transform playerTransform;
    private Map10PlayerCarry playerCarry;
    private InputAction interactAction;
    private bool isPlayerNear = false;

    public static bool IsPlayerNearAnyTree(Vector3 playerPos, float radius)
    {
        for (int i = 0; i < allTrees.Count; i++)
        {
            if (allTrees[i] != null && Vector3.Distance(allTrees[i].transform.position, playerPos) <= radius)
            {
                return true;
            }
        }
        return false;
    }

    void OnEnable()
    {
        if (!allTrees.Contains(this)) allTrees.Add(this);
        SetupTree();
    }

    void OnDisable()
    {
        allTrees.Remove(this);
    }

    void Start()
    {
        SetupTree();

        if (Application.isPlaying)
        {
            // Player'ı bul
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerCarry = player.GetComponent<Map10PlayerCarry>();
                if (playerCarry == null)
                {
                    playerCarry = player.AddComponent<Map10PlayerCarry>();
                }
            }

            if (InputSystem.actions != null)
            {
                interactAction = InputSystem.actions.FindAction("Interact");
            }
        }
    }

    private void SetupTree()
    {
        currentFruits = maxFruits;

        // Orijinal sarı küp MeshRenderer'ını gizle (collider kalsın)
        MeshRenderer baseRend = GetComponent<MeshRenderer>();
        if (baseRend != null)
        {
            baseRend.enabled = false;
        }

        // Zaten oluşturulmuşsa tekrar oluşturma
        Transform existing = transform.Find("TreeVisuals");
        if (existing != null)
        {
            fruitObjects.Clear();
            for (int i = 0; i < existing.childCount; i++)
            {
                Transform child = existing.GetChild(i);
                if (child.name.StartsWith("Mandarin_"))
                {
                    fruitObjects.Add(child.gameObject);
                }
            }
            return;
        }

        // Ağaç görselini oluştur
        BuildTreeVisuals();
    }

    /// <summary>
    /// Ağaç gövdesi, yeşil tacı ve nokta nokta turuncu mandalinaları prosedürel olarak oluşturur
    /// </summary>
    private void BuildTreeVisuals()
    {
        Transform visualRoot = new GameObject("TreeVisuals").transform;
        visualRoot.SetParent(transform);
        visualRoot.localPosition = Vector3.zero;
        visualRoot.localRotation = Quaternion.identity;
        visualRoot.localScale = Vector3.one;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material trunkMat = new Material(shader) { color = trunkColor };
        Material leavesMat = new Material(shader) { color = leavesColor };
        Material fruitMat = new Material(shader) { color = fruitColor };

        // 1. Gövde (Trunk)
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(visualRoot);
        trunk.transform.localPosition = new Vector3(0f, 0f, 0f);
        trunk.transform.localScale = new Vector3(0.5f, 1.2f, 0.5f);
        DestroyCollider(trunk);
        trunk.GetComponent<Renderer>().material = trunkMat;

        // 2. Yaprak Tacı (Canopy - Ana küre ve çevre dolgunluk küreleri)
        GameObject mainCanopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        mainCanopy.name = "Leaves_Main";
        mainCanopy.transform.SetParent(visualRoot);
        mainCanopy.transform.localPosition = new Vector3(0f, 1.2f, 0f);
        mainCanopy.transform.localScale = new Vector3(1.7f, 1.4f, 1.7f);
        DestroyCollider(mainCanopy);
        mainCanopy.GetComponent<Renderer>().material = leavesMat;

        // İkincil tepe yaprak küresi (daha organik ağaç hissi)
        GameObject topCanopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        topCanopy.name = "Leaves_Top";
        topCanopy.transform.SetParent(visualRoot);
        topCanopy.transform.localPosition = new Vector3(0f, 1.8f, 0f);
        topCanopy.transform.localScale = new Vector3(1.2f, 1.0f, 1.2f);
        DestroyCollider(topCanopy);
        topCanopy.GetComponent<Renderer>().material = leavesMat;

        // 3. Mandalinalar (Tacın dış yüzeyinde asılı nokta nokta turuncu küreler)
        Vector3[] fruitOffsets = new Vector3[]
        {
            new Vector3(0.65f, 1.2f, 0.45f),
            new Vector3(-0.65f, 1.1f, 0.5f),
            new Vector3(0.55f, 1.4f, -0.6f),
            new Vector3(-0.6f, 1.3f, -0.5f),
            new Vector3(0.1f, 1.6f, 0.75f),
            new Vector3(-0.1f, 1.7f, -0.7f),
        };

        for (int i = 0; i < maxFruits; i++)
        {
            Vector3 pos = fruitOffsets[i % fruitOffsets.Length];
            GameObject fruit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fruit.name = $"Mandarin_{i}";
            fruit.transform.SetParent(visualRoot);
            fruit.transform.localPosition = pos;
            fruit.transform.localScale = Vector3.one * 0.24f; // Küçük tatlı mandalina bilyesi
            DestroyCollider(fruit);
            fruit.GetComponent<Renderer>().material = fruitMat;

            fruitObjects.Add(fruit);
        }
    }

    private void DestroyCollider(GameObject obj)
    {
        Collider col = obj.GetComponent<Collider>();
        if (col != null)
        {
            if (Application.isPlaying) Destroy(col);
            else DestroyImmediate(col);
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerCarry = player.GetComponent<Map10PlayerCarry>();
            }
            return;
        }

        float dist = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerNear = (dist <= interactDistance);

        // Oyuncu yakındayken E'ye basarsa mandalina topla
        bool ePressed = false;
        if (interactAction != null && interactAction.WasPressedThisFrame())
        {
            ePressed = true;
        }
        else if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ePressed = true;
        }

        if (isPlayerNear && ePressed)
        {
            HarvestMandarin();
        }
    }

    private void HarvestMandarin()
    {
        if (currentFruits <= 0)
        {
            Debug.Log("[Map10Tree] Bu ağaçta toplanacak mandalina kalmadı!");
            return;
        }

        currentFruits--;

        // Ağaçtaki bir meyveyi görsel olarak kaldır
        if (fruitObjects.Count > 0)
        {
            int lastIdx = fruitObjects.Count - 1;
            GameObject f = fruitObjects[lastIdx];
            fruitObjects.RemoveAt(lastIdx);
            if (f != null)
            {
                if (Application.isPlaying) Destroy(f);
                else DestroyImmediate(f);
            }
        }

        // Oyuncunun kemerine ekle
        if (playerCarry != null)
        {
            playerCarry.AddMandarin(1);
        }
    }

    void OnGUI()
    {
        // Oyuncu ağacın dibindeyken ekranda tatlı bir etkileşim yazısı göster
        if (isPlayerNear && currentFruits > 0)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 18;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = new Color(1f, 0.9f, 0.4f);

            string msg = $"[ E ] Mandalina Topla ({currentFruits} adet)";
            float width = 280f;
            float height = 45f;
            float x = (Screen.width - width) / 2f;
            float y = Screen.height - 120f;

            GUI.Box(new Rect(x, y, width, height), msg, style);
        }
        else if (isPlayerNear && currentFruits <= 0)
        {
            GUIStyle style = new GUIStyle(GUI.skin.box);
            style.fontSize = 15;
            style.normal.textColor = Color.gray;

            string msg = "Ağaçta mandalina tükendi";
            float width = 220f;
            float height = 35f;
            float x = (Screen.width - width) / 2f;
            float y = Screen.height - 120f;

            GUI.Box(new Rect(x, y, width, height), msg, style);
        }
    }
}
