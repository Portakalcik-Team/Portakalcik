using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// 6. hafta: slayttaki adımları Map_0'a uygular
public static class Map0Week6CargoBuilder
{
    const string DataFolder = "Assets/Project/Data/Map0";
    const string HafifPath = "Assets/Project/Data/Map0/CT_Map0_Hafif.asset";
    const string AgirPath = "Assets/Project/Data/Map0/CT_Map0_Agir.asset";
    const string BasePrefabPath = "Assets/Project/Prefabs/Map0/Map0_Kargo.prefab";
    const string VariantPath = "Assets/Project/Prefabs/Map0/Map0_Kargo_Agir.prefab";
    const string AgirMatPath = "Assets/Project/Art/Materials/Map0/M_Map0_Agir.mat";

    [MenuItem("Map0/Hafta 6 Kargo Pedlerini Kur")]
    public static void Build()
    {
        var scene = EditorSceneManager.GetActiveScene();
        if (scene.name != "Map_0")
        {
            EditorUtility.DisplayDialog("Map0", "Önce Assets/Scenes/Map_0.unity sahnesini aç.", "Tamam");
            return;
        }

        // 1) veri dosyaları (varsa dokunma, ayarlar korunsun)
        if (!AssetDatabase.IsValidFolder(DataFolder))
            AssetDatabase.CreateFolder("Assets/Project/Data", "Map0");

        Map0CargoType hafif = EnsureType(HafifPath, 1, 0f);
        Map0CargoType agir = EnsureType(AgirPath, 3, 1f);

        // 2) ana prefab: hafif tip
        using (var scope = new PrefabUtility.EditPrefabContentsScope(BasePrefabPath))
        {
            Map0Cargo cargo = scope.prefabContentsRoot.GetComponent<Map0Cargo>();
            SetObject(cargo, "type", hafif);
        }

        // 3) ağır malzeme (M_Interactive.mat'a dokunmadan kopyala)
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(AgirMatPath);
        if (mat == null)
        {
            AssetDatabase.CopyAsset("Assets/Project/Art/Map2_Materials/M_Interactive.mat", AgirMatPath);
            mat = AssetDatabase.LoadAssetAtPath<Material>(AgirMatPath);
        }
        Color color;
        ColorUtility.TryParseHtmlString("#E07B1A", out color);
        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
        if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
        // M_Interactive güçlü sarı ışıyor. Turuncuyu güçlü ışıtınca ekran kırpar, yine sarı görünür.
        // Hafif bir turuncu ışıma: rengi korunur, sarılardan daha mat, "ağır" durur.
        if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", color * 0.35f);
        EditorUtility.SetDirty(mat);

        // 4) ağır prefab varyantı
        GameObject variant = AssetDatabase.LoadAssetAtPath<GameObject>(VariantPath);
        if (variant == null)
        {
            GameObject basePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BasePrefabPath);
            GameObject inst = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);

            SetObject(inst.GetComponent<Map0Cargo>(), "type", agir);

            Transform visual = inst.transform.Find("Map0_Kargo_Visual");
            visual.localScale = Vector3.one;
            visual.GetComponent<MeshRenderer>().sharedMaterial = mat;

            SerializedObject bob = new SerializedObject(visual.GetComponent<Map0Bobber>());
            bob.FindProperty("bobHeight").floatValue = 0.15f;
            bob.FindProperty("bobSpeed").floatValue = 1f;
            bob.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(inst, VariantPath);
            Object.DestroyImmediate(inst);
            variant = AssetDatabase.LoadAssetAtPath<GameObject>(VariantPath);
        }

        // 5) elle konan kargoları ve eski pedleri sil
        string[] olds =
        {
            "Map0_Week3_Kargo_01", "Map0_Week3_Kargo_02", "Map0_Week3_Kargo_03",
            "Map0_Week6_KargoPad_01", "Map0_Week6_KargoPad_02",
            "Map0_Week6_KargoPad_03", "Map0_Week6_KargoPad_Agir"
        };
        foreach (string old in olds)
        {
            GameObject found = FindIncludingInactive(old);
            if (found != null) Undo.DestroyObjectImmediate(found);
        }

        // 6) pedler
        GameObject hafifPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BasePrefabPath);
        CreatePad("Map0_Week6_KargoPad_01", new Vector3(15f, 0.7f, 15f), hafifPrefab, 4f);
        CreatePad("Map0_Week6_KargoPad_02", new Vector3(-15f, 0.7f, -3f), hafifPrefab, 4f);
        CreatePad("Map0_Week6_KargoPad_03", new Vector3(0f, 1.7f, -15f), hafifPrefab, 4f);
        CreatePad("Map0_Week6_KargoPad_Agir", new Vector3(0f, 0.7f, 9f), variant, 8f);

        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Map0 Hafta 6 kuruldu: 3 hafif + 1 ağır ped. Sahneyi kaydet (Ctrl+S).");
    }

    static Map0CargoType EnsureType(string path, int points, float weight)
    {
        Map0CargoType type = AssetDatabase.LoadAssetAtPath<Map0CargoType>(path);
        if (type != null) return type;

        type = ScriptableObject.CreateInstance<Map0CargoType>();
        SerializedObject so = new SerializedObject(type);
        so.FindProperty("points").intValue = points;
        so.FindProperty("weight").floatValue = weight;
        so.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.CreateAsset(type, path);
        return type;
    }

    static void SetObject(Object target, string field, Object value)
    {
        SerializedObject so = new SerializedObject(target);
        so.FindProperty(field).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void CreatePad(string name, Vector3 pos, GameObject prefab, float delay)
    {
        GameObject pad = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(pad, "Map0 Hafta 6");
        pad.transform.position = pos;
        pad.transform.localScale = Vector3.one;

        Map0CargoSpawner spawner = pad.AddComponent<Map0CargoSpawner>();
        SerializedObject so = new SerializedObject(spawner);
        so.FindProperty("cargoPrefab").objectReferenceValue = prefab;
        so.FindProperty("delay").floatValue = delay;
        so.ApplyModifiedPropertiesWithoutUndo();

        GameObject disc = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        disc.name = "Disc";
        disc.transform.SetParent(pad.transform, false);
        disc.transform.localPosition = new Vector3(0f, -0.69f, 0f);
        disc.transform.localScale = new Vector3(1.6f, 0.01f, 1.6f);
        Object.DestroyImmediate(disc.GetComponent<CapsuleCollider>());
    }

    static GameObject FindIncludingInactive(string name)
    {
        foreach (GameObject root in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            if (root.name == name) return root;
        return null;
    }
}
