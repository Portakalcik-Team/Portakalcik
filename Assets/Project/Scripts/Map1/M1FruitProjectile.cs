using UnityEngine;

// Fırlatılan meyvenin üstünde (M1PlayerThrow kendisi ekler, elle ekleme).
// Düşmana çarparsa hasar verir, duvara/zemine çarparsa kaybolur. Ağaçların ve tetikleyicilerin içinden geçer.
public class M1FruitProjectile : MonoBehaviour
{
    public int damage = 3;
    public string fruitName = "Mandalina";
    public M1PlayerTrigger owner;

    static readonly string[] EnemyPrefixes = { "diken", "yarasa", "tilki", "golem", "muhafiz" };
    static readonly string[] TreePrefixes = { "limon", "mandalina", "portakal" };

    bool used;

    // Düşmanlar trigger olduğu için çoğu çarpma buraya düşer
    void OnTriggerEnter(Collider other) { Hit(other.gameObject, true); }
    // Duvar ve zemin normal collider
    void OnCollisionEnter(Collision c)  { Hit(c.gameObject, false); }

    void Hit(GameObject go, bool isTrigger)
    {
        if (used) return;

        for (Transform t = go.transform; t != null; t = t.parent)
        {
            string n = t.name.ToLower();

            if (StartsWith(n, EnemyPrefixes)) { DamageEnemy(t.gameObject); return; }
            if (StartsWith(n, TreePrefixes) || n.StartsWith("player") || n.StartsWith("firlatilan")) return;   // geç
        }

        // Başka bir trigger (Exit, kapı vb.) ise içinden geç; katı bir şeyse kaybol
        if (!isTrigger) { used = true; Destroy(gameObject); }
    }

    void DamageEnemy(GameObject enemyObj)
    {
        used = true;

        var enemy = enemyObj.GetComponent<M1Enemy>();
        if (enemy == null) enemy = enemyObj.AddComponent<M1Enemy>();   // elle eklemene gerek yok

        bool died = enemy.TakeDamage(damage);
        if (owner != null)
        {
            string label = enemyObj.name.Split('_')[0];
            owner.Show(died
                ? label + " yenildi!" + (enemy.openedDoor ? " Kapı açıldı!" : "")
                : label + " vuruldu! (" + enemy.hp + " HP kaldı)");
        }
        Destroy(gameObject);
    }

    bool StartsWith(string n, string[] prefixes)
    {
        foreach (var p in prefixes) if (n.StartsWith(p)) return true;
        return false;
    }
}
