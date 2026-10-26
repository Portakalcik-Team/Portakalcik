using UnityEngine;

public class Map8_GameManager : MonoBehaviour
{
    public static Map8_GameManager Instance { get; private set; }

    [Header("Sayaç Ayarları")]
    public int mevcutPuan = 0;
    public int cikisIcinGerekliPuan = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PuanEkle(int miktar)
    {
        mevcutPuan += miktar;
        Debug.Log("Limon Toplandı! Güncel Limon: " + mevcutPuan);
    }

    public void PuanEksilt(int miktar)
    {
        mevcutPuan -= miktar;
        if (mevcutPuan < 0) mevcutPuan = 0;

        Debug.Log("Düşmana Çarpıldı! Güncel Limon: " + mevcutPuan);
    }
    public bool CikisAcikMi()
    {
        return mevcutPuan >= cikisIcinGerekliPuan;
    }
}
