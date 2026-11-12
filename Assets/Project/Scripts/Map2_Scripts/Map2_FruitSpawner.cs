using UnityEngine;

public class Map2_FruitSpawner : MonoBehaviour
{
    [SerializeField] private Map2_Fruit fruitPrefab;   // what grows on this tree
    [SerializeField] private Transform fruitPoint;     // where it hangs
    private float timer;                               // seconds left until the next fruit

    void Start()
    {
        // the tree starts with a fruit on it: the first wait starts after the first pick
        timer = fruitPrefab.Data().RegrowTime();
    }

    void Update()
    {
        // a regrow time of 0 means this tree gives its fruit only once
        if (fruitPrefab.Data().RegrowTime() <= 0f) return;

        // a fruit still hangs on this tree? then do nothing
        if (GetComponentInChildren<Map2_Fruit>() != null) return;

        // count down: a little less time left every frame
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // time is up: a new fruit at the fruit point, as its child
        Instantiate(fruitPrefab, fruitPoint);
        timer = fruitPrefab.Data().RegrowTime();   // the next wait will be this long
    }
}
