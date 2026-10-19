using UnityEngine;

// Map_3: kamera oyuncuyu ustten-egik aciyla yumusakca takip eder (slayt disi iyilestirme).
// Hedef bos birakilirsa kamera sahnedeki yerinde kalir (Hafta 2'deki genel gorunum).
public class M3KameraTakip : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 13f, -7.5f);
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 velocity;

    void Start()   // Play baslarken genel gorunumden oyuncuya suzulmesin, dogrudan yerinde baslasin
    {
        if (target == null) return;
        transform.position = target.position + offset;
        transform.rotation = Quaternion.LookRotation(-offset, Vector3.up);
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 wanted = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, wanted, ref velocity, smoothTime);
        transform.rotation = Quaternion.LookRotation(target.position - wanted, Vector3.up);
    }
}
