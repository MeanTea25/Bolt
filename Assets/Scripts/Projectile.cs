using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 1;
    void Start() { Destroy(gameObject, 3f); }

    void OnTriggerEnter(Collider other)
    {
        var h = other.GetComponentInParent<Hazard>();
        if (h != null) { h.Hit(damage); Destroy(gameObject); return; }
        if (other.isTrigger || other.GetComponentInParent<PlayerController>() != null) return;
        Destroy(gameObject);   // hit a wall or pillar
    }
}
