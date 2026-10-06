using UnityEngine;
// Seviye 9: meyveler ve yarasalar yukarı aşağı yüzer
public class Map9Bobber : MonoBehaviour{
 [SerializeField] private float bobHeight = 0.25f; // metre, yukarı ve aşağı
 [SerializeField] private float bobSpeed = 2f; // ne kadar hızlı yüzer

private Vector3 startLocalPosition;
void Start(){
startLocalPosition = transform.localPosition;
}
void Update(){
// Sin -1..+1 arası gider; offset -0.25..+0.25 arası olur
float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
transform.localPosition = startLocalPosition + Vector3.up * offset;
}
}