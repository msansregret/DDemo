using UnityEngine;
using System.Collections;

public class Damagable : MonoBehaviour {
    public float health = 100;

    public void Damage(float damage)
    {
        health = health - damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
