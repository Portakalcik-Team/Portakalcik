using UnityEngine;

// Nesneyi yukarı aşağı süzdürür.
public class Map5Bobber : MonoBehaviour
{
    [SerializeField] private float bobHeight = 0.25f;   // Kaç metre yukarı ve aşağı gideceği
    [SerializeField] private float bobSpeed = 2f;       // Ne kadar hızlı süzüleceği

    private Vector3 startLocalPosition;                 // Başlangıç yeri

    void Start()
    {
        // Oyun başlarken nerede olduğumuzu hatırla
        startLocalPosition = transform.localPosition;
    }

    void Update()
    {
        // Sin dalgası -1 ile +1 arasında gidip gelir; bobHeight ile çarpınca -0.25 ile +0.25 olur
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        // Başlangıç yerinin biraz üstüne veya altına koy
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}
