using UnityEngine;

// Player'a ekle (Rigidbody + Collider olmalı). Sadece değme olaylarını yönetir, hareket yok.
// Ağaç      -> "... toplandı!" + ağaç kaybolur
// Kırmızı   -> "Can azaldı!"
// Yeşil yer -> "Bölüm tamamlandı!"
public class PlayerTrigger : MonoBehaviour
{
    [Header("İsim başlangıçları (küçük harf)")]
    public string[] treePrefixes  = { "limon", "mandalina", "portakal" };
    public string[] enemyPrefixes = { "diken", "yarasa", "tilki", "golem", "muhafiz", "enemy" };
    public string[] exitPrefixes  = { "exit", "cikis" };

    [Header("Can")]
    public int hp = 100;
    public int damage = 4;            // Seviye 1 Diken Böceği hasarı
    public float hurtCooldown = 1f;   // üst üste hasar yemesin

    public int toplananAgac;

    string message = "";
    float messageUntil, nextHurtTime;
    bool levelDone;

    // Collider "Is Trigger" ise
    void OnTriggerEnter(Collider other) { Touch(other.gameObject); }
    void OnTriggerStay(Collider other)  { Touch(other.gameObject); }
    // Normal collider ise
    void OnCollisionEnter(Collision c)  { Touch(c.gameObject); }
    void OnCollisionStay(Collision c)   { Touch(c.gameObject); }

    void Touch(GameObject obj)
    {
        if (levelDone) return;

        // Objeden yukarı çıkarak adı eşleşen ilk objeyi bul
        for (Transform t = obj.transform; t != null; t = t.parent)
        {
            string n = t.name.ToLower();

            if (StartsWith(n, treePrefixes))
            {
                toplananAgac++;
                Show(t.name.Split('_')[0] + " toplandı!");
                Destroy(t.gameObject);
                return;
            }
            if (StartsWith(n, exitPrefixes))
            {
                levelDone = true;
                Show("Bölüm tamamlandı!", 999f);
                return;
            }
            if (StartsWith(n, enemyPrefixes))
            {
                if (Time.time < nextHurtTime) return;
                nextHurtTime = Time.time + hurtCooldown;
                hp = Mathf.Max(0, hp - damage);
                Show("Can azaldı! Kalan: " + hp);
                return;
            }
        }
    }

    bool StartsWith(string n, string[] prefixes)
    {
        foreach (var p in prefixes) if (n.StartsWith(p)) return true;
        return false;
    }

    void Show(string text, float seconds = 2f)
    {
        message = text;
        messageUntil = Time.time + seconds;
        Debug.Log(text);
    }

    void OnGUI()
    {
        var small = new GUIStyle(GUI.skin.label) { fontSize = 20 };
        GUI.Label(new Rect(15, 10, 600, 30), $"Can: {hp}   Toplanan ağaç: {toplananAgac}", small);

        if (Time.time >= messageUntil) return;
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 32,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        GUI.Label(new Rect(0, 40, Screen.width, 60), message, style);
    }
}
