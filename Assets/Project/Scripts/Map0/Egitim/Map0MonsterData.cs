using UnityEngine;

// galeride canavarın hangi görselle çizileceğini belirtir
public enum Map0MonsterShape { Bocek, Yarasa, Tilki, Golem, Muhafiz }

// bir canavarın sayısal ve metinsel bilgilerini tutan veri dosyası
[CreateAssetMenu(fileName = "Map0_Canavar", menuName = "Map0/Canavar Bilgisi")]
public class Map0MonsterData : ScriptableObject
{
    // --- kimlik ---
    public string monsterName;              // "Diken Böceği"
    public Map0MonsterShape shape;

    // --- denge değerleri ---
    public string firstLevel;               // "Seviye 1"
    public float speedRatio;                // oyuncu hızı = 1.0
    public float attackInterval;            // saniye
    public int hpLevel1;
    public int damageLevel1;

    // --- oyuncuya anlatım ---
    [TextArea] public string trait;         // kısa özellik
    [TextArea] public string tip;           // oyuncuya taktik

    // --- görsel ---
    public Color color;                     // vücut rengi

    // bir mandalinanın verdiği hasar (denge belgesi)
    public const int TangerineDamage = 3;

    // bu canavarı öldürmek için kaç mandalina gerekir?
    public int HitsToKill()
    {
        return Mathf.CeilToInt(hpLevel1 / (float)TangerineDamage);
    }
}
