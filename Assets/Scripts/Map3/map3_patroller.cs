using UnityEngine;

// Map_3 devriye canavari (W02 slayttaki Patroller yaklasimi):
// baslangic konumu (A) ile Point B arasinda gidip gelir.
// Bu script ana (bos) nesneyi tasir; gorsel donus/suzulme varsa sadece Visual cocugunda calisir.
public class map3_patroller : MonoBehaviour
{
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2.0f;   // metre / saniye

    private Vector3 pointA;
    private bool goingToB = true;

    void Start()   // A = basladigimiz yer
    {
        pointA = transform.position;
    }

    void Update()
    {
        if (pointB == null) return;   // Point B bos birakilirsa her karede hata vermesin

        Vector3 target = goingToB ? pointB.position : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        if (transform.position == target)
        {
            goingToB = !goingToB;   // vardik: geri don
        }
    }

    // Sahne gorunumunde devriye yolunu kirmizi cizgi olarak gosterir (oyuna etkisi yok).
    void OnDrawGizmos()
    {
        if (pointB == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(Application.isPlaying ? pointA : transform.position, pointB.position);
    }
}
