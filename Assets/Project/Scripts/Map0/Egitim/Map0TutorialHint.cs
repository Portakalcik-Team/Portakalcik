using UnityEngine;

// sahneye konan görünmez kutu: oyuncu içine girince ipucu panelini açar
[RequireComponent(typeof(BoxCollider))]
public class Map0TutorialHint : MonoBehaviour
{
    [SerializeField] private int stepNumber;               // eğitimdeki sıra
    [SerializeField] private string title;                 // panel başlığı
    [TextArea(2, 5)]
    [SerializeField] private string body;                  // panel metni
    [SerializeField] private Map0MonsterData monster;      // boş olabilir

    // dışarıdan sadece okunur
    public int StepNumber => stepNumber;
    public string Title => title;
    public string Body => body;
    public Map0MonsterData Monster => monster;

    // builder bu metodu çağırıp alanları doldurur
    public void Configure(int step, string newTitle, string newBody, Map0MonsterData newMonster)
    {
        stepNumber = step;
        title = newTitle;
        body = newBody;
        monster = newMonster;
    }

    // bileşen ilk eklendiğinde çalışır: kutu trigger olsun
    void Reset()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null) box.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ilerlemeyi say
        if (Map0TutorialProgress.Instance != null)
            Map0TutorialProgress.Instance.MarkSeen(this);

        // paneli aç
        if (Map0HintPanel.Instance != null)
            Map0HintPanel.Instance.Show(this);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // paneli kapat (Unity nesnesinde ?. kullanmıyoruz)
        if (Map0HintPanel.Instance != null)
            Map0HintPanel.Instance.Hide(this);
    }

    // editörde alanı görebilelim
    void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        if (box == null) return;

        Gizmos.matrix = transform.localToWorldMatrix;

        // canavar ipucu turuncu, normal ipucu sarı
        Color color = monster != null ? new Color(1f, 0.55f, 0f) : Color.yellow;

        // önce yarı saydam dolgu
        Gizmos.color = new Color(color.r, color.g, color.b, 0.15f);
        Gizmos.DrawCube(box.center, box.size);

        // sonra kenar çizgisi
        Gizmos.color = color;
        Gizmos.DrawWireCube(box.center, box.size);

        // diğer gizmolara karışmasın
        Gizmos.matrix = Matrix4x4.identity;
    }
}
