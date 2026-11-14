using UnityEditor;
using UnityEngine;

// Seviye 9: zemin ve duvar dokularını malzemelere atar — iki kez çalışsa da bozmaz
public static class Map9TextureSetup
{
    private const string MatDir = "Assets/Project/Art/Map9_Materials";
    private const string TriplanarShader = "Portakalcik/Map9 Triplanar";

    private const float TileMeters = 2f;   // doku karesi gerçekte 2x2 m
    private const float FloorSize = 39f;   // Floor objesinin kenarı

    [MenuItem("Tools/Map9/Zemin ve Duvar Doku Kur")]
    public static void Run()
    {
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("Doku Kur", "Önce Play modundan çık.", "Tamam");
            return;
        }

        Shader triplanar = Shader.Find(TriplanarShader);
        if (triplanar == null)
        {
            Debug.LogError("Shader bulunamadı: " + TriplanarShader);
            return;
        }

        // zemin: tek parça, URP Lit yeterli
        Material floor = AssetDatabase.LoadAssetAtPath<Material>(MatDir + "/M9_Floor.mat");
        Apply(floor, "T9_FloorStone", FloorSize / TileMeters);
        floor.EnableKeyword("_NORMALMAP");

        // duvar: farklı ölçekli küpler, dünya koordinatıyla döşenir
        Material wall = AssetDatabase.LoadAssetAtPath<Material>(MatDir + "/M9_Wall.mat");
        wall.shader = triplanar;
        Apply(wall, "T9_WallBlocks", 1f / TileMeters);

        AssetDatabase.SaveAssets();
        Debug.Log("Zemin ve duvar dokuları kuruldu.");
    }

    private static void Apply(Material mat, string texturePrefix, float tiling)
    {
        mat.SetTexture("_BaseMap", ImportTexture(texturePrefix + "_Diff.jpg", false));
        mat.SetTextureScale("_BaseMap", Vector2.one * tiling);
        mat.SetTexture("_BumpMap", ImportTexture(texturePrefix + "_Normal.jpg", true));
        mat.SetFloat("_BumpScale", 1f);
        mat.SetFloat("_Smoothness", 0.2f);
        mat.SetColor("_BaseColor", Color.white);   // eski renk dokuyu boyamasın
        EditorUtility.SetDirty(mat);
    }

    private static Texture2D ImportTexture(string fileName, bool isNormal)
    {
        string path = MatDir + "/" + fileName;
        TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
        TextureImporterType type = isNormal ? TextureImporterType.NormalMap : TextureImporterType.Default;

        // sadece gerekirse yeniden içe aktar
        if (importer.textureType != type || importer.anisoLevel != 8)
        {
            importer.textureType = type;
            importer.anisoLevel = 8;   // birinci şahısta eğik bakışta bulanmasın
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }
}
