using UnityEngine;

// Seviye 9: çıkış karesi nabız gibi büyüyüp küçülür
public class Map9Pulser : MonoBehaviour
{
    [SerializeField] private float pulseAmount = 0.1f;   // 0.1 = yüzde 10
    [SerializeField] private float pulseSpeed = 3f;

    private Vector3 startScale;
    
    void Start()
    {
        startScale = transform.localScale;               // baştaki boyut
    }

    void Update()
    {
        // Sin -1..+1 arası; factor 0.9 ile 1.1 arasında gidip gelir
        float factor = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = startScale * factor;
    }
}