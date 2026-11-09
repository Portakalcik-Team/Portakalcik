using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

// Map_0 sahnesine eğitim ipuçlarını, canavar galerisini ve canlı böceği kurar
public static class Map0TutorialBuilder
{
    private const string RootName = "Map0_Egitim";
    private const string FolderPath = "Assets/Project/Prefabs/Map0/Egitim";

    [MenuItem("Map0/Eğitimi Kur")]
    public static void Build()
    {
        // sadece doğru sahnede çalış
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Map_0")
        {
            EditorUtility.DisplayDialog("Map0", "Önce Assets/Scenes/Map_0.unity sahnesini aç.", "Tamam");
            return;
        }

        // asset klasörü yoksa oluştur
        if (!AssetDatabase.IsValidFolder(FolderPath))
        {
            AssetDatabase.CreateFolder("Assets/Project/Prefabs/Map0", "Egitim");
        }

        // 1) canavar veri dosyalarını oluştur / güncelle
        MonsterSeed[] monsters = CreateMonsterAssets();
        Map0MonsterData diken = monsters[0].data;

        // 2) eski eğitim kökünü sil (tekrar çalıştırılabilir)
        GameObject oldRoot = GameObject.Find(RootName);
        if (oldRoot != null)
        {
            Undo.DestroyObjectImmediate(oldRoot);
        }

        // 3) yeni kök
        GameObject root = new GameObject(RootName);
        root.transform.position = Vector3.zero;
        Undo.RegisterCreatedObjectUndo(root, "Map0 Eğitimi Kur");

        // ilerleme sayacı
        GameObject progress = CreateChild(root.transform, "Map0_Egitim_Ilerleme", Vector3.zero);
        Undo.AddComponent<Map0TutorialProgress>(progress);

        // boş gruplar
        Transform hints = CreateChild(root.transform, "Map0_Egitim_Ipuclari", Vector3.zero).transform;
        Transform gallery = CreateChild(root.transform, "Map0_Egitim_Galeri", Vector3.zero).transform;

        // arayüzü görsel ekibi kurar
        Map0TutorialVisuals.BuildHintCanvas(root.transform);

        // 4) temel ipuçları
        CreateHint(hints, "Map0_Egitim_Ipucu_01_Hareket",
            new Vector3(-13.5f, 1f, 15f), new Vector3(6f, 2f, 3f), 1,
            "Hoş geldin, Portakalcık!",
            "WASD ile yürü, karakter gittiği yöne döner. Space ile zıpla, ama sadece yerdeyken.",
            null);

        CreateHint(hints, "Map0_Egitim_Ipucu_02_Portakal",
            new Vector3(12f, 1f, 15f), new Vector3(6f, 2f, 3f), 2,
            "Portakal = çıkış anahtarı",
            "Portakalın üstünden yürü, başının üstüne alırsın. Onu yeşil çıkışa taşı. E ile istediğin an bırakırsın.",
            null);

        CreateHint(hints, "Map0_Egitim_Ipucu_03_Tehlike",
            new Vector3(6f, 1f, 9f), new Vector3(4f, 2f, 3f), 3,
            "Kırmızı = tehlike",
            "Kırmızıya değersen portakal elinden düşer. Üstünden zıpla ya da kenarından dolaş.",
            null);

        CreateHint(hints, "Map0_Egitim_Ipucu_04_Galeri",
            new Vector3(-15f, 1f, 3f), new Vector3(3f, 2f, 3f), 4,
            "Canavar Galerisi",
            "Diğer haritalarda bu 5 canavarla karşılaşacaksın. Her birinin yanına git ve tanı. Burada sana dokunamazlar.",
            null);

        CreateHint(hints, "Map0_Egitim_Ipucu_10_CanliDiken",
            new Vector3(13.5f, 1f, -3f), new Vector3(3f, 2f, 3f), 10,
            "Bu sefer gerçek!",
            "İleride gerçek bir Diken Böceği dolaşıyor. Değersen portakalı düşürürsün. Senden çok yavaş: zamanla ve yanından geç.",
            null);

        CreateHint(hints, "Map0_Egitim_Ipucu_11_YolAyrimi",
            new Vector3(-15f, 1f, -9f), new Vector3(3f, 2f, 3f), 11,
            "Yol ayrımı",
            "Sağ: kısa yol, hareketli kırmızı blok. Zamanlama ister. Aşağı: uzun yol, buzlu rampa ve platformda bir portakal daha.",
            null);

        CreateHint(hints, "Map0_Egitim_Ipucu_12_Buz",
            new Vector3(-10f, 1f, -15f), new Vector3(3f, 2f, 3f), 12,
            "Dikkat, buz!",
            "Rampa buzlu, tutunman %4'e düşer. Yavaş çıkarsan geri kayarsın. Hız al ve düz gir.",
            null);

        CreateHint(hints, "Map0_Egitim_Ipucu_13_Cikis",
            new Vector3(13.5f, 1f, -9f), new Vector3(3f, 2f, 3f), 13,
            "Son adım",
            "Portakalı yeşil alana bırak. Eğitim bitti, artık gerçek haritalara hazırsın!",
            null);

        // 5) canavar galerisi
        for (int i = 0; i < monsters.Length; i++)
        {
            MonsterSeed seed = monsters[i];
            Map0MonsterData data = seed.data;
            float x = -12f + i * 6f;

            // kaide
            Map0TutorialVisuals.BuildPedestal(gallery, new Vector3(x, 0f, 3.9f), data.color);

            // canavar kökü
            GameObject monsterRoot = CreateChild(gallery,
                "Map0_Egitim_Galeri_" + (i + 1) + "_" + seed.shortName,
                new Vector3(x, 0.5f, 3.9f));

            Transform visual = Map0TutorialVisuals.BuildMonster(data, monsterRoot.transform);

            if (visual != null)
            {
                // yavaşça dönsün
                Map0Spinner spinner = Undo.AddComponent<Map0Spinner>(visual.gameObject);
                SetFloat(spinner, "rotationSpeed", 30f);

                // hafifçe süzülsün
                Map0Bobber bobber = Undo.AddComponent<Map0Bobber>(visual.gameObject);
                SetFloat(bobber, "bobHeight", 0.1f);
                SetFloat(bobber, "bobSpeed", 1.5f);
            }

            // ipucu (5, 6, 7, 8, 9)
            CreateHint(gallery,
                "Map0_Egitim_Ipucu_" + (5 + i).ToString("00") + "_" + seed.shortName,
                new Vector3(x, 1f, 3f), new Vector3(3f, 2f, 3f),
                5 + i, data.monsterName, data.trait, data);
        }

        // 6) canlı Diken Böceği
        GameObject live = CreateChild(root.transform, "Map0_Egitim_CanliDiken",
            new Vector3(9f, 0.5f, -3f));

        Rigidbody rb = Undo.AddComponent<Rigidbody>(live);
        rb.isKinematic = true;
        rb.useGravity = false;

        BoxCollider liveBox = Undo.AddComponent<BoxCollider>(live);
        liveBox.isTrigger = true;
        liveBox.size = new Vector3(1.4f, 1f, 1.4f);

        Undo.AddComponent<Map0Hazard>(live);
        Map0Week5CameraBuilder.AddShake(live); // 5. hafta: değince kamera sarsılsın
        Map0Patroller patroller = Undo.AddComponent<Map0Patroller>(live);

        // B noktası dikenin çocuğu değil, kardeş nesne
        GameObject pointB = CreateChild(root.transform, "Map0_Egitim_CanliDiken_NoktaB",
            new Vector3(-9f, 0.5f, -3f));

        SerializedObject patrollerData = new SerializedObject(patroller);
        patrollerData.FindProperty("pointB").objectReferenceValue = pointB.transform;
        // oyuncu hızı 5 × 0.4 = 2 m/s (denge belgesi)
        patrollerData.FindProperty("speed").floatValue = 2f;
        patrollerData.ApplyModifiedProperties();

        // görsel: kök 0.5 m yukarıda (collider ortası), ayaklar yere bassın
        Transform liveVisual = Map0TutorialVisuals.BuildMonster(diken, live.transform);
        liveVisual.localPosition = new Vector3(0f, -0.5f, 0f);
        liveVisual.localRotation = Quaternion.Euler(0f, -90f, 0f); // ilk gittiği yön: -X

        // 7) bitir
        EditorSceneManager.MarkSceneDirty(scene);
        Selection.activeGameObject = root;
        Debug.Log("Map0: Eğitim kuruldu. Sahneyi kaydetmeyi unutma (Ctrl/Cmd+S).");
    }

