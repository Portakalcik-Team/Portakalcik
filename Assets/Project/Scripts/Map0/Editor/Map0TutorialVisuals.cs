using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

// Eğitimin görsel parçaları: canavar modelleri, kaideler, yer işaretleri ve ipucu kartı.
// Hepsi Unity'nin hazır şekillerinden (küp, küre, silindir) yapılır, collider'ları silinir.
public static class Map0TutorialVisuals
{
    const string MaterialFolder = "Assets/Project/Art/Materials/Map0/Egitim";
    const int GroundLayer = 3;

    // ---------- Canavarlar (ileri = +Z, ayaklar y = 0) ----------

    public static Transform BuildMonster(Map0MonsterData data, Transform parent)
    {
        GameObject visual = new GameObject(parent.name + "_Visual");
        visual.transform.SetParent(parent, false);
        Transform v = visual.transform;

        Material body = Mat(data.shape + "_Govde", data.color);
        Material dark = Mat(data.shape + "_Koyu", Color.Lerp(data.color, Color.black, 0.45f));
        Material light = Mat(data.shape + "_Acik", Color.Lerp(data.color, Color.white, 0.55f));

        switch (data.shape)
        {
            case Map0MonsterShape.Bocek: Beetle(v, body, dark); break;
            case Map0MonsterShape.Yarasa: Bat(v, body, dark); break;
            case Map0MonsterShape.Tilki: Fox(v, body, dark, light); break;
            case Map0MonsterShape.Golem: Golem(v, body, dark); break;
            case Map0MonsterShape.Muhafiz: Guardian(v, body, dark); break;
        }
        return v;
    }

    static void Beetle(Transform v, Material body, Material dark)
    {
        Material eye = Mat("Goz_Kirmizi", new Color(0.95f, 0.18f, 0.15f), true);
        Material spike = Mat("Diken", new Color(0.93f, 0.89f, 0.74f));

        Part(PrimitiveType.Sphere, v, "Govde", new Vector3(0, 0.32f, -0.05f), new Vector3(0.85f, 0.45f, 1.05f), Vector3.zero, body);
        Part(PrimitiveType.Sphere, v, "Bas", new Vector3(0, 0.3f, 0.5f), new Vector3(0.45f, 0.32f, 0.38f), Vector3.zero, dark);
        Part(PrimitiveType.Sphere, v, "Goz_L", new Vector3(-0.11f, 0.36f, 0.66f), Vector3.one * 0.09f, Vector3.zero, eye);
        Part(PrimitiveType.Sphere, v, "Goz_R", new Vector3(0.11f, 0.36f, 0.66f), Vector3.one * 0.09f, Vector3.zero, eye);

        // sırttaki dikenler: iki sıra, hafif dışa yatık
        for (int i = 0; i < 3; i++)
        {
            float z = 0.22f - i * 0.28f;
            Part(PrimitiveType.Cube, v, "Diken_L" + i, new Vector3(-0.17f, 0.6f, z), new Vector3(0.07f, 0.26f, 0.07f), new Vector3(0, 0, 25), spike);
            Part(PrimitiveType.Cube, v, "Diken_R" + i, new Vector3(0.17f, 0.6f, z), new Vector3(0.07f, 0.26f, 0.07f), new Vector3(0, 0, -25), spike);
        }
        // altı bacak
        for (int i = 0; i < 3; i++)
        {
            float z = 0.25f - i * 0.3f;
            Part(PrimitiveType.Cube, v, "Bacak_L" + i, new Vector3(-0.45f, 0.1f, z), new Vector3(0.3f, 0.05f, 0.05f), new Vector3(0, 0, -30), dark);
            Part(PrimitiveType.Cube, v, "Bacak_R" + i, new Vector3(0.45f, 0.1f, z), new Vector3(0.3f, 0.05f, 0.05f), new Vector3(0, 0, 30), dark);
        }
    }

