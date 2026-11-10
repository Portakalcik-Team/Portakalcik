using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Seviye 9 · Hafta 6: asset, prefab ve spawner kurulumu — iki kez çalışsa da bozmaz
public static class Map9Week6Setup
{
    private const string DataDir = "Assets/Project/Data/Map9_EC";
    private const string PrefabDir = "Assets/Project/Prefabs/Map9_EC";
    private const string MatDir = "Assets/Project/Art/Map9_Materials";

    private const string BasePrefabPath = PrefabDir + "/M9_Portakal.prefab";
    private const string DevPrefabPath = PrefabDir + "/M9_Portakal_Dev.prefab";

    // dev portakalın hücresi: Muhafiz_01'in güneyindeki boş koridor
    private const int DevRow = 8;
    private const int DevCol = 7;
    private const float DevHeight = 1f;

    [MenuItem("Tools/Map9/Hafta 6 Kur")]
    public static void Run()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (EditorApplication.isPlaying || scene.name != "Map_9")
        {
            EditorUtility.DisplayDialog("Hafta 6 Kur", "Play modundan çık ve Map_9 sahnesini aç.", "Tamam");
            return;
        }

        Undo.SetCurrentGroupName("Map9 Hafta 6 Kur");
        int undoGroup = Undo.GetCurrentGroup();

        // a. klasör ve türler
        EnsureFolder("Assets/Project", "Data");
        EnsureFolder("Assets/Project/Data", "Map9_EC");
        Map9PortakalType normalType = MakeType(DataDir + "/PT9_Normal.asset", 1, 0f);
        Map9PortakalType devType = MakeType(DataDir + "/PT9_Dev.asset", 3, 1f);

        // b. taban prefab (sahnedeki eski portakaldan)
        Map9Portakal old = Object.FindAnyObjectByType<Map9Portakal>();
        GameObject basePrefab = MakeBasePrefab(old, normalType);
        if (basePrefab == null) return;

        // c. dev variant
        GameObject devPrefab = MakeDevVariant(basePrefab, devType);

        // f. spawner işaretlerinin malzemeleri
        Material normalDisk = MakeColorMaterial(MatDir + "/M9_SpawnerNormal.mat", new Color32(0xFF, 0x9A, 0x1F, 255));
        Material devDisk = MakeColorMaterial(MatDir + "/M9_SpawnerDev.mat", new Color32(0xD1, 0x23, 0x1B, 255));

        // d. normal spawner: eski portakalın yerine, sonra eskisini sil
        GameObject normalSpawner = GameObject.Find("PortakalSpawner_09");
        if (normalSpawner == null)
        {
            if (old != null) MakeSpawner("PortakalSpawner_09", old.transform.position, basePrefab, 4f, normalDisk);
            else Debug.LogError("PortakalSpawner_09 kurulamadı: sahnede eski portakal da yok.");
        }
        if (old != null)
        {
            Debug.Log("Eski portakal silindi: " + old.name);
            Undo.DestroyObjectImmediate(old.gameObject);
        }

        // e. dev spawner: X=(sütun-6)*3, Z=(6-satır)*3
        if (GameObject.Find("PortakalSpawner_Dev_09") == null)
        {
            Vector3 devPos = new Vector3((DevCol - 6) * 3f, DevHeight, (6 - DevRow) * 3f);
            MakeSpawner("PortakalSpawner_Dev_09", devPos, devPrefab, 8f, devDisk);
            Debug.Log("Dev portakal hücresi: R" + DevRow + "C" + DevCol + " " + devPos + " (Muhafiz_01'in yanı)");
        }

