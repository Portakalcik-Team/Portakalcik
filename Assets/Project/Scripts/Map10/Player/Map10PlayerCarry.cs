using System.Collections.Generic;
using UnityEngine;

public class Map10PlayerCarry : MonoBehaviour
{
    [Header("Mandalina Kemeri Ayarları")]
    [SerializeField] private float beltRadius = 0.42f;       // Belin etrafındaki yarıçap
    [SerializeField] private float beltHeight = -0.1f;       // Karakterin merkezine göre bel yüksekliği
    [SerializeField] private float orbScale = 0.16f;         // Beldeki mandalina bilyelerinin boyutu
    [SerializeField] private Color mandarinColor = new Color(1f, 0.5f, 0f); // Canlı turuncu renk
    [SerializeField] private int maxBeltCapacity = 16;       // Belde gösterilebilecek maksimum bilye

    [Header("Durum")]
    [SerializeField] private int mandarinCount = 0;
    public int MandarinCount => mandarinCount;

    // Belde oluşturulan mandalina objeleri
    private readonly List<GameObject> beltOrbs = new List<GameObject>();
    private Transform beltContainer;
    private Material mandarinMaterial;

    void Awake()
    {
        // Bel mandalinaları için container oluştur
        GameObject container = new GameObject("MandarinBelt");
        container.transform.SetParent(transform);
        container.transform.localPosition = new Vector3(0f, beltHeight, 0f);
        container.transform.localRotation = Quaternion.identity;
        beltContainer = container.transform;

        // Ortak mandalina materyali
        Shader standardShader = Shader.Find("Universal Render Pipeline/Lit");
        if (standardShader == null) standardShader = Shader.Find("Standard");

        if (standardShader != null)
        {
            mandarinMaterial = new Material(standardShader)
            {
                color = mandarinColor
            };
        }
    }

    /// <summary>
    /// Beldeki mandalinaların dairesel dizilimini günceller
    /// </summary>
    private void UpdateBeltVisuals()
    {
        int visualCount = Mathf.Min(mandarinCount, maxBeltCapacity);

        // Eksik objeleri oluştur
        while (beltOrbs.Count < visualCount)
        {
            GameObject orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            orb.name = $"BeltMandarin_{beltOrbs.Count}";
            orb.transform.SetParent(beltContainer);
            orb.transform.localScale = Vector3.one * orbScale;

            // Collider'ını kaldır ki oyuncuya çarpmasın
            Collider col = orb.GetComponent<Collider>();
            if (col != null) Destroy(col);

            // Materyal ata
            if (mandarinMaterial != null)
            {
                Renderer rend = orb.GetComponent<Renderer>();
                if (rend != null) rend.material = mandarinMaterial;
            }

            beltOrbs.Add(orb);
        }

        // Fazla objeleri sil
        while (beltOrbs.Count > visualCount)
        {
            GameObject orb = beltOrbs[beltOrbs.Count - 1];
            beltOrbs.RemoveAt(beltOrbs.Count - 1);
            if (orb != null) Destroy(orb);
        }

        // Dairesel olarak belin etrafına yerleştir
        if (visualCount > 0)
        {
            float angleStep = 360f / visualCount;
            for (int i = 0; i < visualCount; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Sin(angle) * beltRadius;
                float z = Mathf.Cos(angle) * beltRadius;

                beltOrbs[i].transform.localPosition = new Vector3(x, 0f, z);
            }
        }
    }

    void Update()
    {
        // Beldeki mandalinalar hafif canlılık katması için yavaşça dönebilir
        if (beltContainer != null && mandarinCount > 0)
        {
            beltContainer.Rotate(Vector3.up, 25f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Ağaçtan veya yerden mandalina toplandığında çağrılır
    /// </summary>
    public void AddMandarin(int amount = 1)
    {
        if (amount <= 0) return;
        mandarinCount += amount;
        UpdateBeltVisuals();
        Debug.Log($"[Map10] +{amount} Mandalina toplandı! Beldeki Toplam: {mandarinCount}");
    }

    /// <summary>
    /// İleride silah veya mermi sistemi için mandalina harcar
    /// </summary>
    public bool UseMandarin(int amount = 1)
    {
        if (mandarinCount < amount) return false;

        mandarinCount -= amount;
        UpdateBeltVisuals();
        return true;
    }

    /// <summary>
    /// PlayerDrop.cs tarafından çağrılan bırakma fonksiyonu
    /// </summary>
    public void DropCargo()
    {
        // Yakında toplanacak bir ağaç varsa drop yapmasın
        if (Map10Tree.IsPlayerNearAnyTree(transform.position, 3.2f))
        {
            return;
        }

        if (mandarinCount <= 0) return;

        mandarinCount--;
        UpdateBeltVisuals();
        Debug.Log($"[Map10] 1 Mandalina yere bırakıldı. Kalan: {mandarinCount}");

        GameObject dropped = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        dropped.name = "DroppedMandarin";
        dropped.transform.position = transform.position + transform.forward * 0.7f + Vector3.up * 0.2f;
        dropped.transform.localScale = Vector3.one * 0.25f;

        if (mandarinMaterial != null)
        {
            Renderer rend = dropped.GetComponent<Renderer>();
            if (rend != null) rend.material = mandarinMaterial;
        }

        Rigidbody rb = dropped.AddComponent<Rigidbody>();
        rb.mass = 0.2f;
        rb.AddForce(transform.forward * 2f + Vector3.up * 2f, ForceMode.Impulse);
    }
}
