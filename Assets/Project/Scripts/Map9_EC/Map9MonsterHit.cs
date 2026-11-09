using UnityEngine;

// Seviye 9: canavara değen oyuncu can kaybeder, portakalı düşer
public class Map9MonsterHit : MonoBehaviour
{
    [SerializeField] private int damage = 12;   // Sv.9: denge dokümanı

    void OnTriggerEnter(Collider other)
    {
        // Who? sadece canı olan: oyuncu
        Map9Health health = other.GetComponent<Map9Health>();
        if (health == null) return;

        // Act 1: elinde portakal varsa düşür
        Map9Carry carry = other.GetComponent<Map9Carry>();
        if (carry != null) carry.DropPortakal();

        // Act 2: canını düşür
        Debug.Log(name + " vurdu: -" + damage);
        health.TakeDamage(damage);
    }
}