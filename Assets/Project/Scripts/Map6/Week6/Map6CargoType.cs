using UnityEngine;

// Kargo türlerinin puan ve ağırlık verilerini tutan ScriptableObject
[CreateAssetMenu(menuName = "Map6/Cargo Type", fileName = "Map6_CT_New")]
public class Map6CargoType : ScriptableObject
{
    [Header("Kargo Puanı Özelliği")]
    public int points = 1;

    [Header("Ağırlık (Zıplama Feragat) Özelliği")]
    [Tooltip("Oyuncunun temel kütlesine (Rigidbody mass) eklenir; ağır kargoda zıplama yüksekliği düşer.")]
    public float weight = 0f;
}