    static void Bat(Transform v, Material body, Material dark)
    {
        Material eye = Mat("Goz_Sari", new Color(1f, 0.85f, 0.2f), true);

        // yarasa havada süzülür: gövde yerden yüksekte
        Part(PrimitiveType.Sphere, v, "Govde", new Vector3(0, 0.95f, 0), new Vector3(0.42f, 0.5f, 0.4f), Vector3.zero, body);
        Part(PrimitiveType.Cube, v, "Kulak_L", new Vector3(-0.11f, 1.25f, 0.02f), new Vector3(0.08f, 0.2f, 0.06f), new Vector3(0, 0, 15), dark);
        Part(PrimitiveType.Cube, v, "Kulak_R", new Vector3(0.11f, 1.25f, 0.02f), new Vector3(0.08f, 0.2f, 0.06f), new Vector3(0, 0, -15), dark);
        Part(PrimitiveType.Sphere, v, "Goz_L", new Vector3(-0.08f, 1.02f, 0.18f), Vector3.one * 0.08f, Vector3.zero, eye);
        Part(PrimitiveType.Sphere, v, "Goz_R", new Vector3(0.08f, 1.02f, 0.18f), Vector3.one * 0.08f, Vector3.zero, eye);

        // kanatlar: iç ve dış parça, yukarıdan bakınca geniş bir siluet
        Part(PrimitiveType.Cube, v, "Kanat_L_Ic", new Vector3(-0.38f, 1.0f, -0.02f), new Vector3(0.42f, 0.04f, 0.42f), new Vector3(0, 0, 18), dark);
        Part(PrimitiveType.Cube, v, "Kanat_L_Dis", new Vector3(-0.74f, 1.1f, -0.08f), new Vector3(0.38f, 0.035f, 0.32f), new Vector3(0, 12, 28), dark);
        Part(PrimitiveType.Cube, v, "Kanat_R_Ic", new Vector3(0.38f, 1.0f, -0.02f), new Vector3(0.42f, 0.04f, 0.42f), new Vector3(0, 0, -18), dark);
        Part(PrimitiveType.Cube, v, "Kanat_R_Dis", new Vector3(0.74f, 1.1f, -0.08f), new Vector3(0.38f, 0.035f, 0.32f), new Vector3(0, -12, -28), dark);
    }

    static void Fox(Transform v, Material body, Material dark, Material light)
    {
        Material eye = Mat("Goz_Siyah", new Color(0.08f, 0.07f, 0.07f));

        Part(PrimitiveType.Cube, v, "Govde", new Vector3(0, 0.45f, -0.05f), new Vector3(0.42f, 0.36f, 0.85f), Vector3.zero, body);
        Part(PrimitiveType.Cube, v, "Gogus", new Vector3(0, 0.4f, 0.32f), new Vector3(0.3f, 0.26f, 0.12f), Vector3.zero, light);
        Part(PrimitiveType.Cube, v, "Bas", new Vector3(0, 0.7f, 0.48f), new Vector3(0.38f, 0.32f, 0.32f), Vector3.zero, body);
        Part(PrimitiveType.Cube, v, "Burun", new Vector3(0, 0.63f, 0.72f), new Vector3(0.17f, 0.14f, 0.2f), Vector3.zero, light);
        Part(PrimitiveType.Cube, v, "BurunUcu", new Vector3(0, 0.66f, 0.83f), Vector3.one * 0.06f, Vector3.zero, eye);
        Part(PrimitiveType.Cube, v, "Kulak_L", new Vector3(-0.12f, 0.92f, 0.45f), new Vector3(0.1f, 0.18f, 0.06f), new Vector3(0, 0, 12), dark);
        Part(PrimitiveType.Cube, v, "Kulak_R", new Vector3(0.12f, 0.92f, 0.45f), new Vector3(0.1f, 0.18f, 0.06f), new Vector3(0, 0, -12), dark);
        Part(PrimitiveType.Cube, v, "Goz_L", new Vector3(-0.1f, 0.76f, 0.645f), new Vector3(0.06f, 0.06f, 0.02f), Vector3.zero, eye);
        Part(PrimitiveType.Cube, v, "Goz_R", new Vector3(0.1f, 0.76f, 0.645f), new Vector3(0.06f, 0.06f, 0.02f), Vector3.zero, eye);

        // kabarık kuyruk, ucu beyaz
        Part(PrimitiveType.Capsule, v, "Kuyruk", new Vector3(0, 0.58f, -0.62f), new Vector3(0.22f, 0.28f, 0.22f), new Vector3(-55, 0, 0), body);
        Part(PrimitiveType.Sphere, v, "KuyrukUcu", new Vector3(0, 0.74f, -0.84f), Vector3.one * 0.2f, Vector3.zero, light);

        for (int i = 0; i < 4; i++)
        {
            float x = i % 2 == 0 ? -0.13f : 0.13f;
            float z = i < 2 ? 0.25f : -0.35f;
            Part(PrimitiveType.Cube, v, "Bacak_" + i, new Vector3(x, 0.14f, z), new Vector3(0.1f, 0.28f, 0.1f), Vector3.zero, dark);
        }
    }

