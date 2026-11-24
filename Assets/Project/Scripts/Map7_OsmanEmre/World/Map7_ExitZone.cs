using UnityEngine;

// Map 7 çıkış bölgesi (W07): oyuncu çıkışa ulaşınca Map7_Round.EndRound çağrılır ve oyun biter.
public class Map7_ExitZone : MonoBehaviour
{
    [SerializeField] private Map7_Round round;

    public void Configure(Map7_Round r)
    {
        round = r;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (round == null) round = FindFirstObjectByType<Map7_Round>();
        if (round != null) round.EndRound("Seviye tamamlandı!");
    }
}
