using UnityEngine;
using UnityEngine.SceneManagement;

// Move (WASD), rotate (X, Shift = reverse), scale (Y, Shift = shrink), shoot (Space).
public class PlayerController : MonoBehaviour
{
    public float baseSpeed = 6f, rotSpeed = 120f, scaleSpeed = 1f;
    public float minScale = 0.5f, maxScale = 2.5f;
    public int maxHealth = 5;

    // values shown by the HUD
    public int Health { get; private set; }
    public bool Dead { get; private set; }
    public float NetRotation { get; private set; }   // signed degrees rotated with X
    public float Distance { get; private set; }      // units travelled
    public float CurrentScale => transform.localScale.x;
    public Vector3 StartPos { get; private set; }

    Rigidbody rb;
    float invUntil, fireCd;
    Vector3 lastPos;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Health = maxHealth;
        StartPos = transform.position;
        lastPos = StartPos;
    }

    void Update()
    {
        if (Dead)
        {
            if (Inp.Restart) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }
        float dt = Time.deltaTime;
        float dir = Inp.Shift ? -1f : 1f;

        if (Inp.X)
        {
            float a = dir * rotSpeed * dt;
            transform.Rotate(0f, a, 0f, Space.World);
            NetRotation += a;
        }
        if (Inp.Y)
        {
            float s = Mathf.Clamp(CurrentScale + dir * scaleSpeed * dt, minScale, maxScale);
            transform.localScale = Vector3.one * s;
        }

        fireCd -= dt;
        if (Inp.Fire && fireCd <= 0f) { Shoot(); fireCd = 0.3f; }
    }

    void FixedUpdate()
    {
        if (Dead) { rb.linearVelocity = Vector3.zero; return; }
        Vector2 m = Inp.Move;
        // bigger = slower, smaller = faster
        float speed = baseSpeed / Mathf.Sqrt(CurrentScale);
        Vector3 v = new Vector3(m.x, 0f, m.y);
        if (v.sqrMagnitude > 1f) v.Normalize();
        rb.linearVelocity = v * speed;
        Distance += (transform.position - lastPos).magnitude;
        lastPos = transform.position;
    }

    void Shoot()
    {
        float s = CurrentScale;
        Vector3 pos = transform.position + transform.forward * 0.9f * s + Vector3.up * 1.3f * s;
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "Projectile";
        go.transform.position = pos;
        go.transform.localScale = Vector3.one * 0.3f * s;   // projectile size follows player scale
        go.GetComponent<Renderer>().material = Mats.Make(new Color(0.2f, 1f, 0.9f));
        var col = go.GetComponent<SphereCollider>();
        col.isTrigger = true;
        var prb = go.AddComponent<Rigidbody>();
        prb.useGravity = false;
        prb.linearVelocity = transform.forward * 16f;
        var p = go.AddComponent<Projectile>();
        p.damage = Mathf.CeilToInt(s);                       // bigger player = stronger shots
    }

    public void Damage(int amount)
    {
        if (Dead || Time.time < invUntil) return;
        invUntil = Time.time + 1f;
        Health -= amount;
        if (Health <= 0) { Health = 0; Dead = true; }
    }

    public bool Invulnerable => Time.time < invUntil;
}