    static void Golem(Transform v, Material body, Material dark)
    {
        Material moss = Mat("Yosun", new Color(0.36f, 0.55f, 0.24f));
        Material eye = Mat("Goz_Kehribar", new Color(1f, 0.62f, 0.15f), true);

        Part(PrimitiveType.Cube, v, "Bacak_L", new Vector3(-0.22f, 0.2f, 0), new Vector3(0.3f, 0.4f, 0.32f), Vector3.zero, dark);
        Part(PrimitiveType.Cube, v, "Bacak_R", new Vector3(0.22f, 0.2f, 0), new Vector3(0.3f, 0.4f, 0.32f), Vector3.zero, dark);
        Part(PrimitiveType.Cube, v, "Govde", new Vector3(0, 0.75f, 0), new Vector3(0.85f, 0.7f, 0.6f), new Vector3(0, 0, 0), body);
        Part(PrimitiveType.Cube, v, "Bas", new Vector3(0, 1.25f, 0.06f), new Vector3(0.4f, 0.32f, 0.38f), new Vector3(0, 0, 4), body);
        Part(PrimitiveType.Cube, v, "Goz_L", new Vector3(-0.09f, 1.27f, 0.255f), new Vector3(0.08f, 0.05f, 0.02f), Vector3.zero, eye);
        Part(PrimitiveType.Cube, v, "Goz_R", new Vector3(0.09f, 1.27f, 0.255f), new Vector3(0.08f, 0.05f, 0.02f), Vector3.zero, eye);

        // uzun kollar yere kadar sarkar
        Part(PrimitiveType.Cube, v, "Kol_L", new Vector3(-0.58f, 0.6f, 0.05f), new Vector3(0.28f, 0.8f, 0.3f), new Vector3(0, 0, -6), dark);
        Part(PrimitiveType.Cube, v, "Kol_R", new Vector3(0.58f, 0.6f, 0.05f), new Vector3(0.28f, 0.8f, 0.3f), new Vector3(0, 0, 6), dark);

        // omuz ve sırtta yosun: yukarıdan bakınca yeşil lekeler
        Part(PrimitiveType.Cube, v, "Yosun_Omuz_L", new Vector3(-0.3f, 1.12f, 0), new Vector3(0.3f, 0.06f, 0.5f), Vector3.zero, moss);
        Part(PrimitiveType.Cube, v, "Yosun_Omuz_R", new Vector3(0.34f, 1.12f, -0.05f), new Vector3(0.22f, 0.06f, 0.4f), Vector3.zero, moss);
        Part(PrimitiveType.Cube, v, "Yosun_Bas", new Vector3(0.04f, 1.42f, 0.02f), new Vector3(0.24f, 0.04f, 0.22f), Vector3.zero, moss);
    }

