using UnityEngine;

// Seviye 9'a özel: Labirent Muhafızı sağa sola bakınır
public class Map9Sentry : MonoBehaviour
{
    [SerializeField] private float swingAngle = 60f;   // sağa ve sola kaç derece
    [SerializeField] private float swingSpeed = 1f;    // ne kadar hızlı bakınır

    private float startY;

    void Start()
    {
        startY = transform.localEulerAngles.y;         // baştaki bakış yönü
    }

    void Update()
    {
        // Sin -1..+1 arası; swingAngle ile çarpınca -60..+60 derece
        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        transform.localEulerAngles = new Vector3(0f, startY + angle, 0f);
    }
}