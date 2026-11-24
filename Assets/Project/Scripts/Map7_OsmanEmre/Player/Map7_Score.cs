using System.Collections.Generic;
using UnityEngine;

// Taşınan yük (W06) ve Map7_Round'a puan aktarımı (W07).
// Skoru artık sahnedeki Map7_Round tutar ve Canvas (ScoreText) üzerinde gösterir;
// bu bileşen ağır (Altın) meyvelerin yükünü sayar ve oyuncuyu yavaşlatır.
public class Map7_Score : MonoBehaviour
{
    [SerializeField] private float slowPerWeight = 0.2f;   // her yük birimi hızı yüzde kaç düşürür
    [SerializeField] private float minSpeedFactor = 0.35f; // ne kadar yük olursa olsun hız bunun altına inmez

    private readonly List<float> carried = new List<float>();   // taşınan ağır meyvelerin yükleri
    private Map7_PlayerMove move;
    private Map7_Round round;

    public int Score()
    {
        if (round == null) round = FindFirstObjectByType<Map7_Round>();
        return round != null ? round.Score() : 0;
    }

    public float Load() { return TotalLoad(); }

    void Awake()
    {
        move = GetComponent<Map7_PlayerMove>();
    }

    // Map7_FruitPickup çağırır: bir meyve toplandı
    public void Collect(Map7_FruitType type, int points, float weight)
    {
        if (round == null) round = Map7_Round.EnsureInScene();
        if (round != null) round.AddPoints(points);

        if (weight > 0f) carried.Add(weight);
        ApplySlow();

        int total = round != null ? round.Score() : points;
        Debug.Log("+" + points + " " + type + "   Score: " + total);
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
        // Tur bittiyse veya ağır meyve taşınmıyorsa hiçbir şey çizme (skor artık Canvas HUD'da)
        if (Time.timeScale == 0f || carried.Count == 0) return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 16;
        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = new Color(1f, 0.84f, 0f);
        GUI.Label(new Rect(0, Screen.height - 102f, Screen.width, 26f),
            "Altın Portakal x" + carried.Count + "  (yük " + TotalLoad().ToString("0.0") + ": yavaşlatır)", style);
    }
}