    static void Guardian(Transform v, Material body, Material dark)
    {
        Material crest = Mat("Sorguc", new Color(0.8f, 0.16f, 0.16f));
        Material visor = Mat("Vizor", new Color(0.05f, 0.05f, 0.08f));
        Material blade = Mat("Kilic", new Color(0.88f, 0.9f, 0.95f));
        Material gold = Mat("Altin", new Color(0.92f, 0.72f, 0.25f));

        Part(PrimitiveType.Cube, v, "Bacak_L", new Vector3(-0.14f, 0.3f, 0), new Vector3(0.18f, 0.6f, 0.2f), Vector3.zero, dark);
        Part(PrimitiveType.Cube, v, "Bacak_R", new Vector3(0.14f, 0.3f, 0), new Vector3(0.18f, 0.6f, 0.2f), Vector3.zero, dark);
        Part(PrimitiveType.Cube, v, "Govde", new Vector3(0, 0.95f, 0), new Vector3(0.55f, 0.7f, 0.35f), Vector3.zero, body);
        Part(PrimitiveType.Cube, v, "Kemer", new Vector3(0, 0.65f, 0), new Vector3(0.58f, 0.08f, 0.38f), Vector3.zero, gold);
        Part(PrimitiveType.Cube, v, "Omuz_L", new Vector3(-0.36f, 1.25f, 0), new Vector3(0.22f, 0.12f, 0.32f), new Vector3(0, 0, 12), body);
        Part(PrimitiveType.Cube, v, "Omuz_R", new Vector3(0.36f, 1.25f, 0), new Vector3(0.22f, 0.12f, 0.32f), new Vector3(0, 0, -12), body);
        Part(PrimitiveType.Cube, v, "Migfer", new Vector3(0, 1.52f, 0), new Vector3(0.36f, 0.38f, 0.36f), Vector3.zero, body);
        Part(PrimitiveType.Cube, v, "Vizor", new Vector3(0, 1.54f, 0.175f), new Vector3(0.26f, 0.06f, 0.02f), Vector3.zero, visor);
        Part(PrimitiveType.Cube, v, "Sorguc", new Vector3(0, 1.78f, -0.02f), new Vector3(0.07f, 0.16f, 0.4f), Vector3.zero, crest);

        // sol kolda kalkan, sağ elde kılıç
        Part(PrimitiveType.Cube, v, "Kalkan", new Vector3(-0.45f, 0.95f, 0.12f), new Vector3(0.07f, 0.65f, 0.5f), new Vector3(0, -15, 0), dark);
        Part(PrimitiveType.Cube, v, "KalkanAmblem", new Vector3(-0.49f, 0.95f, 0.13f), new Vector3(0.02f, 0.22f, 0.22f), new Vector3(45, -15, 0), gold);
        Part(PrimitiveType.Cube, v, "Kabza", new Vector3(0.42f, 0.8f, 0.18f), new Vector3(0.22f, 0.05f, 0.06f), Vector3.zero, gold);
        Part(PrimitiveType.Cube, v, "Kilic", new Vector3(0.42f, 1.22f, 0.18f), new Vector3(0.05f, 0.8f, 0.1f), Vector3.zero, blade);
    }

    // ---------- Kaide ve yer işareti ----------

    public static GameObject BuildPedestal(Transform parent, Vector3 worldPos, Color accent)
    {
        int index = 1;
        foreach (Transform child in parent)
            if (child.name.StartsWith("Map0_Egitim_Kaide_")) index++;

        GameObject root = new GameObject("Map0_Egitim_Kaide_" + index.ToString("00"));
        root.transform.SetParent(parent, false);
        root.transform.position = worldPos;

        // taş gövde: üstüne basılabilir, bu yüzden collider kalır ve Ground katmanında
        GameObject stone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stone.name = root.name + "_Tas";
        stone.layer = GroundLayer;
        stone.transform.SetParent(root.transform, false);
        stone.transform.localPosition = new Vector3(0, 0.25f, 0);
        stone.transform.localScale = new Vector3(1.1f, 0.25f, 1.1f);
        stone.GetComponent<Renderer>().sharedMaterial = Mat("Kaide_Tas", new Color(0.42f, 0.4f, 0.38f));

        Part(PrimitiveType.Cylinder, root.transform, root.name + "_Halka", new Vector3(0, 0.4f, 0), new Vector3(1.18f, 0.03f, 1.18f), Vector3.zero, Mat("Kaide_" + ColorUtility.ToHtmlStringRGB(accent), accent));
        Part(PrimitiveType.Cylinder, root.transform, root.name + "_Ust", new Vector3(0, 0.5f, 0), new Vector3(1.0f, 0.01f, 1.0f), Vector3.zero, Mat("Kaide_Ust", new Color(0.62f, 0.6f, 0.56f)));
        return root;
    }

