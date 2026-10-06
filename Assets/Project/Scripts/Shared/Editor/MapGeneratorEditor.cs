using UnityEditor;
using UnityEngine;

/// <summary>
/// MapGenerator için özel Editor Inspector'ı.
/// "Haritayı Oluştur" ve "Haritayı Temizle" butonları ekler.
/// </summary>
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Varsayılan Inspector çizimi
        DrawDefaultInspector();

        MapGenerator generator = (MapGenerator)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Editor Araçları", EditorStyles.boldLabel);

        // Oluştur butonu
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        if (GUILayout.Button("🗺️ Haritayı Oluştur", GUILayout.Height(35)))
        {
            generator.ClearGenerated();
            generator.GenerateMap();
            EditorUtility.SetDirty(generator);
        }

        // Temizle butonu
        GUI.backgroundColor = new Color(0.9f, 0.3f, 0.3f);
        if (GUILayout.Button("🗑️ Haritayı Temizle", GUILayout.Height(30)))
        {
            generator.ClearGenerated();
            EditorUtility.SetDirty(generator);
        }

        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "Prefab'ları atadıktan sonra 'Haritayı Oluştur' butonuna basın.\n" +
            "Play modunda da otomatik oluşturulur (generateOnStart açıksa).",
            MessageType.Info);
    }
}
