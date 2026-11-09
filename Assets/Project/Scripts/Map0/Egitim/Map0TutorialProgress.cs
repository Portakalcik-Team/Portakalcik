using System.Collections.Generic;
using UnityEngine;

// eğitimde kaç ipucunun görüldüğünü sayar; sahnede tek tane olur
public class Map0TutorialProgress : MonoBehaviour
{
    public static Map0TutorialProgress Instance { get; private set; }

    // (görülen, toplam)
    public event System.Action<int, int> Changed;

    private HashSet<Map0TutorialHint> seen = new HashSet<Map0TutorialHint>(); // her ipucu bir kere
    private int total;
    private bool completed; // "tamamlandı" yazısı bir kere çıksın

    public int Seen => seen.Count;
    public int Total => total;

    void Awake()
    {
        // sahnedeki tek ilerleme sayacı
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        // sahnedeki bütün ipuçlarını say
        total = FindObjectsByType<Map0TutorialHint>(FindObjectsSortMode.None).Length;
        Changed?.Invoke(0, total);
    }

    // ipucu tetiklendiğinde çağrılır
    public void MarkSeen(Map0TutorialHint hint)
    {
        if (!seen.Add(hint)) return; // daha önce görüldüyse sayma

        Changed?.Invoke(Seen, Total);

        // hepsi görüldüyse sadece bir kere haber ver
        if (Seen >= Total && !completed)
        {
            completed = true;
            Debug.Log("Map0: Eğitim tamamlandı!");
        }
    }
}
