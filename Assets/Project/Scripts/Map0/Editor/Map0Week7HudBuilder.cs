using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// 7. hafta: slayttaki HUD, 90 saniyelik tur, oyun sonu ve "Tekrar oyna" adımlarını Map_0'a uygular
public static class Map0Week7HudBuilder
{
    const string ScenePath = "Assets/Scenes/Map_0.unity";
    const string CanvasName = "Map0_Week7_HUD";
    const string RoundName = "Map0_Week7_Round";
    const string TmpEssentials = "Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage";

    // ipucu kartıyla aynı renkler: koyu kart, krem yazı, portakal vurgu
    static readonly Color CardBg = new Color(0.11f, 0.095f, 0.085f, 0.93f);
    static readonly Color Dim = new Color(0.06f, 0.05f, 0.045f, 0.78f);
    static readonly Color TextMain = new Color(1f, 0.96f, 0.9f);
    static readonly Color Orange = new Color(1f, 0.62f, 0.11f);
    static readonly Color Ink = new Color(0.11f, 0.095f, 0.085f);

    [MenuItem("Map0/Hafta 7 HUD ve Oyun Sonunu Kur")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Map_0")
        {
            EditorUtility.DisplayDialog("Map0", "Önce Assets/Scenes/Map_0.unity sahnesini aç.", "Tamam");
            return;
        }

        // 0) TextMeshPro'nun temel dosyaları (Assets/TextMesh Pro) yoksa bir kez içeri al
        // (TMP_Settings dosya yokken hata fırlatıyor, o yüzden önce dosyaya bakılır)
        if (AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/TextMesh Pro/Resources/TMP Settings.asset") == null)
        {
            UnityEditor.AssetPackage.Package.Import(TmpEssentials, false);
            Debug.Log("Map0: TMP Essentials içeri alınıyor (Assets/TextMesh Pro). Bitince menüyü bir kez daha çalıştır.");
            return;
        }
        TMP_FontAsset font = TMP_Settings.defaultFontAsset;

        // 1) eski kurulumu sil, menü tekrar çalışabilsin
        foreach (string old in new[] { CanvasName, RoundName })
        {
            GameObject found = FindRoot(old);
            if (found != null) Undo.DestroyObjectImmediate(found);
        }

        // 2) Canvas: ekranla büyüyüp küçülür, ipucu kartının (10) üstünde çizilir
        GameObject canvasGo = new GameObject(CanvasName, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGo, "Map0 Hafta 7");
        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        RectTransform root = (RectTransform)canvasGo.transform;

        Sprite rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        // 3) skor: sol üst köşeye çapalı kutu, solda portakal şerit
        RectTransform scorePill = Box("SkorKutu", root, new Vector2(0, 1), new Vector2(40, -40), new Vector2(280, 84), CardBg, rounded);
        Strip(scorePill, rounded);
        TextMeshProUGUI scoreText = Label("ScoreText", scorePill, font, 44, TextMain, "Skor 0", TextAlignmentOptions.MidlineLeft);
        Fill(scoreText.rectTransform, new Vector2(36, 0), new Vector2(-20, 0));

        // 4) süre: üst ortaya çapalı kutu
        RectTransform timePill = Box("SureKutu", root, new Vector2(0.5f, 1), new Vector2(0, -40), new Vector2(200, 84), CardBg, rounded);
        TextMeshProUGUI timeText = Label("TimeText", timePill, font, 48, TextMain, "1:30", TextAlignmentOptions.Midline);
        timeText.characterSpacing = 2f;
        Fill(timeText.rectTransform, Vector2.zero, Vector2.zero);

        // 5) oyun sonu paneli: ekranı karartır, ortada kart
        RectTransform panel = Rect("GameOverPanel", root, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f));
        Image dim = panel.gameObject.AddComponent<Image>();
        dim.color = Dim; // arkadaki oyuna tıklanmasın diye raycast açık kalır

        RectTransform card = Box("Kart", panel, new Vector2(0.5f, 0.5f), new Vector2(0, 0), new Vector2(760, 560), CardBg, rounded);
        card.pivot = new Vector2(0.5f, 0.5f);
        card.anchoredPosition = Vector2.zero;

        RectTransform topStrip = Rect("Serit", card, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0.5f, 1));
        topStrip.anchoredPosition = new Vector2(0, -22);
        topStrip.sizeDelta = new Vector2(-80, 6);
        Image stripImg = topStrip.gameObject.AddComponent<Image>();
        stripImg.sprite = rounded; stripImg.type = Image.Type.Sliced; stripImg.color = Orange;
        stripImg.pixelsPerUnitMultiplier = 3f;
        stripImg.raycastTarget = false;

        TextMeshProUGUI resultText = Label("ResultText", card, font, 64, TextMain, "Süre doldu!\nSkor 0\nEn iyi 0", TextAlignmentOptions.Center);
        resultText.lineSpacing = 12f;
        RectTransform resultRt = resultText.rectTransform;
        resultRt.anchorMin = resultRt.anchorMax = new Vector2(0.5f, 0.5f);
        resultRt.pivot = new Vector2(0.5f, 0.5f);
        resultRt.anchoredPosition = new Vector2(0, 60);
        resultRt.sizeDelta = new Vector2(680, 320);

