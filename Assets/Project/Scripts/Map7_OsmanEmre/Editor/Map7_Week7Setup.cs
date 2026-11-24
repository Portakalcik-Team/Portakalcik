#if UNITY_EDITOR
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Map 7 · Hafta 7: Canvas HUD, Map7_Round, GameOverPanel, Tekrar oyna butonu ve Scene List kurulumu.
// İki kez çalıştırılsa da mevcut objeleri bozmaz.
public static class Map7_Week7Setup
{
    private const string OutlineMatPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Outline.mat";
    private static readonly Color DarkText = new Color32(0x1C, 0x1F, 0x26, 255);

    [MenuItem("Tools/Map7/Hafta 7 Kur")]
    public static void Run()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (EditorApplication.isPlaying || scene.name != "Map_7")
        {
            EditorUtility.DisplayDialog("Map7 Hafta 7 Kur", "Play modundan çık ve Map_7 sahnesini aç.", "Tamam");
            return;
        }

        if (Resources.Load<TMP_Settings>("TMP Settings") == null)
        {
            EditorUtility.DisplayDialog("Map7 Hafta 7 Kur",
                "TextMeshPro Essentials eksik.\n\nWindow -> TextMeshPro -> Import TMP Essential Resources -> Import\n\nSonra bu menüyü tekrar çalıştır.",
                "Tamam");
            return;
        }

        Undo.SetCurrentGroupName("Map7 Hafta 7 Kur");
        int undoGroup = Undo.GetCurrentGroup();

        // 1) Canvas + EventSystem
        Canvas canvas = MakeCanvas();
        MakeEventSystem();

        // 2) Skor (sol üst) ve Süre (üst orta) yazıları
        TMP_Text scoreText = MakeHudText(canvas.transform, "ScoreText", "Skor 0",
            new Vector2(0f, 1f), new Vector2(40f, -40f), new Vector2(400f, 80f), TextAlignmentOptions.MidlineLeft);
        TMP_Text timeText = MakeHudText(canvas.transform, "TimeText", "1:30",
            new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(300f, 80f), TextAlignmentOptions.Center);

        // 3) GameOverPanel + ResultText + Map7_GameOver
        GameObject panel = MakePanel(canvas.transform);
        TMP_Text resultText = MakeResultText(panel.transform);
        Map7_GameOver gameOver = panel.GetComponent<Map7_GameOver>();
        if (gameOver == null) gameOver = Undo.AddComponent<Map7_GameOver>(panel);
        SetRef(gameOver, "resultText", resultText);

        // 4) Tekrar oyna butonu -> Map7_GameOver.Restart
        MakePlayAgainButton(panel.transform, gameOver);

        // 5) Map7_Round objesi ve slotları
        Map7_Round round = MakeRound(scoreText, timeText, gameOver);

