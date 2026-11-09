using UnityEngine;
using Unity.Cinemachine;   // W05: Cinemachine sınıflarını kullanmak için

// Kırmızı tehlike: kargo taşıyan oyuncunun kargosunu düşürür.
// W05: kargo düşünce kamerayı kısa bir an sarsar.
public class Map5Hazard : MonoBehaviour
{
    private CinemachineImpulseSource shake;   // null = sarsıntı yok

    void Start()
    {
        // Bu tehlikenin üzerindeki Impulse Source (varsa)
        shake = GetComponent<CinemachineImpulseSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Kim? Kargo taşıyan oyuncu
        Map5PlayerCarry carry = other.GetComponent<Map5PlayerCarry>();
        if (carry == null) return;
        if (!carry.IsCarrying()) return;

        // Yap: kargoyu düşür
        carry.DropCargo();
        Debug.Log(name + " kargoyu düşürdü!");

        // W05: ...ve kameraya kısa bir sarsıntı gönder
        if (shake != null) shake.GenerateImpulse();
    }
}
