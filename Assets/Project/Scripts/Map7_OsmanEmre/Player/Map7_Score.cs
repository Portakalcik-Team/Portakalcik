using System.Collections.Generic;
using UnityEngine;

// Skor ve taşınan yük (W06). Meyve toplayınca puan eklenir; ağır (Altın) meyve oyuncuyu yavaşlatır.
// Console'a "+3   Score: 7" yazar ve ekranın sol üstünde gösterir.
// Map7_LevelSetup oyun başında bunu oyuncuya ekler.
public class Map7_Score : MonoBehaviour
{
    [SerializeField] private float slowPerWeight = 0.2f;   // her yük birimi hızı yüzde kaç düşürür
    [SerializeField] private float minSpeedFactor = 0.35f; // ne kadar yük olursa olsun hız bunun altına inmez

    private int score = 0;
    private readonly List<float> carried = new List<float>();   // taşınan ağır meyvelerin yükleri
    private Map7_PlayerMove move;

    public int Score() { return score; }
    public float Load() { return TotalLoad(); }

    void Awake()
    {
        move = GetComponent<Map7_PlayerMove>();
    }

    // Map7_FruitPickup çağırır: bir meyve toplandı
    public void Collect(Map7_FruitType type, int points, float weight)
    {
        score += points;
        if (weight > 0f) carried.Add(weight);
        ApplySlow();
        Debug.Log("+" + points + " " + type + "   Score: " + score);
    }

    // Meyve Kepçesi çağırır: Portakal atılırken, taşınan Altın varsa o harcanır (true döner)
    public bool UseGolden()
    {
        if (carried.Count == 0) return false;
        carried.RemoveAt(carried.Count - 1);
        ApplySlow();
        return true;
    }

    float TotalLoad()
    {
        float sum = 0f;
        foreach (float w in carried) sum += w;
        return sum;
    }

    void ApplySlow()
    {
        if (move == null) return;
        float factor = Mathf.Max(minSpeedFactor, 1f - slowPerWeight * TotalLoad());
        move.SetSpeedMultiplier(factor);
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 22;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 16, 400, 34), "Skor: " + score, style);

        if (carried.Count > 0)
        {
            style.fontSize = 16;
            style.normal.textColor = new Color(1f, 0.84f, 0f);
            GUI.Label(new Rect(20, 48, 460, 28), "Altın Portakal x" + carried.Count + "  (yük " + TotalLoad().ToString("0.0") + ": yavaşlatır)", style);
        }
    }
}
