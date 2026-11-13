using UnityEngine;
using UnityEngine.InputSystem;

public class M10PlayerMove : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5.5f;
    [SerializeField] private float acceleration = 45f;
    [SerializeField] private float turnSpeed = 720f;
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private InputAction moveAction;
    private Vector3 direction;
    private float grip = 1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        // Harita üretilmiş mi ve güncel mi kontrol et, değilse hemen üret
        Map10TreeManager.EnsureMap();

        // Null-safe input action
        if (InputSystem.actions != null)
        {
            moveAction = InputSystem.actions.FindAction("Move");
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Kamerayı Player'ın child'ından çıkar (varsa)
        if (cameraTransform != null && cameraTransform.parent == transform)
        {
            cameraTransform.SetParent(null);
        }

        // Kameraya Map10CameraFollow ekle (yoksa)
        if (cameraTransform != null && cameraTransform.GetComponent<Map10CameraFollow>() == null)
        {
            cameraTransform.gameObject.AddComponent<Map10CameraFollow>();
        }

        // Player'a Map10PlayerCarry ekle (yoksa)
        if (GetComponent<Map10PlayerCarry>() == null)
        {
            gameObject.AddComponent<Map10PlayerCarry>();
        }

        // Player'a güzel karakter görünümü ekle (yoksa)
        if (GetComponent<Map10PlayerVisuals>() == null)
        {
            gameObject.AddComponent<Map10PlayerVisuals>();
        }

        // Haritadaki ağaçları dönüştürecek TreeManager'ı başlat (yoksa)
        if (FindFirstObjectByType<Map10TreeManager>() == null)
        {
            GameObject treeMgr = new GameObject("Map10TreeManager");
            treeMgr.AddComponent<Map10TreeManager>();
        }

        // Player'a mandalina fırlatma sistemi ekle (yoksa)
        if (GetComponent<Map10PlayerShooter>() == null)
        {
            gameObject.AddComponent<Map10PlayerShooter>();
        }

        // Haritadaki canavarları dönüştürecek EnemyManager'ı başlat (yoksa)
        if (FindFirstObjectByType<Map10EnemyManager>() == null)
        {
            GameObject enemyMgr = new GameObject("Map10EnemyManager");
            enemyMgr.AddComponent<Map10EnemyManager>();
        }

        // Harita atmosferini ve görsellerini güzelleştirecek Environment'ı başlat (yoksa)
        if (FindFirstObjectByType<Map10Environment>() == null)
        {
            GameObject envMgr = new GameObject("Map10Environment");
            envMgr.AddComponent<Map10Environment>();
        }

        // Çıkış (Finish/Exit) objelerini Map10Finish ile donat
        foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj != null && (obj.name.ToLower().Contains("exit") || obj.name.ToLower().Contains("finish")))
            {
                if (obj.GetComponent<Map10Finish>() == null)
                {
                    obj.AddComponent<Map10Finish>();
                }
            }
        }
    }

    void Update()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        Vector2 input = Vector2.zero;

        // 1. New Input System Action
        if (moveAction != null)
        {
            input = moveAction.ReadValue<Vector2>();
        }

        // 2. Keyboard.current doğrudan kontrol (Fallback)
        if (input.sqrMagnitude < 0.01f && Keyboard.current != null)
        {
            float x = 0f;
            float y = 0f;

            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;

            input = new Vector2(x, y);
        }

        // 3. Legacy Input kontrol (Fallback)
        if (input.sqrMagnitude < 0.01f)
        {
            try
            {
                float lx = Input.GetAxisRaw("Horizontal");
                float ly = Input.GetAxisRaw("Vertical");
                if (Mathf.Abs(lx) > 0.01f || Mathf.Abs(ly) > 0.01f)
                {
                    input = new Vector2(lx, ly);
                }
            }
            catch { }
        }

        // Kamera yönüne göre dünya yönü hesapla
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        if (cameraTransform != null)
        {
            forward = cameraTransform.forward;
            right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        direction = forward * input.y + right * input.x;

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 wanted = direction * maxSpeed;
        Vector3 v = rb.linearVelocity;
        Vector3 flat = new Vector3(v.x, 0f, v.z);

        float step = acceleration * grip * Time.fixedDeltaTime;
        flat = Vector3.MoveTowards(flat, wanted, step);

        rb.linearVelocity = new Vector3(flat.x, v.y, flat.z);
    }

    public void SetGrip(float value)
    {
        grip = value;
    }
}
