using UnityEngine;
using Unity.Cinemachine;

// Kısa kamera sarsıntısı (W05: Impulse). Sadece önemli anlarda kullan: düşman oyuncuya değince.
// Sarsıntıyı CM_Follow / CM_FirstPerson üzerindeki Cinemachine Impulse Listener dinler.
public static class Map7_CameraShake
{
    private static CinemachineImpulseSource source;

    // force: sarsıntı gücü (varsayılan küçük tutuldu; çok sarsıntı bazı oyuncuları rahatsız eder)
    public static void Shake(float force = 0.5f)
    {
        if (source == null)
        {
            GameObject go = new GameObject("Map7_ShakeSource");
            source = go.AddComponent<CinemachineImpulseSource>();
        }
        source.GenerateImpulse(Vector3.down * force);
    }
}
