using UnityEngine;

// Map_3: W02 slayttaki Bobber. Visual cocugunu yukari asagi ziplatir (ana nesneye degil).
public class map3_bobber : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.25f;   // metre yukari ve asagi
    [SerializeField] private float bobSpeed = 2f;       // ne kadar hizli ziplar

    private Vector3 startLocalPosition;

    void Start()   // basladigimiz yeri hatirla
    {
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Time.time saatten hesaplar, her kare bir sey eklemedigi icin Time.deltaTime gerekmez.
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}
