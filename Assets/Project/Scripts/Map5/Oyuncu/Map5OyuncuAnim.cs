using UnityEngine;

// Büyücü modelinin animasyonunu seçer: yürüyorsa "Walk", duruyorsa "Idle".
public class Map5OyuncuAnim : MonoBehaviour
{
    private Rigidbody rb;
    private Animation anim;   // modelin içindeki animasyon bileşeni

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animation>();
    }

    void Update()
    {
        if (anim == null) return;

        // Sadece yatay hıza bak (zıplama sayılmasın)
        Vector3 v = rb.linearVelocity;
        float speed = new Vector3(v.x, 0f, v.z).magnitude;

        if (speed > 0.5f)
            anim.CrossFade("Walk", 0.15f);
        else
            anim.CrossFade("Idle", 0.15f);
    }
}