        // 6) "Tekrar oyna" düğmesi: portakal, koyu yazı
        RectTransform button = Box("PlayAgainButton", card, new Vector2(0.5f, 0.5f), new Vector2(0, -180), new Vector2(420, 100), Orange, rounded);
        button.pivot = new Vector2(0.5f, 0.5f);
        button.anchoredPosition = new Vector2(0, -180);
        Image buttonImg = button.GetComponent<Image>();
        buttonImg.raycastTarget = true;
        Button btn = button.gameObject.AddComponent<Button>();
        btn.targetGraphic = buttonImg;
        ColorBlock colors = btn.colors;
        // durgunken biraz mat, üstüne gelince tam portakal, basınca koyu
        colors.normalColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
        colors.selectedColor = Color.white;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        btn.colors = colors;
        Navigation nav = btn.navigation;
        nav.mode = Navigation.Mode.None;
        btn.navigation = nav;
        TextMeshProUGUI buttonLabel = Label("Text (TMP)", button, font, 44, Ink, "Tekrar oyna", TextAlignmentOptions.Midline);
        Fill(buttonLabel.rectTransform, Vector2.zero, Vector2.zero);

        Map0GameOver gameOver = panel.gameObject.AddComponent<Map0GameOver>();
        SetObject(gameOver, "resultText", resultText);
        UnityEventTools.AddPersistentListener(btn.onClick, gameOver.Restart);
        panel.gameObject.SetActive(false); // tur bitene kadar gizli

        // 7) Round nesnesi: süre, iki yazı ve panel
        GameObject roundGo = new GameObject(RoundName);
        Undo.RegisterCreatedObjectUndo(roundGo, "Map0 Hafta 7");
        Map0Round round = roundGo.AddComponent<Map0Round>();
        SerializedObject rso = new SerializedObject(round);
        rso.FindProperty("roundLength").floatValue = 90f;
        rso.FindProperty("scoreText").objectReferenceValue = scoreText;
        rso.FindProperty("timeText").objectReferenceValue = timeText;
        rso.FindProperty("gameOver").objectReferenceValue = gameOver;
        rso.ApplyModifiedPropertiesWithoutUndo();

        // 8) yeşil çıkış puanı Round'a versin
        int zones = 0;
        foreach (Map0DeliveryZone zone in Object.FindObjectsByType<Map0DeliveryZone>(FindObjectsInactive.Include))
        {
            SetObject(zone, "round", round);
            zones++;
        }

        // 9) düğmeye tıklamayı ileten EventSystem (Input System modülüyle)
        EventSystem es = Object.FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (es == null)
        {
            GameObject esGo = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            Undo.RegisterCreatedObjectUndo(esGo, "Map0 Hafta 7");
        }
        else
        {
            StandaloneInputModule legacy = es.GetComponent<StandaloneInputModule>();
            if (legacy != null) Object.DestroyImmediate(legacy);
            if (es.GetComponent<InputSystemUIInputModule>() == null) es.gameObject.AddComponent<InputSystemUIInputModule>();
        }

        // 10) sahne listesi: yeniden yükleme için Map_0 listede olmalı
        AddToSceneList();

        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Map0 Hafta 7 kuruldu: HUD, 90 s tur, oyun sonu, Tekrar oyna. " + zones + " teslim bölgesi bağlandı. Sahneyi kaydet (Ctrl+S).");
    }

    static void AddToSceneList()
    {
        var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (EditorBuildSettingsScene s in list)
        {
            if (s.path != ScenePath) continue;
            if (!s.enabled) { s.enabled = true; EditorBuildSettings.scenes = list.ToArray(); }
            return;
        }
        list.Add(new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = list.ToArray();
    }

    // ---------- Yardımcılar ----------

    static RectTransform Box(string name, RectTransform parent, Vector2 anchor, Vector2 pos, Vector2 size, Color color, Sprite sprite)
    {
        RectTransform rt = Rect(name, parent, anchor, anchor, anchor);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        Image img = rt.gameObject.AddComponent<Image>();
        img.sprite = sprite; img.type = Image.Type.Sliced; img.color = color;
        img.raycastTarget = false;
        return rt;
    }

    static void Strip(RectTransform pill, Sprite sprite)
    {
        RectTransform s = Rect("Serit", pill, new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f));
        s.anchoredPosition = new Vector2(16, 0);
        s.sizeDelta = new Vector2(6, -36);
        Image img = s.gameObject.AddComponent<Image>();
        img.sprite = sprite; img.type = Image.Type.Sliced; img.color = Orange;
        img.pixelsPerUnitMultiplier = 3f;
        img.raycastTarget = false;
    }

    static TextMeshProUGUI Label(string name, RectTransform parent, TMP_FontAsset font, float size, Color color, string text, TextAlignmentOptions align)
    {
        RectTransform rt = Rect(name, parent, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f));
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.font = font;
        t.fontSize = size;
        t.fontStyle = FontStyles.Bold;
        t.color = color;
        t.text = text;
        t.alignment = align;
        t.textWrappingMode = TextWrappingModes.NoWrap;
        t.raycastTarget = false;
        return t;
    }

    static RectTransform Rect(string name, RectTransform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.pivot = pivot;
        rt.sizeDelta = Vector2.zero;
        return rt;
    }

    static void Fill(RectTransform rt, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }

    static void SetObject(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        so.FindProperty(field).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static GameObject FindRoot(string name)
    {
        foreach (GameObject r in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            if (r.name == name) return r;
        return null;
    }
}
