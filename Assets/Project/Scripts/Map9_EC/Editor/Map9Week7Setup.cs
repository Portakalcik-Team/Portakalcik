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

// Seviye 9 · Hafta 7: HUD, Round, Game Over paneli, Tekrar oyna butonu ve Scene List — iki kez çalışsa da bozmaz
public static class Map9Week7Setup
{
    private const string OutlineMatPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Outline.mat";
    private static readonly Color DarkText = new Color32(0x1C, 0x1F, 0x26, 255);

    [MenuItem("Tools/Map9/Hafta 7 Kur")]
    public static void Run()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (EditorApplication.isPlaying || scene.name != "Map_9")
        {
            EditorUtility.DisplayDialog("Hafta 7 Kur", "Play modundan çık ve Map_9 sahnesini aç.", "Tamam");
            return;
        }

        // TMP Essentials yoksa yazılar pembe kare çıkar: önce import
        if (Resources.Load<TMP_Settings>("TMP Settings") == null)
        {
            EditorUtility.DisplayDialog("Hafta 7 Kur",
                "TextMeshPro Essentials eksik.\n\nWindow → TextMeshPro → Import TMP Essential Resources → Import\n\nSonra bu menüyü tekrar çalıştır.",
                "Tamam");
            return;
        }

        Undo.SetCurrentGroupName("Map9 Hafta 7 Kur");
        int undoGroup = Undo.GetCurrentGroup();

        // Adım 2: Canvas + EventSystem
        Canvas canvas = MakeCanvas();
        MakeEventSystem();

        // Adım 3–4: skor ve süre yazıları
        TMP_Text scoreText = MakeHudText(canvas.transform, "ScoreText", "Skor 0/5",
            new Vector2(0f, 1f), new Vector2(40f, -40f), new Vector2(400f, 80f), TextAlignmentOptions.MidlineLeft);
        TMP_Text timeText = MakeHudText(canvas.transform, "TimeText", "1:30",
            new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(300f, 80f), TextAlignmentOptions.Center);

        // Adım 9–10: Game Over paneli, sonuç yazısı, Map9GameOver
        GameObject panel = MakePanel(canvas.transform);
        TMP_Text resultText = MakeResultText(panel.transform);
        Map9GameOver gameOver = panel.GetComponent<Map9GameOver>();
        if (gameOver == null) gameOver = Undo.AddComponent<Map9GameOver>(panel);
        SetRef(gameOver, "resultText", resultText);

        // Adım 14–15: Tekrar oyna butonu → Map9GameOver.Restart
        MakePlayAgainButton(panel.transform, gameOver);

        // Adım 7 + 12: Round objesi ve slotları
        Map9Round round = MakeRound(scoreText, timeText, gameOver);

        // Adım 6: çıkış Round'a ödesin
        Map9ExitZone exit = Object.FindAnyObjectByType<Map9ExitZone>();
        if (exit != null) SetRef(exit, "round", round);
        else Debug.LogError("Sahnede Map9ExitZone yok: Round slotu bağlanamadı.");

        // Adım 12: panel tur bitene kadar gizli
        panel.SetActive(false);

