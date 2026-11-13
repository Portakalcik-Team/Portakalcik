using UnityEngine;
using UnityEngine.InputSystem;

public class Map10PlayerShooter : MonoBehaviour
{
    [Header("Atış Ayarları")]
    [SerializeField] private float shootCooldown = 0.22f;
    [SerializeField] private float bulletScale = 0.22f;

    private Map10PlayerCarry carry;
    private float nextShootTime = 0f;
    private Material bulletMaterial;
    private Material leafMaterial;

    void Start()
    {
        carry = GetComponent<Map10PlayerCarry>();

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        bulletMaterial = new Material(shader) { color = new Color(1f, 0.5f, 0f) };
        leafMaterial = new Material(shader) { color = new Color(0.2f, 0.75f, 0.2f) };
    }

    void Update()
    {
        bool shootPressed = false;

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            shootPressed = true;
        }
        else if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            shootPressed = true;
        }

        if (shootPressed && Time.time >= nextShootTime)
        {
            TryShootMandarin();
        }
    }

    private void TryShootMandarin()
    {
        if (carry == null) carry = GetComponent<Map10PlayerCarry>();

        if (carry == null || carry.MandarinCount <= 0)
        {
            Debug.Log("[Map10] Fırlatacak mandalina yok! Ağaçlardan mandalina topla.");
            return;
        }

        // Belden 1 mandalina harca
        if (carry.UseMandarin(1))
        {
            nextShootTime = Time.time + shootCooldown;
            Shoot();
        }
    }

    private void Shoot()
    {
        Camera cam = Camera.main;
        Vector3 aimOrigin = cam != null ? cam.transform.position : (transform.position + Vector3.up * 1.35f);
        Vector3 aimDirection = cam != null ? cam.transform.forward : transform.forward;

        // Silahın geriye tepme efektini tetikle
        if (Map10CameraFollow.Instance != null)
        {
            Map10CameraFollow.Instance.TriggerGunRecoil();
        }

        // 1. DİP DİBE / YAKIN MESAFE KONTROLÜ:
        // Eğer oyuncunun hemen önünde (1.3 metreye kadar) bir düşman varsa anında doğrudan vur!
        RaycastHit hit;
        if (Physics.SphereCast(aimOrigin, 0.45f, aimDirection, out hit, 1.3f))
        {
            Map10Enemy directEnemy = hit.collider.GetComponentInParent<Map10Enemy>();
            if (directEnemy != null)
            {
                directEnemy.TakeDamage(1, hit.point);
                SpawnInstantSplash(hit.point);
                Debug.Log("[Map10] Yakın mesafeden doğrudan mandalina isabeti!");
                return;
            }
        }

        // 2. NORMAL MERMİ FIRLATMA (Namlu ucundan crosshair yönüne):
        Vector3 spawnPos = aimOrigin + aimDirection * 0.5f;
        if (cam != null)
        {
            spawnPos += cam.transform.right * 0.18f - cam.transform.up * 0.12f;
        }

        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.name = "MandarinBullet";
        bullet.transform.position = spawnPos;
        bullet.transform.localScale = Vector3.one * bulletScale;

        Renderer rend = bullet.GetComponent<Renderer>();
        if (rend != null && bulletMaterial != null)
        {
            rend.material = bulletMaterial;
        }

        // Minik yaprak
        GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaf.name = "Leaf";
        leaf.transform.SetParent(bullet.transform);
        leaf.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        leaf.transform.localScale = new Vector3(0.35f, 0.15f, 0.2f);
        Collider leafCol = leaf.GetComponent<Collider>();
        if (leafCol != null) Destroy(leafCol);
        Renderer leafRend = leaf.GetComponent<Renderer>();
        if (leafRend != null && leafMaterial != null)
        {
            leafRend.material = leafMaterial;
        }

        // Oyuncu collider'ını yoksay
        Collider playerCol = GetComponent<Collider>();
        Collider bulletCol = bullet.GetComponent<Collider>();
        if (playerCol != null && bulletCol != null)
        {
            Physics.IgnoreCollision(playerCol, bulletCol);
        }

        Map10MandarinBullet bulletScript = bullet.AddComponent<Map10MandarinBullet>();
        bulletScript.Launch(aimDirection);
    }

    private void SpawnInstantSplash(Vector3 pos)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material orangeMat = new Material(shader) { color = new Color(1f, 0.45f, 0f) };

        for (int i = 0; i < 6; i++)
        {
            GameObject splash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            splash.name = "MandarinJuice";
            splash.transform.position = pos + Random.insideUnitSphere * 0.15f;
            splash.transform.localScale = Vector3.one * Random.Range(0.08f, 0.14f);
            Collider c = splash.GetComponent<Collider>();
            if (c != null) Destroy(c);
            splash.GetComponent<Renderer>().material = orangeMat;

            Rigidbody sRb = splash.AddComponent<Rigidbody>();
            sRb.mass = 0.05f;
            sRb.linearVelocity = (Random.onUnitSphere + Vector3.up * 0.8f) * Random.Range(2f, 4f);
            Destroy(splash, 0.5f);
        }
    }
}
