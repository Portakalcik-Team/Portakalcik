using UnityEngine;

public class Map2_Fruit : MonoBehaviour
{
    // lemon, tangerine or orange? a data file decides
    [SerializeField] private Map2_FruitData data;

    void Awake()
    {
        // an empty Data slot? say it in red
        if (data == null) Debug.LogError(name + ": no Fruit Data");
    }

    public Map2_FruitData Data()
    {
        return data;
    }
}
