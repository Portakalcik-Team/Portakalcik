using UnityEngine;
// Seviye 9: meyveler kendi etrafında döner
public class Map9Spinner : MonoBehaviour
{
// derece / SANİYE
[SerializeField] private float rotationSpeed = 90f;
void Update()
{
transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
}
}