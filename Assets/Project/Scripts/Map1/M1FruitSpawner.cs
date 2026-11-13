using UnityEngine;

// BOŞ bir objeye ekle (ped). Type slotuna M1FruitType dosyasını sürükle.
// Pedin üstündeki meyve alınınca Delay sn sonra yenisi çıkar.
public class M1FruitSpawner : MonoBehaviour
{
    public M1FruitType type;
    public float delay = 4f;
    public bool dropFromHeight = true;   // pedi ağaç tepesine koy, meyve yere düşsün
    public GameObject prefab;        // boş bırakabilirsin, o zaman küre yapılır

    float timer;

    void Update()
    {
        if (type == null) return;
        if (GetComponentInChildren<M1FruitPickup>() != null) return;   // meyve hâlâ bekliyor

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Spawn();
        timer = delay;
    }

    void Spawn()
    {
        GameObject go;
        if (prefab != null)
        {
            go = Instantiate(prefab, transform.position, transform.rotation, transform);
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.SetParent(transform, false);
            go.transform.position = transform.position;
            go.GetComponent<Renderer>().material.color = type.color;
            var old = go.GetComponent<Collider>();
            if (old != null) Destroy(old);
        }

        go.name = "Pickup_" + type.displayName;
        go.transform.localScale = Vector3.one * type.size;

        var col = go.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 0.9f;   // topun biraz dışından da alınsın

        // Tetikleyici hareket edebilsin diye kinematik Rigidbody (yer çekimini script yapar)
        var rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        var pick = go.AddComponent<M1FruitPickup>();
        pick.type = type;
        pick.fall = dropFromHeight;
    }
}
