using UnityEngine;

namespace Portakalcik
{
    public enum TreeType
    {
        Mandalina,
        Portakal,
        Limon
    }

    public class FruitTree : MonoBehaviour
    {
        [Header("Ağaç Türü ve Verimi")]
        public TreeType treeType = TreeType.Mandalina;
        [Tooltip("Denge dokümanına göre Seviye 6 verimleri: Mandalina=11, Portakal=3, Limon=2")]
        public int fruitYield = 11;

        public bool isHarvested = false;
        public float interactDistance = 3.5f;

        public bool CanHarvest(Vector3 playerPos)
        {
            if (isHarvested) return false;
            return Vector3.Distance(transform.position, playerPos) <= interactDistance;
        }

        public int Harvest()
        {
            if (isHarvested) return 0;
            isHarvested = true;

            // Hasat edilince ağacın boyutunu biraz küçülterek görsel geribildirim ver
            transform.localScale = transform.localScale * 0.85f;

            Debug.Log($"[Portakalcik] {treeType} Ağacından {fruitYield} adet meyve toplandı!");
            return fruitYield;
        }
    }
}
