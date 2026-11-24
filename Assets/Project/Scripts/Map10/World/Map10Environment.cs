using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Map10Environment : MonoBehaviour
{
    [Header("Atmosfer ve Sis (Fog) Ayarları")]
    [SerializeField] private bool enableMysticFog = true;
    [SerializeField] private Color mysticFogColor = new Color(0.16f, 0.12f, 0.22f, 1f); // Mistik gün batımı / son zindan sisi
    [SerializeField] private float fogDensity = 0.012f;

    [Header("Ortam Işığı")]
    [SerializeField] private Color ambientColor = new Color(0.35f, 0.32f, 0.42f, 1f);

    private ParticleSystem dustParticles;

    void OnEnable()
    {
        ApplyAtmosphere();
    }

    void Start()
    {
        ApplyAtmosphere();

        if (Application.isPlaying)
        {
            SetupAmbientParticles();
            DecorateDoors();
        }
    }

    private void ApplyAtmosphere()
    {
        if (enableMysticFog)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fogColor = mysticFogColor;
            RenderSettings.ambientSkyColor = ambientColor;
        }
    }

    /// <summary>
    /// Oyuncunun etrafında hafif süzülen büyülü sporlar / toz zerrecikleri
    /// </summary>
    private void SetupAmbientParticles()
    {
        GameObject dustObj = new GameObject("Map10AmbientDust");
        dustObj.transform.SetParent(transform);

        dustParticles = dustObj.AddComponent<ParticleSystem>();
        var main = dustParticles.main;
        main.loop = true;
        main.startLifetime = 6f;
        main.startSpeed = 0.25f;
        main.startSize = 0.08f;
        main.startColor = new Color(1f, 0.8f, 0.3f, 0.6f); // Hafif parlayan altın tozları
        main.maxParticles = 80;

        var emission = dustParticles.emission;
        emission.rateOverTime = 12f;

        var shape = dustParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(25f, 5f, 25f);

        var vel = dustParticles.velocityOverLifetime;
        vel.enabled = true;
        vel.y = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);

        ParticleSystemRenderer rend = dustObj.GetComponent<ParticleSystemRenderer>();
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        rend.material = new Material(shader) { color = new Color(1f, 0.85f, 0.4f, 0.7f) };
    }

    void Update()
    {
        if (Application.isPlaying && dustParticles != null)
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                dustParticles.transform.position = cam.transform.position + cam.transform.forward * 4f;
            }
        }
    }

    /// <summary>
    /// Haritadaki kapılara demir parmaklık / ahşap kiriş deseni ekler
    /// </summary>
    private void DecorateDoors()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        Material ironMat = new Material(shader) { color = new Color(0.2f, 0.2f, 0.22f) };
        Material bandMat = new Material(shader) { color = new Color(0.45f, 0.25f, 0.1f) };

        foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            if (obj != null && obj.name.ToLower().Contains("door"))
            {
                // Kapıya çalışma mantığını ekle (yaklaşınca / vurulunca açılma)
                if (obj.GetComponent<M10Door>() == null)
                {
                    obj.AddComponent<M10Door>();
                }

                if (obj.transform.Find("DoorDecor") == null)
                {
                    GameObject decor = new GameObject("DoorDecor");
                    decor.transform.SetParent(obj.transform);
                    decor.transform.localPosition = Vector3.zero;
                    decor.transform.localRotation = Quaternion.identity;

                    // Yatay metal takviye bantları
                    for (int i = -1; i <= 1; i++)
                    {
                        GameObject band = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        band.name = $"MetalBand_{i}";
                        band.transform.SetParent(decor.transform);
                        band.transform.localPosition = new Vector3(0f, i * 0.7f, 0f);
                        band.transform.localScale = new Vector3(1.02f, 0.15f, 1.05f);
                        Collider c = band.GetComponent<Collider>();
                        if (c != null)
                        {
                            if (Application.isPlaying) Destroy(c);
                            else DestroyImmediate(c);
                        }
                        band.GetComponent<Renderer>().material = ironMat;
                    }
                }
            }
        }
    }
}
