using UnityEngine;

// Spawner kendisi ekler, elle ekleme.
// Oyuncu değince meyveyi envantere ekler, puan yazar, gerekirse yavaşlatır, kaybolur.
public class M1FruitPickup : MonoBehaviour
{
    public M1FruitType type;
    public bool fall = true;         // ağaçtan düşer gibi yere insin

    float vy;
    bool landed;

    public static int TotalPoints;   // bu oyundaki toplam meyve puanı

    void Awake() { TotalPoints = TotalPoints; }

    // Spawner yüksekte (ağaç tepesinde) doğurur, meyve yere düşer
    void Update()
    {
        if (!fall || landed) return;

        vy += Physics.gravity.y * Time.deltaTime;
        float step = -vy * Time.deltaTime;
        float half = transform.lossyScale.y * 0.5f;

        // Altında zemin var mı? (trigger'lar, yani ağaçlar sayılmaz)
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, step + half, ~0, QueryTriggerInteraction.Ignore))
        {
            Vector3 pos = transform.position;
            transform.position = new Vector3(pos.x, hit.point.y + half, pos.z);
            landed = true;
            return;
        }
        transform.position += Vector3.down * step;
    }

    void OnTriggerEnter(Collider other)
    {
        if (type == null) return;
        var inv = other.GetComponentInParent<M1PlayerTrigger>();
        if (inv == null) return;

        inv.AddFruit(type.fruitIndex, type.amount);
        TotalPoints += type.points;

        string msg = "+" + type.points + " puan (" + type.displayName + ")  Toplam: " + TotalPoints;
        if (type.slowSeconds > 0f)
        {
            M1PlayerMove.SlowFactor = type.slowFactor;
            M1PlayerMove.SlowUntil = Time.time + type.slowSeconds;
            msg += "  Ağır! " + type.slowSeconds + " sn yavaşsın";
        }
        inv.Show(msg, 2f);
        Destroy(gameObject);
    }
}
