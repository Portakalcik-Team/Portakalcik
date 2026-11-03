using UnityEngine;
using Unity.Cinemachine;

public class Map0Hazard : MonoBehaviour
{
    private CinemachineImpulseSource shake; // yoksa null: sarsıntı yok

    void Start()
    {
        // aynı tehlikedeki Impulse Source, varsa
        shake = GetComponent<CinemachineImpulseSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Kim? kargo taşıyan oyuncu
        Map0PlayerCarry carry = other.GetComponent<Map0PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu düşür
        carry.DropCargo();
        Debug.Log(name + " kargoyu düşürdü!");

        // ...ve kameraya kısa bir vuruş
        if (shake != null) shake.GenerateImpulse();
    }
}
