using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// Map3 W07: slayttaki Canvas, ScoreText, TimeText, GameOverPanel ve PlayAgainButton'i
// Map_3 sahnesine tek tikla kurar. Iki kez calisirsa ikinci kurulum yapmaz.
public static class M3W07HudBuilder
{
    private static readonly Color Ink = new Color32(0x1C, 0x1F, 0x26, 255);       // slayttaki yazi rengi
    private static readonly Color PanelColor = new Color32(0x1C, 0x1F, 0x26, 220); // yari saydam koyu panel
    private static readonly Color Orange = new Color32(0xFF, 0x9A, 0x1F, 255);     // portakal turuncusu buton

    [MenuItem("Tools/Map3/Hafta 7 HUD Kur")]
    public static void Run()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (EditorApplication.isPlaying || scene.name != "Map_3")
        {
            EditorUtility.DisplayDialog("Hafta 7 HUD Kur", "Play modundan çık ve Map_3 sahnesini aç.", "Tamam");
            return;
        }
        if (GameObject.Find("Map3_HUD") != null)
        {
            EditorUtility.DisplayDialog("Hafta 7 HUD Kur", "Map3_HUD zaten var, tekrar kurulmadı.", "Tamam");
            return;
        }
        if (TMP_Settings.instance == null || TMP_Settings.defaultFontAsset == null)
        {
            EditorUtility.DisplayDialog("Hafta 7 HUD Kur",
                "Önce Window > TextMeshPro > Import TMP Essential Resources yap.", "Tamam");
            return;
        }

        Undo.SetCurrentGroupName("Map3 Hafta 7 HUD Kur");
        int undoGroup = Undo.GetCurrentGroup();

        // 1. Canvas: Screen Space Overlay, 1920x1080, Match 0.5
        GameObject canvasGo = new GameObject("Map3_HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGo, "HUD");
        canvasGo.layer = LayerMask.NameToLayer("UI");
        canvasGo.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        // EventSystem: yeni Input System modulu
        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(es, "EventSystem");
        }

        // 2. ScoreText sol ust, TimeText orta ust
        TMP_Text scoreText = MakeText("ScoreText", canvasGo.transform, "Skor 0", 48, Ink, TextAlignmentOptions.Left);
        Place(scoreText.rectTransform, new Vector2(0, 1), new Vector2(40, -40), new Vector2(400, 80));

        TMP_Text timeText = MakeText("TimeText", canvasGo.transform, "1:30", 48, Ink, TextAlignmentOptions.Center);
        Place(timeText.rectTransform, new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(300, 80));

        // 5. GameOverPanel: ekrani kaplar, baslangicta gizli
        GameObject panel = new GameObject("GameOverPanel", typeof(RectTransform), typeof(Image), typeof(M3GameOver));
        panel.layer = canvasGo.layer;
        panel.transform.SetParent(canvasGo.transform, false);
        RectTransform pr = panel.GetComponent<RectTransform>();
        pr.anchorMin = Vector2.zero;
        pr.anchorMax = Vector2.one;
        pr.offsetMin = Vector2.zero;
        pr.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = PanelColor;

        TMP_Text resultText = MakeText("ResultText", panel.transform, "Süre bitti!\nSkor 0\nEn iyi 0", 72, Color.white, TextAlignmentOptions.Center);
        Place(resultText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0, 80), new Vector2(1000, 400));

        // 6. PlayAgainButton -> M3GameOver.Restart
        GameObject button = new GameObject("PlayAgainButton", typeof(RectTransform), typeof(Image), typeof(Button));
        button.layer = canvasGo.layer;
        button.transform.SetParent(panel.transform, false);
        Place(button.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0, -200), new Vector2(420, 110));
        button.GetComponent<Image>().color = Orange;
        MakeText("Text (TMP)", button.transform, "Tekrar Oyna", 48, Ink, TextAlignmentOptions.Center)
            .rectTransform.sizeDelta = new Vector2(420, 110);

        M3GameOver gameOver = panel.GetComponent<M3GameOver>();
        UnityEventTools.AddPersistentListener(button.GetComponent<Button>().onClick, gameOver.Restart);
        SetRef(gameOver, "resultText", resultText);
        panel.SetActive(false);

        // 3. M3Round: Canvas uzerinde, yazilar ve panel bagli
        M3Round round = canvasGo.AddComponent<M3Round>();
        SetRef(round, "scoreText", scoreText);
        SetRef(round, "timeText", timeText);
        SetRef(round, "gameOver", gameOver);

        // 4. teslim alani puani M3Round'a versin
        M3DeliveryZone zone = Object.FindAnyObjectByType<M3DeliveryZone>();
        if (zone != null) SetRef(zone, "round", round);
        else Debug.LogError("Sahnede M3DeliveryZone bulunamadı, Round alanını elle bağla.");

        // Restart icin sahne Build Profiles listesinde olmali
        bool inBuild = false;
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            if (s.path == scene.path) inBuild = true;
        if (!inBuild) Debug.LogWarning("Map_3 Build Profiles sahne listesinde yok: Tekrar Oyna için File > Build Profiles > Scene List'e ekle.");

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = canvasGo;
        Debug.Log("Hafta 7 HUD kuruldu. Sahneyi Ctrl+S ile kaydet.");
    }

    private static TMP_Text MakeText(string name, Transform parent, string text, float size, Color color, TextAlignmentOptions align)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.layer = parent.gameObject.layer;
        go.transform.SetParent(parent, false);
        TextMeshProUGUI t = go.GetComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.raycastTarget = false;
        return t;
    }

    // anchor ve pivot ayni nokta (Shift+Alt gibi)
    private static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    private static void SetRef(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        so.FindProperty(field).objectReferenceValue = value;
        so.ApplyModifiedProperties();
    }
}