    public static void BuildHintMarker(Transform zone)
    {
        // alanın ortasında yerde turuncu halka, üstünde süzülen bilgi taşı
        GameObject marker = new GameObject(zone.name + "_Isaret");
        marker.transform.SetParent(zone, false);
        marker.transform.position = new Vector3(zone.position.x, 0f, zone.position.z);

        Material orange = Mat("Ipucu_Turuncu", new Color(1f, 0.62f, 0.11f), true);
        Material inner = Mat("Ipucu_Ic", new Color(0.32f, 0.3f, 0.29f));
        Part(PrimitiveType.Cylinder, marker.transform, "Halka", new Vector3(0, 0.012f, 0), new Vector3(1.1f, 0.01f, 1.1f), Vector3.zero, orange);
        Part(PrimitiveType.Cylinder, marker.transform, "HalkaIc", new Vector3(0, 0.022f, 0), new Vector3(0.85f, 0.01f, 0.85f), Vector3.zero, inner);

        GameObject gemRoot = new GameObject("BilgiTasi");
        gemRoot.transform.SetParent(marker.transform, false);
        gemRoot.transform.localPosition = new Vector3(0, 2.3f, 0);
        Part(PrimitiveType.Cube, gemRoot.transform, "Tas", Vector3.zero, Vector3.one * 0.35f, new Vector3(45, 0, 45), orange);
        gemRoot.AddComponent<Map0Spinner>();
        gemRoot.AddComponent<Map0Bobber>();
    }

    // ---------- İpucu kartı (UI) ----------

    static readonly Color CardBg = new Color(0.11f, 0.095f, 0.085f, 0.93f);
    static readonly Color ChipBg = new Color(1f, 1f, 1f, 0.06f);
    static readonly Color TextMain = new Color(1f, 0.96f, 0.9f);
    static readonly Color TextMuted = new Color(1f, 0.96f, 0.9f, 0.55f);
    static readonly Color Orange = new Color(1f, 0.62f, 0.11f);

    public static GameObject BuildHintCanvas(Transform parent)
    {
        GameObject canvasGo = new GameObject("Map0_Egitim_UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasGo.transform.SetParent(parent, false);
        Canvas canvas = canvasGo.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        RectTransform canvasRt = (RectTransform)canvasGo.transform;

        Sprite rounded = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        // --- Kart: alt orta, içerik kadar uzar ---
        RectTransform card = Rect("Kart", canvasRt, new Vector2(0.5f, 0), new Vector2(0.5f, 0), new Vector2(0.5f, 0));
        card.anchoredPosition = new Vector2(0, 44);
        card.sizeDelta = new Vector2(820, 0);
        Image cardImg = card.gameObject.AddComponent<Image>();
        cardImg.sprite = rounded; cardImg.type = Image.Type.Sliced; cardImg.color = CardBg;
        cardImg.raycastTarget = false;
        CanvasGroup group = card.gameObject.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false; group.interactable = false;
        VerticalLayoutGroup cardLayout = card.gameObject.AddComponent<VerticalLayoutGroup>();
        cardLayout.padding = new RectOffset(40, 32, 24, 26);
        cardLayout.spacing = 6;
        cardLayout.childControlWidth = true; cardLayout.childControlHeight = true;
        cardLayout.childForceExpandWidth = true; cardLayout.childForceExpandHeight = false;
        ContentSizeFitter fit = card.gameObject.AddComponent<ContentSizeFitter>();
        fit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // sol kenarda renkli şerit (layout dışı)
        RectTransform accent = Rect("Serit", card, new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0.5f));
        accent.anchoredPosition = new Vector2(16, 0);
        accent.sizeDelta = new Vector2(6, -36);
        Image accentImg = accent.gameObject.AddComponent<Image>();
        accentImg.sprite = rounded; accentImg.type = Image.Type.Sliced; accentImg.color = Orange;
        accentImg.pixelsPerUnitMultiplier = 3f; // ince şeritte köşe küçük olsun
        accent.gameObject.AddComponent<LayoutElement>().ignoreLayout = true;

        Text step = Label("AdimEtiketi", card, 15, FontStyle.Bold, TextMuted, "ADIM 1");
        Text title = Label("Baslik", card, 34, FontStyle.Bold, TextMain, "Başlık");
        Text body = Label("Metin", card, 22, FontStyle.Normal, new Color(1f, 0.96f, 0.9f, 0.85f), "Açıklama");
        body.lineSpacing = 1.1f;

        // --- Canavar bloğu: 4 değer kutusu + ipucu ---
        RectTransform monster = Rect("CanavarBlok", card, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f));
        VerticalLayoutGroup monsterLayout = monster.gameObject.AddComponent<VerticalLayoutGroup>();
        monsterLayout.padding = new RectOffset(0, 0, 10, 0);
        monsterLayout.spacing = 12;
        monsterLayout.childControlWidth = true; monsterLayout.childControlHeight = true;
        monsterLayout.childForceExpandWidth = true; monsterLayout.childForceExpandHeight = false;

