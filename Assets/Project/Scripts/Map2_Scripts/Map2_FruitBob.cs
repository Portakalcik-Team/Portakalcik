using UnityEngine;
public class Map2_FruitBob : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 90f;    // degrees per second
    [SerializeField] private float bobHeight = 0.15f;  // metres up and down
    [SerializeField] private float bobSpeed = 2f;      // how fast it floats
    private Vector3 startLocalPosition;
    private float timer = 0f;                          // this fruit's own clock, in seconds
    void Start()
    {
        startLocalPosition = transform.localPosition;
    }
    void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);    // spin: degrees per second
        timer += Time.deltaTime;                                 // float: our own clock
        float offset = Mathf.Sin(timer * bobSpeed) * bobHeight;  // between -0.15 and +0.15
        transform.localPosition = startLocalPosition + Vector3.up * offset;
    }
}