using UnityEngine;                                                          // Unity'nin temel sınıfları
using UnityEngine.InputSystem;                                              // Yeni Input System (InputAction) için gerekli

// Space / gamepad Güney tuşu: sadece yerdeyken zıpla.
public class M4PlayerJump : MonoBehaviour                                   // Oyuncuya eklenen zıplama scripti
{
    [SerializeField] private float jumpForce = 4f;                          // Zıplama itiş gücü; kütle arttıkça aynı güç daha az yükseltir
    // hangi katmanlar zemin sayılır: Ground seç!
    [SerializeField] private LayerMask groundLayers;                        // Zemin sayılacak katmanlar (Inspector'da Ground seçilir)

    private Rigidbody rb;                                                   // Oyuncunun fizik bedeni
    private InputAction jumpAction;                                         // "Jump" tuş eylemi

    void Start()                                                            // Oyun başlayınca bir kez çalışır
    {
        rb = GetComponent<Rigidbody>();                                     // Kendi Rigidbody bileşenini bul ve sakla
        jumpAction = InputSystem.actions.FindAction("Jump");                // Input ayarlarından "Jump" eylemini bul
    }

    void Update()                                                           // Her karede çalışır
    {
        // sadece tuşa basıldığı karede
        if (!jumpAction.WasPressedThisFrame()) return;                      // Bu karede zıplama tuşuna basılmadıysa çık
        if (!IsGrounded()) return;                                          // Havada zıplama yok

        // yukarı doğru tek bir itiş
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);             // Yukarı yönde tek seferlik anlık itiş uygula
    }

    // ayağımızın altında Ground var mı?
    bool IsGrounded()                                                       // Yerde miyiz sorusunu cevaplar
    {
        // ortamızdan 1.1 m aşağı: boyumuzun yarısı + 0.1
        return Physics.Raycast(transform.position, Vector3.down,            // Merkezden aşağı doğru ışın at
                               1.1f, groundLayers);                         // 1.1 m uzunluğunda, sadece zemin katmanlarına çarparsa true
    }
}
