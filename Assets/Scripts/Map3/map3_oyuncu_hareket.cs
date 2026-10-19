using UnityEngine;
using UnityEngine.InputSystem;

// Map_3: basit WASD / yon tuslari hareketi (slayt disi oynanabilirlik iyilestirmesi; slaytta hareket Hafta 4).
// Dinamik Rigidbody'yi FixedUpdate icinde kuvvetle surer, transform.position kullanmaz.
// Buzda (dusuk surtunmeli zemin) kontrol zayiflar ve tus birakilinca kayma devam eder.
// SelectionBase: Scene'de kapsule tiklayinca Visual degil, fizik govdeli kok secilir (surukleme testi icin).
[SelectionBase]
[RequireComponent(typeof(Rigidbody))]
public class map3_oyuncu_hareket : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;          // metre / saniye
    [SerializeField] private float acceleration = 30f;      // normal zeminde hizlanma/frenleme (m/s^2)
    [SerializeField] private float iceAcceleration = 3f;    // buzda kontrol (m/s^2)
    [SerializeField] private float iceFrictionLimit = 0.1f; // bu degerin altindaki Dynamic Friction = buz
    [SerializeField] private float turnSpeed = 720f;        // derece / saniye

    private Rigidbody rb;
    private Collider body;
    private Vector2 input;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        body = GetComponent<Collider>();
    }

    void Update()   // tuslari her kare oku
    {
        input = Vector2.zero;
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
        input = Vector2.ClampMagnitude(input, 1f);
    }

    void FixedUpdate()   // fizik hareketi
    {
        if (rb.isKinematic) return;   // Scene gorunumunden surukleme testi icin kinematic yapilabilir

        Vector3 direction = new Vector3(input.x, 0f, input.y);
        bool onIce = IsOnIce();

        Vector3 velocity = rb.linearVelocity;
        Vector3 flat = new Vector3(velocity.x, 0f, velocity.z);

        // buzda tus basili degilse hicbir sey yapma: kayma fizik materyaline kalir
        if (direction != Vector3.zero || !onIce)
        {
            Vector3 wanted = direction * moveSpeed;
            float maxChange = (onIce ? iceAcceleration : acceleration) * Time.fixedDeltaTime;
            Vector3 change = Vector3.ClampMagnitude(wanted - flat, maxChange);
            rb.AddForce(change, ForceMode.VelocityChange);
        }

        // gidilen yone don (sadece Y ekseni)
        if (direction != Vector3.zero)
        {
            Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, target, turnSpeed * Time.fixedDeltaTime));
        }
    }

    // ayagin altindaki collider'in fizik materyali kaygan mi?
    private bool IsOnIce()
    {
        float footDistance = body != null ? body.bounds.extents.y + 0.15f : 1.1f;
        if (Physics.Raycast(rb.position, Vector3.down, out RaycastHit hit, footDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            PhysicsMaterial material = hit.collider.sharedMaterial;
            return material != null && material.dynamicFriction < iceFrictionLimit;
        }
        return false;
    }
}