        // Adım 17: Map_9 Scene List'te olmalı, yoksa LoadScene hata verir
        AddSceneToBuildList(scene.path);

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Hafta 7 kurulumu bitti. Sahneyi Ctrl+S ile kaydet.");
    }

    // ---------- Canvas / EventSystem ----------

    private static Canvas MakeCanvas()
    {
        Canvas canvas = null;
        foreach (GameObject root in SceneManager.GetActiveScene().GetRootGameObjects())
            if (root.name == "Canvas" && root.GetComponent<Canvas>() != null) canvas = root.GetComponent<Canvas>();

        if (canvas == null)
        {
            GameObject go = new GameObject("Canvas", typeof(RectTransform));
            Undo.RegisterCreatedObjectUndo(go, "Map9 Canvas");
            go.layer = LayerMask.NameToLayer("UI");
            canvas = go.AddComponent<Canvas>();
            go.AddComponent<CanvasScaler>();
            go.AddComponent<GraphicRaycaster>();
            Debug.Log("Canvas oluşturuldu.");
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        return canvas;
    }

    private static void MakeEventSystem()
    {
        EventSystem es = Object.FindAnyObjectByType<EventSystem>(FindObjectsInactive.Include);
        if (es == null)
        {
            GameObject go = new GameObject("EventSystem");
            Undo.RegisterCreatedObjectUndo(go, "Map9 EventSystem");
            es = go.AddComponent<EventSystem>();
            Debug.Log("EventSystem oluşturuldu.");
        }

        // yeni Input System: eski Standalone modülü yerine InputSystemUIInputModule
        StandaloneInputModule old = es.GetComponent<StandaloneInputModule>();
        if (old != null) Undo.DestroyObjectImmediate(old);
        if (es.GetComponent<InputSystemUIInputModule>() == null) Undo.AddComponent<InputSystemUIInputModule>(es.gameObject);
    }

    // ---------- yazılar ----------

    // child'ı bul (gizli olsa da), yoksa RectTransform'lu yeni obje yap
    private static GameObject FindOrMakeChild(Transform parent, string name, out bool created)
    {
        Transform t = parent.Find(name);
        created = t == null;
        if (!created) return t.gameObject;

        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);
        Undo.RegisterCreatedObjectUndo(go, "Map9 " + name);
        return go;
    }

    private static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = anchor;        // Shift: pivot da köşede
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    private static TMP_Text MakeHudText(Transform canvas, string name, string text,
        Vector2 anchor, Vector2 pos, Vector2 size, TextAlignmentOptions align)
    {
        GameObject go = FindOrMakeChild(canvas, name, out bool created);
        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = go.AddComponent<TextMeshProUGUI>();
        if (!created) return tmp;   // elle yapılan ayarları ezme

        Place((RectTransform)go.transform, anchor, pos, size);
        tmp.text = text;
        tmp.fontSize = 56;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = align;
        tmp.raycastTarget = false;

        // birinci şahıs: arka plan taş zemin, duvar, gökyüzü… beyaz yazı + koyu kontur her yerde okunur
        tmp.color = Color.white;
        Material outline = AssetDatabase.LoadAssetAtPath<Material>(OutlineMatPath);
        if (outline != null) tmp.fontSharedMaterial = outline;

        Debug.Log(name + " oluşturuldu.");
        return tmp;
    }

    // ---------- Game Over paneli ----------

    private static GameObject MakePanel(Transform canvas)
    {
        GameObject go = FindOrMakeChild(canvas, "GameOverPanel", out bool created);
        if (!created) return go;

        // tüm ekranı kaplar
        RectTransform rt = (RectTransform)go.transform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = go.AddComponent<Image>();
        img.color = new Color32(0, 0, 0, 200);   // yarı saydam siyah
        Debug.Log("GameOverPanel oluşturuldu.");
        return go;
    }

    private static TMP_Text MakeResultText(Transform panel)
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

    private static void MakePlayAgainButton(Transform panel, Map9GameOver gameOver)
    {
        GameObject go = FindOrMakeChild(panel, "PlayAgainButton", out bool created);
        if (created)
        {
            Place((RectTransform)go.transform, new Vector2(0.5f, 0.5f), new Vector2(0f, -260f), new Vector2(420f, 100f));
            Image img = go.AddComponent<Image>();
            img.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
            img.type = Image.Type.Sliced;
            go.AddComponent<Button>();

            // butonun yazısı: butonu baştan sona kaplar
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
            Debug.Log("PlayAgainButton oluşturuldu.");
        }

        // On Click () → Map9GameOver.Restart — bir kez
        Button button = go.GetComponent<Button>();
        if (button.onClick.GetPersistentEventCount() == 0)
        {
            UnityEventTools.AddPersistentListener(button.onClick, gameOver.Restart);
            EditorUtility.SetDirty(button);
        }
    }

    // ---------- Round ----------

    private static Map9Round MakeRound(TMP_Text scoreText, TMP_Text timeText, Map9GameOver gameOver)
    {
        Map9Round round = Object.FindAnyObjectByType<Map9Round>(FindObjectsInactive.Include);
        if (round == null)
        {
            GameObject go = new GameObject("Round_09");
            Undo.RegisterCreatedObjectUndo(go, "Map9 Round");
            round = go.AddComponent<Map9Round>();
            Debug.Log("Round_09 oluşturuldu.");
        }

        // alanlar private: SerializedObject ile yazılır
        SerializedObject so = new SerializedObject(round);
        so.FindProperty("scoreText").objectReferenceValue = scoreText;
        so.FindProperty("timeText").objectReferenceValue = timeText;
        so.FindProperty("gameOver").objectReferenceValue = gameOver;
        so.ApplyModifiedProperties();
        return round;
    }

    private static void SetRef(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        so.FindProperty(field).objectReferenceValue = value;
        so.ApplyModifiedProperties();
    }

    // ---------- Scene List ----------

    private static void AddSceneToBuildList(string path)
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (EditorBuildSettingsScene s in scenes)
        {
            if (s.path != path) continue;
            if (!s.enabled)
            {
                s.enabled = true;
                EditorBuildSettings.scenes = scenes.ToArray();
                Debug.Log("Scene List: " + path + " açıldı.");
            }
            return;
        }

        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("Scene List: " + path + " eklendi.");
    }
}
