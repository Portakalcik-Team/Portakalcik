using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// PLAYER'a ekle (Rigidbody + Collider gerekli). Değme olayları, envanter, can, süre, puan, ölüm, bölüm sonu.
// Ağaç      -> meyveler envantere girer, ağaç yerinde kalır (bir kez toplanır)
// Düşman    -> temasta hasar (düşmanın kendi hasar ve aralığıyla)
// Yeşil yer -> 1 portakalla bölüm biter, puan gösterilir
public class M1PlayerTrigger : MonoBehaviour
{
    // Oyun bitti (öldü ya da kazandı): diğer scriptler bunu kontrol edip durur
    public static bool Stopped;

    [Header("İsim başlangıçları (küçük harf)")]
    public string[] treePrefixes  = { "limon", "mandalina", "portakal" };
    public string[] enemyPrefixes = { "diken", "yarasa", "tilki", "golem", "muhafiz", "enemy" };
    public string[] exitPrefixes  = { "exit", "cikis" };

    [Header("Can")]
    public int hp = 100;
    public int damage = 4;            // düşmanda M1Enemy yoksa bu kullanılır
    public float hurtCooldown = 1f;

    [Header("Ağaç başına meyve (Seviye 1)")]
    public int mandalinaPerTree = 6;
    public int portakalPerTree = 2;
    public int limonPerTree = 2;

    [Header("Çıkış ve puan (Seviye 1)")]
    public bool requireOrange = true;     // çıkış için 1 portakal gerekir
    public float parTime = 120f;

    [Header("Envanter")]
    public int mandalina;
    public int portakal;
    public int limon;
    public int selectedFruit;         // 0 = mandalina, 1 = portakal, 2 = limon

    readonly HashSet<GameObject> harvested = new HashSet<GameObject>();
    string message = "";
    float messageUntil, nextHurtTime, nextExitMsg, startTime, endTime;
    bool dead, won;

    static readonly string[] FruitNames = { "Mandalina", "Portakal", "Limon" };

    public static string FruitName(int type) { return FruitNames[Mathf.Clamp(type, 0, 2)]; }

    public int GetFruit(int type)
    {
        return type == 0 ? mandalina : type == 1 ? portakal : limon;
    }

    public void AddFruit(int type, int n)
    {
        if (type == 0) mandalina += n;
        else if (type == 1) portakal += n;
        else limon += n;
    }

    void Awake()
    {
        Stopped = false;
        startTime = Time.time;
    }

    void Update()
    {
        if (Stopped && RestartPressed())
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);   // sahne Build Settings'te olmalı
    }

    // Collider "Is Trigger" ise
    void OnTriggerEnter(Collider other) { Touch(other.gameObject); }
    void OnTriggerStay(Collider other)  { Touch(other.gameObject); }
    // Normal collider ise
    void OnCollisionEnter(Collision c)  { Touch(c.gameObject); }
    void OnCollisionStay(Collision c)   { Touch(c.gameObject); }

    void Touch(GameObject obj)
    {
        if (Stopped) return;

        // Objeden yukarı çıkarak adı eşleşen ilk objeyi bul
        for (Transform t = obj.transform; t != null; t = t.parent)
        {
            string n = t.name.ToLower();

            if (StartsWith(n, treePrefixes)) { Harvest(t.gameObject, n); return; }

            if (StartsWith(n, exitPrefixes)) { TryExit(); return; }

            if (StartsWith(n, enemyPrefixes)) { Hurt(t.gameObject); return; }
        }
    }

    void Harvest(GameObject tree, string lowerName)
    {
        if (harvested.Contains(tree)) return;      // bu ağaç zaten toplandı
        harvested.Add(tree);

        int type, amount;
        if (lowerName.StartsWith("limon"))          { type = 2; amount = limonPerTree; }
        else if (lowerName.StartsWith("portakal"))  { type = 1; amount = portakalPerTree; }
        else                                        { type = 0; amount = mandalinaPerTree; }

        AddFruit(type, amount);
        Show(FruitName(type) + " toplandı! +" + amount);
        Darken(tree);                               // ağaç kalır, sadece soluk görünür
    }

    void Darken(GameObject tree)
    {
        foreach (var r in tree.GetComponentsInChildren<Renderer>())
        {
            if (r.gameObject.name == "Meyve") { r.gameObject.SetActive(false); continue; }   // toplanan meyveler kaybolur
            var m = r.material;                     // bu ağaca özel kopya
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", m.GetColor("_BaseColor") * 0.45f);
            else if (m.HasProperty("_Color")) m.SetColor("_Color", m.GetColor("_Color") * 0.45f);
        }
    }

    void Hurt(GameObject enemyObj)
    {
        if (Time.time < nextHurtTime) return;

        var en = enemyObj.GetComponent<M1Enemy>();
        int dmg = en != null ? en.damage : damage;
        float cd = en != null ? en.attackInterval : hurtCooldown;
        nextHurtTime = Time.time + cd;

        hp = Mathf.Max(0, hp - dmg);
        if (hp <= 0)
        {
            dead = true;
            Stopped = true;
            endTime = Time.time;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Show("ÖLDÜN!  R tuşu: yeniden başla", 999f);
            return;
        }
        Show("Can azaldı! -" + dmg + "  Kalan: " + hp);
    }

    void TryExit()
    {
        if (requireOrange && portakal < 1)
        {
            if (Time.time >= nextExitMsg) { Show("Çıkış için 1 portakal lazım!"); nextExitMsg = Time.time + 2f; }
            return;
        }
        if (requireOrange) portakal--;

        won = true;
        Stopped = true;
        endTime = Time.time;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        float elapsed = endTime - startTime;
        // Denge Sistemi: 500 + seviye*100 + süre bonusu*3 + kullanılmayan mandalina*20 + portakal*30 (limon soruları henüz yok)
        int score = 600 + Mathf.Max(0, Mathf.RoundToInt(parTime - elapsed)) * 3 + mandalina * 20 + portakal * 30;
        Show("BÖLÜM TAMAMLANDI!  Süre: " + Mathf.RoundToInt(elapsed) + " sn   Puan: " + score + "   (R: tekrar)", 999f);
    }

    bool StartsWith(string n, string[] prefixes)
    {
        foreach (var p in prefixes) if (n.StartsWith(p)) return true;
        return false;
    }

    public void Show(string text, float seconds = 2f)
    {
        message = text;
        messageUntil = Time.time + seconds;
        Debug.Log(text);
    }

    bool RestartPressed()
    {
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        return k != null && k.rKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.R);
#endif
    }

    void OnGUI()
    {
        var small = new GUIStyle(GUI.skin.label) { fontSize = 20 };
        float t = (Stopped ? endTime : Time.time) - startTime;
        GUI.Label(new Rect(15, 10, 900, 30), "Can: " + hp + "     Süre: " + Mathf.RoundToInt(t) + " / " + Mathf.RoundToInt(parTime) + " sn", small);

        string[] counts = { mandalina.ToString(), portakal.ToString(), limon.ToString() };
        string line = "";
        for (int i = 0; i < 3; i++)
            line += (i == selectedFruit ? "[" : " ") + (i + 1) + " " + FruitNames[i] + ": " + counts[i] + (i == selectedFruit ? "]" : " ") + "   ";
        GUI.Label(new Rect(15, 38, 900, 30), line, small);

        // Nişangah
        if (!Stopped) GUI.Label(new Rect(Screen.width / 2f - 6f, Screen.height / 2f - 14f, 30, 30), "+", small);

        if (Time.time >= messageUntil) return;
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 32,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        GUI.Label(new Rect(0, 80, Screen.width, 60), message, style);
    }
}
