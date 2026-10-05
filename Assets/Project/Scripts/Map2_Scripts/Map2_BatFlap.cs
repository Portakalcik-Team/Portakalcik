using UnityEngine;
public class Map2_BatFlap : MonoBehaviour
{
    [SerializeField] private Transform leftWing;        // Wing_L_Pivot
    [SerializeField] private Transform rightWing;       // Wing_R_Pivot
    [SerializeField] private float flapAngle = 35f;     // degrees up and down
    [SerializeField] private float flapSpeed = 12f;     // wing beats - fast
    [SerializeField] private float bobHeight = 0.25f;   // metres the body rises and falls
    [SerializeField] private float bobSpeed = 2f;       // body - slow
    private Vector3 startLocalPosition;
    private float timer = 0f;                           // this bat's own clock, in seconds
    void Start()
    {
        startLocalPosition = transform.localPosition;
    }
    void Update()
    {
        timer += Time.deltaTime;                        // add the seconds since the last frame
        float wing = Mathf.Sin(timer * flapSpeed) * flapAngle;   // between -35 and +35
        leftWing.localEulerAngles = new Vector3(0f, 0f, -wing);   // mirrored, so both wings
        rightWing.localEulerAngles = new Vector3(0f, 0f, wing);   // go up and down together
        float bob = Mathf.Sin(timer * bobSpeed) * bobHeight;      // slow rise and fall
        transform.localPosition = startLocalPosition + Vector3.up * bob;
    }
}