    // boş bir çocuk nesne oluşturur
    private static GameObject CreateChild(Transform parent, string name, Vector3 localPosition)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPosition;
        Undo.RegisterCreatedObjectUndo(go, "Map0 nesnesi oluştur");
        return go;
    }

    // tetikleyici kutu + ipucu bileşeni kurar
    private static Map0TutorialHint CreateHint(Transform parent, string objName, Vector3 center,
        Vector3 size, int step, string title, string body, Map0MonsterData monster)
    {
        GameObject go = new GameObject(objName);
        go.transform.SetParent(parent, false);
        go.transform.position = center;
        go.layer = 0; // Default
        Undo.RegisterCreatedObjectUndo(go, "Map0 ipucu oluştur");

        // görünmez tetikleyici alan
        BoxCollider box = Undo.AddComponent<BoxCollider>(go);
        box.isTrigger = true;
        box.size = size;

        // ipucu bilgileri
        Map0TutorialHint hint = Undo.AddComponent<Map0TutorialHint>(go);
        hint.Configure(step, title, body, monster);

        // canavarı olmayan ipuçlarının yerde işareti olsun
        if (monster == null)
        {
            Map0TutorialVisuals.BuildHintMarker(go.transform);
        }

        return hint;
    }

    // canavar asset'lerini oluşturur veya varsa günceller
    private static MonsterSeed[] CreateMonsterAssets()
    {
        MonsterSeed[] seeds = BuildSeeds();

        foreach (MonsterSeed seed in seeds)
        {
            string path = FolderPath + "/" + seed.fileName;
            Map0MonsterData data = AssetDatabase.LoadAssetAtPath<Map0MonsterData>(path);

            if (data == null)
            {
                data = ScriptableObject.CreateInstance<Map0MonsterData>();
                AssetDatabase.CreateAsset(data, path);
            }

            // alanları her çalıştırmada tazele
            data.monsterName = seed.displayName;
            data.shape = seed.shape;
            data.firstLevel = seed.firstLevel;
            data.speedRatio = seed.speedRatio;
            data.attackInterval = seed.attackInterval;
            data.hpLevel1 = seed.hp;
            data.damageLevel1 = seed.damage;
            data.trait = seed.trait;
            data.tip = seed.tip;
            data.color = ParseColor(seed.colorHex);

            EditorUtility.SetDirty(data);
            seed.data = data;
        }

        AssetDatabase.SaveAssets();
        return seeds;
    }

    // denge belgesindeki 5 canavar
    private static MonsterSeed[] BuildSeeds()
    {
        return new MonsterSeed[]
        {
            new MonsterSeed
            {
                shortName = "DikenBocegi",
                fileName = "Map0_Canavar_1_DikenBocegi.asset",
                displayName = "Diken Böceği",
                shape = Map0MonsterShape.Bocek,
                firstLevel = "Seviye 1",
                speedRatio = 0.4f,
                attackInterval = 2.0f,
                hp = 6,
                damage = 4,
                colorHex = "#6E8B3D",
                trait = "Çok yavaş, düşük hasar. Kolay hedef.",
                tip = "Kısayol kapılarının önünde bekler. 2 mandalina yeter."
            },
            new MonsterSeed
            {
                shortName = "GolgeYarasa",
                fileName = "Map0_Canavar_2_GolgeYarasa.asset",
                displayName = "Gölge Yarasa",
                shape = Map0MonsterShape.Yarasa,
                firstLevel = "Seviye 2",
                speedRatio = 1.2f,
                attackInterval = 1.0f,
                hp = 9,
                damage = 6,
                colorHex = "#4B3B6B",
                trait = "Senden hızlı ama kırılgan. Uçar.",
                tip = "Kaçamazsın; dur, nişan al ve vur."
            },
            new MonsterSeed
            {
                shortName = "OrmanTilkisi",
                fileName = "Map0_Canavar_3_OrmanTilkisi.asset",
                displayName = "Orman Tilkisi",
                shape = Map0MonsterShape.Tilki,
                firstLevel = "Seviye 3",
                speedRatio = 0.9f,
                attackInterval = 1.5f,
                hp = 12,
                damage = 7,
                colorHex = "#D9772B",
                trait = "Hızı sana yakın. Kaçabilir!",
                tip = "Iskalanan atış boşa giden mandalinadır. Yakından at."
            },
            new MonsterSeed
            {
                shortName = "KokGolemi",
                fileName = "Map0_Canavar_4_KokGolemi.asset",
                displayName = "Kök Golemi",
                shape = Map0MonsterShape.Golem,
                firstLevel = "Seviye 5",
                speedRatio = 0.3f,
                attackInterval = 2.5f,
                hp = 18,
                damage = 10,
                colorHex = "#7A5A3A",
                trait = "Tank: çok yavaş ama çok güçlü.",
                tip = "Mandalinan azsa uzun yoldan git, onu atla."
            },
            new MonsterSeed
            {
                shortName = "LabirentMuhafizi",
                fileName = "Map0_Canavar_5_LabirentMuhafizi.asset",
                displayName = "Labirent Muhafızı",
                shape = Map0MonsterShape.Muhafiz,
                firstLevel = "Seviye 9",
                speedRatio = 0.7f,
                attackInterval = 1.8f,
                hp = 24,
                damage = 12,
                colorHex = "#8A8F99",
                trait = "Boss. Çıkışa en yakın kısayolu korur.",
                tip = "Önce ağaçları topla, sonra karşısına çık."
            }
        };
    }

    // "#6E8B3D" gibi metni renge çevirir
    private static Color ParseColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color)) return color;
        return Color.white;
    }

    // private SerializedField değerini editörden değiştirir
    private static void SetFloat(UnityEngine.Object target, string fieldName, float value)
    {
        SerializedObject so = new SerializedObject(target);
        so.FindProperty(fieldName).floatValue = value;
        so.ApplyModifiedProperties();
    }

    // tek bir canavarın tablo satırı
    private class MonsterSeed
    {
        public string shortName;
        public string fileName;
        public string displayName;
        public Map0MonsterShape shape;
        public string firstLevel;
        public float speedRatio;
        public float attackInterval;
        public int hp;
        public int damage;
        public string colorHex;
        public string trait;
        public string tip;

        public Map0MonsterData data; // oluşturulan asset (sonradan doldurulur)
    }
}
