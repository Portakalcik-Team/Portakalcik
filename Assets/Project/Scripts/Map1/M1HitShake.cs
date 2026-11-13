using UnityEngine;

// KAMERAYA ekle (Main Camera, M1FirstPersonCamera ile aynı objede).
// Can azalınca kamera kısa süre titrer. Başka hiçbir şeyde titremez (W05: sarsıntı önemli bir anı anlatmalı).
[DefaultExecutionOrder(100)]   // M1FirstPersonCamera'dan SONRA çalışsın
public class M1HitShake : MonoBehaviour
{
    public float strength = 0.5f;    // titreşim şiddeti (slayt: küçük, 0.5)
    public float duration = 0.2f;    // saniye (slayt: kısa, 0.2)

    M1PlayerTrigger player;
    int lastHp = -1;
    float endTime;

    void LateUpdate()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<M1PlayerTrigger>();   // Player oyun başında kurulabilir
            if (player == null) return;
            lastHp = player.hp;
        }

        // Can düştüyse sarsıntıyı başlat
        if (player.hp < lastHp) endTime = Time.time + duration;
        lastHp = player.hp;

        if (Time.time >= endTime) return;

        // Süre bittikçe zayıflasın
        float k = (endTime - Time.time) / duration;
        float a = strength * 0.1f * k;   // 0.1: metre/derece cinsine küçült
        transform.position += Random.insideUnitSphere * a;
        transform.rotation *= Quaternion.Euler(Random.Range(-1f, 1f) * strength * 2f * k,
                                               Random.Range(-1f, 1f) * strength * 2f * k, 0f);
    }
}
