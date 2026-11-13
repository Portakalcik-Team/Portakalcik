using UnityEngine;

// Veri dosyası (ScriptableObject): sahneye konmaz, Project'te .asset olarak durur.
// Create > Portakalcik > M1 Meyve Tipi ile oluşturulur.
[CreateAssetMenu(menuName = "Portakalcik/M1 Meyve Tipi")]
public class M1FruitType : ScriptableObject
{
    public string displayName = "Mandalina";
    [Tooltip("0 = mandalina, 1 = portakal, 2 = limon")]
    public int fruitIndex = 0;
    public int amount = 1;            // kaç meyve eklenir
    public int points = 1;            // alınca kaç puan
    public float size = 0.35f;        // yerdeki görünüm boyu
    public Color color = new Color(1f, 0.65f, 0.1f);

    [Header("Bedeli")]
    public float slowSeconds = 0f;    // kaç sn yavaşlatır
    [Range(0.1f, 1f)] public float slowFactor = 1f;   // 0.5 = yarı hız
}
