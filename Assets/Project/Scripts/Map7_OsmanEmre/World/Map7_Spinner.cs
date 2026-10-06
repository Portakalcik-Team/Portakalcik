using UnityEngine;

public class Map7_Spinner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }


    [SerializeField]
    private float
rotationSpeed = 90f;
    void Update()
    {
transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);

    }
}
