using UnityEngine;
using Unity.Cinemachine;

// Tehlike alanına veya engeline çarpıldığında oyuncunun kargosunu düşürür
public class Map6Hazard : MonoBehaviour
{
    [Header("Kamera Sarsıntısı (Impulse) Özelliği")]
    [SerializeField] private CinemachineImpulseSource shake = null;

    void Start()
    {
        if (shake == null) shake = GetComponent<CinemachineImpulseSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Gelen nesnede veya ebeveyninde Map6PlayerCarry ara
        Map6PlayerCarry carry = other.GetComponent<Map6PlayerCarry>();
        if (carry == null) carry = other.GetComponentInParent<Map6PlayerCarry>();

        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Kargoyu düşür, sarsıntı tetikle ve konsola bildir
        carry.DropCargo();
        if (shake != null) shake.GenerateImpulse();
        Debug.Log(name + " kargoyu düşürdü!");
    }
}
