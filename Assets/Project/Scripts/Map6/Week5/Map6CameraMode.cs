using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

// Varsayılan FPS ve C tuşu ile Kuşbakışı (Week 5) kamera modları arasında geçiş yapar
public class Map6CameraMode : MonoBehaviour
{
    [Header("Kamera Modu Özelliği")]
    [SerializeField] private bool startInFirstPerson = true; // Varsayılan FPS başlangıcı
    [SerializeField] private CinemachineCamera fpsCamera = null;
    [SerializeField] private CinemachineCamera topDownCamera = null;
    [SerializeField] private int activeFpsPriority = 30;
    [SerializeField] private int inactiveFpsPriority = 5;

    [Header("FPS ve Kuşbakışı Taşıma Noktası Özelliği")]
    [SerializeField] private Transform fpsCameraAnchor = null;
    [SerializeField] private Transform carryPoint = null;
    [SerializeField] private Vector3 fpsCarryLocalPosition = new Vector3(0.35f, -0.28f, 0.7f);
    [SerializeField] private Vector3 topDownCarryLocalPosition = new Vector3(0f, 1.6f, 0f);
    [SerializeField] private string toggleKeyPath = "<Keyboard>/c";

    private Map6PlayerMove playerMove;
    private Map6MouseLook mouseLook;
    private Map6PlayerVisuals playerVisuals;
    private InputAction toggleAction;
    private bool isFirstPerson;

    void Awake()
    {
        playerMove = GetComponent<Map6PlayerMove>();
        mouseLook = GetComponent<Map6MouseLook>();
        playerVisuals = GetComponent<Map6PlayerVisuals>();
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(toggleKeyPath))
        {
            toggleAction = new InputAction("ToggleCameraMode", InputActionType.Button);
            toggleAction.AddBinding(toggleKeyPath);
            toggleAction.Enable();
        }

        ApplyMode(startInFirstPerson);
    }

    void OnDestroy()
    {
        if (toggleAction != null)
        {
            toggleAction.Disable();
            toggleAction.Dispose();
        }
    }

    void Update()
    {
        bool pressed = toggleAction != null
            ? toggleAction.WasPressedThisFrame()
            : (Keyboard.current != null && Keyboard.current.cKey.wasPressedThisFrame);

        if (pressed)
        {
            ApplyMode(!isFirstPerson);
        }
    }

    public void ApplyMode(bool fps)
    {
        isFirstPerson = fps;

        if (fpsCamera != null)
        {
            fpsCamera.Priority.Value = fps ? activeFpsPriority : inactiveFpsPriority;
            fpsCamera.gameObject.SetActive(fps);
        }

        if (topDownCamera != null && !topDownCamera.gameObject.activeSelf)
        {
            topDownCamera.gameObject.SetActive(true);
        }

        if (playerMove != null)
        {
            playerMove.SetFirstPersonMode(fps);
        }

        if (mouseLook != null)
        {
            mouseLook.SyncYawFromBody();
            mouseLook.SetMouseLookEnabled(fps);
        }

        if (playerVisuals != null)
        {
            playerVisuals.SetHeadVisible(!fps);
        }

        if (carryPoint != null)
        {
            if (fps && fpsCameraAnchor != null)
            {
                carryPoint.SetParent(fpsCameraAnchor, false);
                carryPoint.localPosition = fpsCarryLocalPosition;
            }
            else
            {
                carryPoint.SetParent(transform, false);
                carryPoint.localPosition = topDownCarryLocalPosition;
            }

            carryPoint.localRotation = Quaternion.identity;
        }
    }
}