        // 6) Çıkış prefab'ında Map7_ExitZone yoksa ekle (sahnedeki olası Exit objesine de bağla)
        const string exitPrefabPath = "Assets/Project/Art/Prefabs/Map7_OsmanEmre/Exit_Prefab.prefab";
        GameObject exitPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(exitPrefabPath);
        if (exitPrefab != null && exitPrefab.GetComponent<Map7_ExitZone>() == null)
        {
            GameObject root = PrefabUtility.LoadPrefabContents(exitPrefabPath);
            try
            {
                if (root.GetComponent<Map7_ExitZone>() == null)
                {
                    root.AddComponent<Map7_ExitZone>();
                    PrefabUtility.SaveAsPrefabAsset(root, exitPrefabPath);
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        foreach (Map7_ExitZone ez in Object.FindObjectsByType<Map7_ExitZone>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            SetRef(ez, "round", round);

        // 7) Panel tur bitene kadar gizli
        panel.SetActive(false);

        // 8) Map_7 Scene List'te olmalı
        AddSceneToBuildList(scene.path);

        Undo.CollapseUndoOperations(undoGroup);
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[Map7] Hafta 7 kurulumu tamamlandı. Sahneyi Ctrl+S ile kaydet.");
    }

    static Canvas MakeCanvas()
    {
        Canvas canvas = null;
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            if (root.name == "Canvas" && root.GetComponent<Canvas>() != null) canvas = root.GetComponent<Canvas>();

        if (canvas == null)
        {
            GameObject go = new GameObject("Canvas", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(go, "Map7 Canvas");
            go.layer = LayerMask.NameToLayer("UI");
            canvas = go.AddComponent<Canvas>();
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    static void MakeEventSystem()
    {
        EventSystem es = Object.FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (es == null)
        {
            GameObject go = new GameObject("EventSystem");
            Undo.RegisterCreatedObjectUndo(go, "Map7 EventSystem");
            es = go.AddComponent<EventSystem>();
        }

        StandaloneInputModule old = es.GetComponent<StandaloneInputModule>();
        if (old != null) Undo.DestroyObjectImmediate(old);
        if (es.GetComponent<InputSystemUIInputModule>() == null) Undo.AddComponent<InputSystemUIInputModule>(es.gameObject);
    }

    static GameObject FindOrMakeChild(Transform parent, string name, out bool created)
    {
        Transform t = parent.Find(name);
        created = t == null;
        if (!created) return t.gameObject;

        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, "Map7 " + name);
        return go;
    }

    static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static TMP_Text MakeHudText(Transform canvas, string name, string text,
        Vector2 anchor, Vector2 pos, Vector2 size, TextAlignmentOptions align)
    {
        GameObject go = FindOrMakeChild(canvas, name, out bool created);
        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = go.AddComponent<TextMeshProUGUI>();
        if (!created) return tmp;

        Place((RectTransform)go.transform, anchor, pos, size);
        tmp.text = text;
        tmp.fontSize = 48;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = align;
        tmp.raycastTarget = false;
        tmp.color = Color.white;
        Material outline = AssetDatabase.LoadAssetAtPath<Material>(OutlineMatPath);
        if (outline != null) tmp.fontSharedMaterial = outline;
        return tmp;
    }

    static GameObject MakePanel(Transform canvas)
    {
        GameObject go = FindOrMakeChild(canvas, "GameOverPanel", out bool created);
        if (!created) return go;

        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = go.AddComponent<Image>();
        img.color = new Color32(0, 0, 0, 200);
        return go;
    }

    static TMP_Text MakeResultText(Transform panel)
    {
        GameObject go = FindOrMakeChild(panel, "ResultText", out bool created);
        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = go.AddComponent<TextMeshProUGUI>();
        if (!created) return tmp;

        Place((RectTransform)go.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, 60f), new Vector2(900f, 360f));
        tmp.text = "Süre doldu!\nSkor 0\nEn iyi 0";
        tmp.fontSize = 72;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.raycastTarget = false;
        return tmp;
    }

    static void MakePlayAgainButton(Transform panel, Map7_GameOver gameOver)
    {
        GameObject go = FindOrMakeChild(panel, "PlayAgainButton", out bool created);
        if (created)
        {
            Place((RectTransform)go.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -260f), new Vector2(420f, 100f));
            Image img = go.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            go.AddComponent<Button>();

            GameObject label = FindOrMakeChild(go.transform, "Text (TMP)", out _);
            RectTransform lrt = (RectTransform)label.transform;
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = Vector2.zero;
            lrt.offsetMax = Vector2.zero;
            TextMeshProUGUI tmp = label.AddComponent<TextMeshProUGUI>();
            tmp.text = "Tekrar oyna";
            tmp.fontSize = 44;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = DarkText;
            tmp.raycastTarget = false;
        }

        Button button = go.GetComponent<Button>();
        if (button.onClick.GetPersistentEventCount() == 0)
        {
            UnityEventTools.AddPersistentListener(button.onClick, gameOver.Restart);
            EditorUtility.SetDirty(button);
        }
    }

    static Map7_Round MakeRound(TMP_Text scoreText, TMP_Text timeText, Map7_GameOver gameOver)
    {
        Map7_Round round = Object.FindAnyObjectByType<Map7_Round>(FindObjectsInactive.Include);
        if (round == null)
        {
            GameObject go = new GameObject("Map7_Round");
            Undo.RegisterCreatedObjectUndo(go, "Map7 Round");
            round = go.AddComponent<Map7_Round>();
        }

        SerializedObject so = new SerializedObject(round);
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.FindProperty("timeText").objectReferenceValue = timeText;
        so.FindProperty("gameOver").objectReferenceValue = gameOver;
        so.ApplyModifiedProperties();
        return round;
    }

    static void SetRef(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        so.FindProperty(field).objectReferenceValue = value;
        so.ApplyModifiedProperties();
    }

    static void AddSceneToBuildList(string path)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (EditorBuildSettingsScene s in scenes)
        {
            if (s.path != path) continue;
            if (!s.enabled)
            {
                s.enabled = true;
                EditorBuildSettings.scenes = scenes.ToArray();
            }
            return;
        }

        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
#endif