        // g. Ctrl+S elle yapılacak
        Undo.CollapseUndoOperations(undoGroup);
        AssetDatabase.SaveAssets();
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Hafta 6 kurulumu bitti. Sahneyi Ctrl+S ile kaydet.");
    }

    private static void EnsureFolder(string parent, string name)
    {
        if (AssetDatabase.IsValidFolder(parent + "/" + name)) return;
        AssetDatabase.CreateFolder(parent, name);
        Debug.Log("Klasör: " + parent + "/" + name);
    }

    private static Map9PortakalType MakeType(string path, int points, float weight)
    {
        Map9PortakalType type = AssetDatabase.LoadAssetAtPath<Map9PortakalType>(path);
        if (type != null) return type;

        type = ScriptableObject.CreateInstance<Map9PortakalType>();
        AssetDatabase.CreateAsset(type, path);

        // alanlar private: SerializedObject ile yazılır
        SerializedObject so = new SerializedObject(type);
        so.FindProperty("points").intValue = points;
        so.FindProperty("weight").floatValue = weight;
        so.ApplyModifiedPropertiesWithoutUndo();
        Debug.Log("Tür oluşturuldu: " + path);
        return type;
    }

    private static void SetType(Map9Portakal portakal, Map9PortakalType type)
    {
        SerializedObject so = new SerializedObject(portakal);
        so.FindProperty("type").objectReferenceValue = type;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    private static GameObject MakeBasePrefab(Map9Portakal old, Map9PortakalType type)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(BasePrefabPath);
        if (prefab != null) return prefab;

        if (old == null)
        {
            Debug.LogError("Sahnede Map9Portakal yok, M9_Portakal oluşturulamadı.");
            return null;
        }

        // kopyayı kaydet: sahnedeki asıl obje prefab'a bağlanmasın
        GameObject copy = Object.Instantiate(old.gameObject);
        copy.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        copy.GetComponent<Rigidbody>().isKinematic = true;
        SetType(copy.GetComponent<Map9Portakal>(), type);

        prefab = PrefabUtility.SaveAsPrefabAsset(copy, BasePrefabPath);
        Object.DestroyImmediate(copy);
        Debug.Log("Prefab oluşturuldu: " + BasePrefabPath);
        return prefab;
    }

    private static GameObject MakeDevVariant(GameObject basePrefab, Map9PortakalType devType)
    {
        GameObject variant = AssetDatabase.LoadAssetAtPath<GameObject>(DevPrefabPath);
        if (variant != null) return variant;

        Material baseMat = basePrefab.transform.Find("Visual").GetComponent<Renderer>().sharedMaterial;
        Material devMat = MakeDevMaterial(baseMat);

        // prefab örneğini kaydetmek Variant üretir
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
        SetType(instance.GetComponent<Map9Portakal>(), devType);

        Transform visual = instance.transform.Find("Visual");
        Renderer visualRenderer = visual.GetComponent<Renderer>();
        visual.localScale *= 1.5f;
        visualRenderer.sharedMaterial = devMat;
        PrefabUtility.RecordPrefabInstancePropertyModifications(visual);
        PrefabUtility.RecordPrefabInstancePropertyModifications(visualRenderer);

        variant = PrefabUtility.SaveAsPrefabAsset(instance, DevPrefabPath);
        Object.DestroyImmediate(instance);
        Debug.Log("Variant oluşturuldu: " + DevPrefabPath);
        return variant;
    }

    private static Material MakeDevMaterial(Material source)
    {
        string path = MatDir + "/M9_PortakalDev.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        mat = new Material(source);
        mat.parent = null;   // M9_Portakal, M9_Mandalina'nın variant'ı: bağımsız olsun
        mat.SetColor("_BaseColor", new Color32(0xB8, 0x3A, 0x0B, 255));
        AssetDatabase.CreateAsset(mat, path);
        Debug.Log("Malzeme oluşturuldu: " + path);
        return mat;
    }

    private static Material MakeColorMaterial(string path, Color color)
    {
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", color);
        AssetDatabase.CreateAsset(mat, path);
        Debug.Log("Malzeme oluşturuldu: " + path);
        return mat;
    }

    private static void MakeSpawner(string name, Vector3 position, GameObject prefab, float delay, Material diskMat)
    {
        GameObject spawner = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(spawner, "Map9 Spawner");
        spawner.transform.position = position;

        // alanlar private: SerializedObject ile yazılır
        SerializedObject so = new SerializedObject(spawner.AddComponent<Map9PortakalSpawner>());
        so.FindProperty("portakalPrefab").objectReferenceValue = prefab;
        so.FindProperty("delay").floatValue = delay;
        so.ApplyModifiedPropertiesWithoutUndo();

        // işaret diski: zemine oturur, collider yok
        GameObject disk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Object.DestroyImmediate(disk.GetComponent<Collider>());
        disk.name = "Disk";
        disk.transform.SetParent(spawner.transform, false);
        disk.transform.position = new Vector3(position.x, 0.02f, position.z);
        disk.transform.localScale = new Vector3(1.2f, 0.02f, 1.2f);
        disk.GetComponent<Renderer>().sharedMaterial = diskMat;
        Undo.RegisterCreatedObjectUndo(disk, "Map9 Spawner Disk");

        Debug.Log("Spawner oluşturuldu: " + name + " " + position + ", delay " + delay);
    }
}