        RectTransform row = Rect("Degerler", monster, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f));
        HorizontalLayoutGroup rowLayout = row.gameObject.AddComponent<HorizontalLayoutGroup>();
        rowLayout.spacing = 10;
        rowLayout.childControlWidth = true; rowLayout.childControlHeight = true;
        rowLayout.childForceExpandWidth = true; rowLayout.childForceExpandHeight = true;
        row.gameObject.AddComponent<LayoutElement>().preferredHeight = 74;

        string[] labels = { "HIZ", "CAN", "HASAR", "MANDALİNA" };
        Text[] values = new Text[4];
        for (int i = 0; i < 4; i++)
            values[i] = Chip(row, labels[i], rounded);

        Text tip = Label("Ipucu", monster, 19, FontStyle.Italic, TextMuted, "İpucu");

        // --- İlerleme: sağ üst ---
        RectTransform pill = Rect("Ilerleme", canvasRt, Vector2.one, Vector2.one, Vector2.one);
        pill.anchoredPosition = new Vector2(-32, -32);
        pill.sizeDelta = new Vector2(240, 64);
        Image pillImg = pill.gameObject.AddComponent<Image>();
        pillImg.sprite = rounded; pillImg.type = Image.Type.Sliced; pillImg.color = CardBg;
        pillImg.raycastTarget = false;

        Text pillLabel = Label("Etiket", pill, 14, FontStyle.Bold, TextMuted, "EĞİTİM");
        Place(pillLabel.rectTransform, new Vector2(0, 0.5f), new Vector2(0.5f, 1), new Vector2(18, -2), new Vector2(0, -8));
        pillLabel.alignment = TextAnchor.MiddleLeft;
        Text pillValue = Label("Deger", pill, 22, FontStyle.Bold, TextMain, "0 / 0");
        Place(pillValue.rectTransform, new Vector2(0.4f, 0.5f), new Vector2(1, 1), new Vector2(0, -2), new Vector2(-18, -8));
        pillValue.alignment = TextAnchor.MiddleRight;

        RectTransform track = Rect("Cubuk", pill, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0.5f, 0));
        track.anchoredPosition = new Vector2(0, 14);
        track.sizeDelta = new Vector2(-36, 6);
        Image trackImg = track.gameObject.AddComponent<Image>();
        trackImg.sprite = rounded; trackImg.type = Image.Type.Sliced; trackImg.color = ChipBg;
        trackImg.pixelsPerUnitMultiplier = 3f;
        RectTransform fill = Rect("Dolgu", track, Vector2.zero, new Vector2(0, 1), new Vector2(0, 0.5f));
        fill.offsetMin = Vector2.zero; fill.offsetMax = Vector2.zero;
        Image fillImg = fill.gameObject.AddComponent<Image>();
        fillImg.sprite = rounded; fillImg.type = Image.Type.Sliced; fillImg.color = Orange;
        fillImg.pixelsPerUnitMultiplier = 3f;

        // --- Paneli bağla ---
        Map0HintPanel panel = canvasGo.AddComponent<Map0HintPanel>();
        SerializedObject so = new SerializedObject(panel);
        so.FindProperty("cardGroup").objectReferenceValue = group;
        so.FindProperty("card").objectReferenceValue = card;
        so.FindProperty("accentBar").objectReferenceValue = accentImg;
        so.FindProperty("stepLabel").objectReferenceValue = step;
        so.FindProperty("titleText").objectReferenceValue = title;
        so.FindProperty("bodyText").objectReferenceValue = body;
        so.FindProperty("monsterBlock").objectReferenceValue = monster.gameObject;
        SerializedProperty stats = so.FindProperty("statValues");
        stats.arraySize = 4;
        for (int i = 0; i < 4; i++) stats.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        so.FindProperty("tipText").objectReferenceValue = tip;
        so.FindProperty("progressText").objectReferenceValue = pillValue;
        so.FindProperty("progressFill").objectReferenceValue = fill;
        so.FindProperty("progressFillImage").objectReferenceValue = fillImg;
        so.ApplyModifiedPropertiesWithoutUndo();

        return canvasGo;
    }

    static Text Chip(RectTransform row, string label, Sprite rounded)
    {
        RectTransform chip = Rect("Kutu_" + label, row, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f));
        Image bg = chip.gameObject.AddComponent<Image>();
        bg.sprite = rounded; bg.type = Image.Type.Sliced; bg.color = ChipBg;
        bg.raycastTarget = false;

        Text value = Label("Deger", chip, 30, FontStyle.Bold, Orange, "0");
        value.alignment = TextAnchor.MiddleCenter;
        Place(value.rectTransform, new Vector2(0, 0.38f), Vector2.one, Vector2.zero, new Vector2(0, -4));

        Text caption = Label("Etiket", chip, 12, FontStyle.Bold, TextMuted, label);
        caption.alignment = TextAnchor.MiddleCenter;
        Place(caption.rectTransform, Vector2.zero, new Vector2(1, 0.4f), new Vector2(0, 6), Vector2.zero);
        return value;
    }

    // ---------- Yardımcılar ----------

    static GameObject Part(PrimitiveType type, Transform parent, string name, Vector3 pos, Vector3 scale, Vector3 euler, Material mat)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;
        Object.DestroyImmediate(go.GetComponent<Collider>()); // sadece görsel
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.Euler(euler);
        go.transform.localScale = scale;
        Renderer r = go.GetComponent<Renderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = scale.y < 0.03f ? ShadowCastingMode.Off : ShadowCastingMode.On;
        return go;
    }

    // malzemeyi kendi klasörümüzde bul ya da oluştur (ortak malzemelere dokunmayız)
    static Material Mat(string key, Color color, bool glow = false)
    {
        if (!AssetDatabase.IsValidFolder(MaterialFolder))
            AssetDatabase.CreateFolder("Assets/Project/Art/Materials/Map0", "Egitim");

        string path = MaterialFolder + "/M_Map0_Egitim_" + key + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(LitShader());
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.color = color;
        if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", 0.25f);
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.25f);
        if (glow)
        {
            mat.EnableKeyword("_EMISSION");
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            mat.SetColor("_EmissionColor", color * 1.4f);
        }
        EditorUtility.SetDirty(mat);
        return mat;
    }

    // projede URP açık; yine de hangi pipeline varsa onun varsayılan shader'ını kullan
    static Shader LitShader()
    {
        RenderPipelineAsset rp = GraphicsSettings.currentRenderPipeline;
        if (rp != null && rp.defaultMaterial != null) return rp.defaultMaterial.shader;
        return Shader.Find("Standard");
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

    static Text Label(string name, RectTransform parent, int size, FontStyle style, Color color, string text)
    {
        RectTransform rt = Rect(name, parent, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f));
        Text t = rt.gameObject.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = size; t.fontStyle = style; t.color = color; t.text = text;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    static void Place(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }
}
