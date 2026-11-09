#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// Map7 düşman prefab'larına karakter şekillerini gerçek alt objeler olarak kaydeder.
// Unity script derlemesinden sonra otomatik çalışır; elle çalıştırmak için: Tools > Map7 > Bake Enemy Visuals
[InitializeOnLoad]
public static class Map7_BakeEnemies
{
    const string PrefabFolder = "Assets/Project/Prefabs/Map7_OsmanEmre";

    static Map7_BakeEnemies()
    {
        // Derleme bitince, Play modunda değilken bir kez dene (zaten kayıtlıysa hiçbir şey yapmaz)
        EditorApplication.delayCall += () =>
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) Bake(false);
        };
    }

    [MenuItem("Tools/Map7/Bake Enemy Visuals")]
    static void BakeMenu() { Bake(true); }

    [MenuItem("Tools/Map7/Rebuild Enemy Visuals (overwrite)")]
    static void RebakeMenu() { Bake(true, true); }

    // Sahnedeki düşmanlar prefab örneği değil, bağımsız kopyalar; bu yüzden prefab değişince güncellenmiyorlar.
    // Bu araç onlara da Map7_EnemyLook ekleyip görünümü kurar. Kayıtlı sahneyi ayrıca kaydetmen gerekir (Ctrl+S).
    [MenuItem("Tools/Map7/Apply Enemy Visuals To Open Scene")]
    static void ApplyToSceneMenu() { ApplyToScene(true); }

    static void ApplyToScene(bool log)
    {
        int count = 0;
        foreach (Transform t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            // Kapılar: katı engel + portakalla açılma mantığı
            if (t.name.StartsWith("Door_") && t.GetComponent<Map7_Door>() == null)
            {
                Undo.AddComponent<Map7_Door>(t.gameObject);
                EditorUtility.SetDirty(t.gameObject);
                count++;
            }

            // Oyuncu: meyve kepçesi (fırlatıcı)
            if (t.name == "Player" && t.CompareTag("Player") && t.GetComponent<Map7_FruitLauncher>() == null)
            {
                Undo.AddComponent<Map7_FruitLauncher>(t.gameObject);
                EditorUtility.SetDirty(t.gameObject);
                count++;
            }

            // Oyuncu: labirent kaşifi görünümü
            if (t.name == "Player" && t.CompareTag("Player") && t.GetComponent<Map7_PlayerLook>() == null)
            {
                Map7_PlayerLook pl = Undo.AddComponent<Map7_PlayerLook>(t.gameObject);
                pl.Build();
                EditorUtility.SetDirty(t.gameObject);
                count++;
                continue;
            }

            // Sahnede çıkışın adı sadece "Exit" olabilir
            if (t.name == "Exit" && t.GetComponent<Map7_PropLook>() == null)
            {
                Map7_PropLook ep = Undo.AddComponent<Map7_PropLook>(t.gameObject);
                SerializedObject eso = new SerializedObject(ep);
                eso.FindProperty("kind").enumValueIndex = (int)Map7_PropKind.Exit;
                eso.ApplyModifiedPropertiesWithoutUndo();
                ep.Build();
                EditorUtility.SetDirty(t.gameObject);
                count++;
                continue;
            }

            if (!t.name.EndsWith("_Prefab") && !System.Text.RegularExpressions.Regex.IsMatch(t.name, @"_\d+_\d+$")) continue;

            // Düşmanlar
            if (t.GetComponent<Map7_EnemyLook>() == null)
            {
                Map7_EnemyKind? kind = null;
                if (t.name.StartsWith("DikenB")) kind = Map7_EnemyKind.DikenBocegi;
                else if (t.name.StartsWith("GolgeYarasa")) kind = Map7_EnemyKind.GolgeYarasa;
                else if (t.name.StartsWith("OrmanTilkisi")) kind = Map7_EnemyKind.OrmanTilkisi;
                else if (t.name.StartsWith("KokGolemi")) kind = Map7_EnemyKind.KokGolemi;
                if (kind != null)
                {
                    Map7_EnemyLook look = Undo.AddComponent<Map7_EnemyLook>(t.gameObject);
                    SerializedObject so = new SerializedObject(look);
                    so.FindProperty("kind").enumValueIndex = (int)kind.Value;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    look.Build();
                    EditorUtility.SetDirty(t.gameObject);
                    count++;
                    continue;
                }
            }

            // Meyve ağaçları
            if (t.GetComponent<Map7_TreeLook>() == null)
            {
                Map7_FruitType? fruit = null;
                if (t.name.StartsWith("Limon_")) fruit = Map7_FruitType.Limon;
                else if (t.name.StartsWith("Mandalina_")) fruit = Map7_FruitType.Mandalina;
                else if (t.name.StartsWith("Portakal_")) fruit = Map7_FruitType.Portakal;
                if (fruit != null)
                {
                    Map7_TreeLook tree = Undo.AddComponent<Map7_TreeLook>(t.gameObject);
                    SerializedObject so = new SerializedObject(tree);
                    so.FindProperty("fruitType").enumValueIndex = (int)fruit.Value;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    tree.Build();
                    EditorUtility.SetDirty(t.gameObject);
                    count++;
                    continue;
                }
            }

            // Duvar, kapı, çıkış
            if (t.GetComponent<Map7_PropLook>() == null)
            {
                Map7_PropKind? prop = null;
                if (t.name.StartsWith("Wall_")) prop = Map7_PropKind.Wall;
                else if (t.name.StartsWith("Door_")) prop = Map7_PropKind.Door;
                else if (t.name.StartsWith("Exit_")) prop = Map7_PropKind.Exit;
                if (prop != null)
                {
                    Map7_PropLook p = Undo.AddComponent<Map7_PropLook>(t.gameObject);
                    SerializedObject so = new SerializedObject(p);
                    so.FindProperty("kind").enumValueIndex = (int)prop.Value;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    p.Build();
                    EditorUtility.SetDirty(t.gameObject);
                    count++;
                }
            }
        }
        if (count > 0)
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        if (log || count > 0) Debug.Log("[Map7] Sahnedeki objelere yeni görünüm eklendi: " + count + " adet. Sahneyi kaydetmeyi unutma (Ctrl+S).");
    }

    static void Bake(bool log, bool force = false)
    {
        int count = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { PrefabFolder }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                Map7_EnemyLook look = root.GetComponent<Map7_EnemyLook>();
                Map7_TreeLook tree = root.GetComponent<Map7_TreeLook>();
                Map7_PropLook prop = root.GetComponent<Map7_PropLook>();
                Map7_PlayerLook playerLook = root.GetComponent<Map7_PlayerLook>();
                if (look == null && tree == null && prop == null && playerLook == null) continue;
                if (!force && root.transform.Find("Visual") != null) continue;

                if (look != null) look.Build();
                if (tree != null) tree.Build();
                if (prop != null) prop.Build();
                if (playerLook != null) playerLook.Build();
                PrefabUtility.SaveAsPrefabAsset(root, path);
                count++;
                Debug.Log("[Map7] Görünüm prefab'a kaydedildi: " + path);
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
        if (count > 0) AssetDatabase.SaveAssets();
        else if (log) Debug.Log("[Map7] Kaydedilecek yeni görünüm yok (hepsi zaten kayıtlı).");
    }
}
#endif
