using UnityEngine;


public class M1FirstPersonCamera : MonoBehaviour
{
    public Transform player;                 // boşsa "Player" otomatik bulunur
    public float headHeight = 0.7f;          // Player'ın merkezinden yukarı mesafe
    public float mouseSensitivity = 0.15f;
    public float maxLookUp = 80f;

    public static float Yaw { get; private set; }

    float pitch;

    void Start()
    {
        FindPlayer();
        var cam = GetComponent<Camera>();
        if (cam != null) cam.nearClipPlane = 0.05f;

        Yaw = transform.eulerAngles.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Esc ile fareyi serbest bırak, tıklayınca tekrar kilitle
        if (Pressed(KeyCode.Escape)) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            if (MouseClicked()) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
            return;
        }

        Vector2 d = MouseDelta() * mouseSensitivity;
        Yaw += d.x;
        pitch = Mathf.Clamp(pitch - d.y, -maxLookUp, maxLookUp);
    }

    // Player oyun başlayınca kurulabildiği için bulunana kadar her karede arar
    void FindPlayer()
    {
        if (player != null) return;
        var p = GameObject.Find("Player");
        if (p == null) p = GameObject.Find("Player Prefab");
        if (p == null) p = GameObject.Find("Player(Clone)");
        if (p != null) player = p.transform;
    }

    void LateUpdate()
    {
        if (player == null) { FindPlayer(); if (player == null) return; }
        transform.position = player.position + Vector3.up * headHeight;
        transform.rotation = Quaternion.Euler(pitch, Yaw, 0f);
    }

    Vector2 MouseDelta()
    {
#if ENABLE_INPUT_SYSTEM
        var m = UnityEngine.InputSystem.Mouse.current;
        return m != null ? m.delta.ReadValue() : Vector2.zero;
#else
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10f;
#endif
    }

    bool MouseClicked()
    {
#if ENABLE_INPUT_SYSTEM
        var m = UnityEngine.InputSystem.Mouse.current;
        return m != null && m.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    bool Pressed(KeyCode key)
    {
#if ENABLE_INPUT_SYSTEM
        var k = UnityEngine.InputSystem.Keyboard.current;
        return k != null && key == KeyCode.Escape && k.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(key);
#endif
    }
}