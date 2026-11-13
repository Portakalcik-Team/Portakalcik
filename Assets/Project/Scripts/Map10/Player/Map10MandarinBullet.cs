using UnityEngine;

public class Map10MandarinBullet : MonoBehaviour
{
    [SerializeField] private float speed = 14f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private int damage = 1;

    private Rigidbody rb;
    private bool hasHit = false;

    public void Launch(Vector3 direction)
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;
        rb.mass = 0.3f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Hafif yukarı eğim vererek gerçekçi meyve atışı hissi
        Vector3 launchDir = (direction.normalized + Vector3.up * 0.15f).normalized;
        rb.linearVelocity = launchDir * speed;

        // Kendi etrafında tatlı dönme
        rb.angularVelocity = Random.insideUnitSphere * 15f;

        // Başlangıç noktasında hemen düşman var mı kontrol et (yakın mesafe güvenliği)
        Collider[] overlaps = Physics.OverlapSphere(transform.position, 0.4f);
        foreach (Collider col in overlaps)
        {
            if (col.gameObject != gameObject && !col.CompareTag("Player"))
            {
                Map10Enemy enemy = col.GetComponentInParent<Map10Enemy>();
                if (enemy != null)
                {
                    HandleHit(col.gameObject);
                    return;
                }
            }
        }

        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleHit(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // Oyuncuya veya diğer mermilere çarpmasın
        if (other.CompareTag("Player") || other.GetComponent<Map10MandarinBullet>() != null)
            return;

        HandleHit(other.gameObject);
    }

    private void HandleHit(GameObject hitObj)
    {
        if (hasHit) return;

        // Düşmana çarptı mı?
        Map10Enemy enemy = hitObj.GetComponentInParent<Map10Enemy>();
        if (enemy != null)
        {
            hasHit = true;
            enemy.TakeDamage(damage, transform.position);
            SpawnSplashEffect();
            Destroy(gameObject);
            return;
        }

        // Duvara veya zemine çarpınca da küçük meyve sıçraması yapıp silinsin
        if (!hitObj.CompareTag("Player"))
        {
            hasHit = true;
            SpawnSplashEffect();
            Destroy(gameObject);
        }
    }

    private void SpawnSplashEffect()
    {
        // Mandalina çarpınca etrafa dağılan küçük turuncu parçacıklar
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material orangeMat = new Material(shader) { color = new Color(1f, 0.45f, 0f) };

        for (int i = 0; i < 6; i++)
        {
            GameObject splash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            splash.name = "MandarinJuice";
            splash.transform.position = transform.position + Random.insideUnitSphere * 0.15f;
            splash.transform.localScale = Vector3.one * Random.Range(0.08f, 0.14f);

            Collider c = splash.GetComponent<Collider>();
            if (c != null) Destroy(c);

            splash.GetComponent<Renderer>().material = orangeMat;

            Rigidbody sRb = splash.AddComponent<Rigidbody>();
            sRb.mass = 0.05f;
            sRb.linearVelocity = (Random.onUnitSphere + Vector3.up * 0.8f) * Random.Range(2f, 4.5f);

            Destroy(splash, 0.6f);
        }
    }
}
