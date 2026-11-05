using UnityEngine;

// A bouncing hazard ball. Hurts the player on contact, destroyed by projectiles.
public class Hazard : MonoBehaviour
{
    public float speed = 4f;
    public int hp = 3;
    Rigidbody rb;
    Renderer rend;
    float flashUntil;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
    }

    public void Launch(Vector3 dir, float spd)
    {
        speed = spd;
        dir.y = 0f;
        rb.linearVelocity = dir.normalized * speed;
    }

    void FixedUpdate()
    {
        Vector3 v = rb.linearVelocity; v.y = 0f;
        if (v.sqrMagnitude < 0.01f) v = new Vector3(Random.value - .5f, 0, Random.value - .5f);
        rb.linearVelocity = v.normalized * speed;   // keep constant speed after bounces
    }

    void Update()
    {
        rend.material.color = Time.time < flashUntil ? Color.white : new Color(0.9f, 0.15f, 0.2f);
    }

    public void Hit(int dmg)
    {
        hp -= dmg;
        flashUntil = Time.time + 0.1f;
        if (hp <= 0) { GameManager.Score += 10; Destroy(gameObject); }
    }

    void OnCollisionEnter(Collision c)
    {
        var p = c.collider.GetComponentInParent<PlayerController>();
        if (p != null) p.Damage(1);
    }

    void OnCollisionStay(Collision c) { OnCollisionEnter(c); }
}
