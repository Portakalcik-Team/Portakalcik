using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncuyu W A S D tuşlarıyla yürütür.
public class Map5PlayerMover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;   // Saniyede kaç metre yürüyeceği

    private CharacterController controller;          // Duvarlardan geçmemizi engelleyen bileşen

    void Start()
    {
        // Aynı nesnedeki CharacterController'ı bul
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Hangi tuşa basıldığına göre yön belirle
        Vector3 direction = Vector3.zero;
        if (Keyboard.current.wKey.isPressed) direction.z += 1f;   // ileri
        if (Keyboard.current.sKey.isPressed) direction.z -= 1f;   // geri
        if (Keyboard.current.dKey.isPressed) direction.x += 1f;   // sağ
        if (Keyboard.current.aKey.isPressed) direction.x -= 1f;   // sol

        // O yöne doğru yürü (Time.deltaTime ile hız her bilgisayarda aynı)
        controller.Move(direction * moveSpeed * Time.deltaTime);
    }
}
