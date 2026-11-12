using UnityEngine;
using Unity.Cinemachine;

public class Map2_MonsterHit : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    private CinemachineImpulseSource shake;   // null = no shake

    void Start()
    {
        // the Impulse Source on this same monster, if it has one
        shake = GetComponent<CinemachineImpulseSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        // Who? only the player
        if (!other.CompareTag("Player")) return;
        // Act: report the hit - health comes in a later week
        Debug.Log(name + " hit you for " + damage + " damage!");
        // ...and give the camera a short bump
        if (shake != null) shake.GenerateImpulse();
    }
}
