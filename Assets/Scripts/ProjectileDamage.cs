using UnityEngine;
using System.Collections;

public class ProjectileDamage : MonoBehaviour {

    public float damage = 0; // Set on instatiation, it will be passed onto any damagable object this project triggers with
    public string ignoreTag; // A tag that is ignored, when triggering with it, used to pass in whatever tag the instantiating object has

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Boundary" || other.tag == ignoreTag)
        {
            return;
        }
        Damagable damagable = other.GetComponent<Damagable>();
        if (damagable != null)
        {
            damagable.Damage(damage);
        }
        Destroy(gameObject);
    }
}
