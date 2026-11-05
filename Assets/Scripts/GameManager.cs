using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static int Score;
    public int maxHazards = 6;
    public float arenaHalf = 15f;

    PlayerController player;
    float spawnTimer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindFirstObjectByType<GameManager>() != null) return;
        new GameObject("GameManager").AddComponent<GameManager>();
    }

    void Awake()
    {
        Score = 0;
        Random.InitState(7);
        BuildLight();
        BuildArena();
        SpawnPlayer();
        SetupCamera();
        gameObject.AddComponent<HUD>().player = player;
    }

    void BuildLight()
    {
        if (FindFirstObjectByType<Light>() != null) return;
        var l = new GameObject("Sun").AddComponent<Light>();
        l.type = LightType.Directional;
        l.transform.rotation = Quaternion.Euler(50, -30, 0);
    }

    GameObject Block(string name, Vector3 pos, Vector3 size, Color c, PrimitiveType t = PrimitiveType.Cube)
    {
        var g = GameObject.CreatePrimitive(t);
        g.name = name;
        g.transform.position = pos;
        g.transform.localScale = size;
        g.GetComponent<Renderer>().material = Mats.Make(c);
        return g;
    }

    void BuildArena()
    {
        float h = arenaHalf;
        Block("Floor", new Vector3(0, -0.1f, 0), new Vector3(h * 2 + 2, 0.2f, h * 2 + 2), new Color(0.22f, 0.25f, 0.3f));
        Color wall = new Color(0.35f, 0.38f, 0.45f);
        Block("WallN", new Vector3(0, 1.5f, h + 0.5f), new Vector3(h * 2 + 2, 3, 1), wall);
        Block("WallS", new Vector3(0, 1.5f, -h - 0.5f), new Vector3(h * 2 + 2, 3, 1), wall);
        Block("WallE", new Vector3(h + 0.5f, 1.5f, 0), new Vector3(1, 3, h * 2 + 2), wall);
        Block("WallW", new Vector3(-h - 0.5f, 1.5f, 0), new Vector3(1, 3, h * 2 + 2), wall);

        // static obstacles
        for (int i = 0; i < 9; i++)
        {
            Vector3 p;
            do { p = new Vector3(Random.Range(-h + 2, h - 2), 0, Random.Range(-h + 2, h - 2)); }
            while (p.magnitude < 5f);
            bool tall = i % 2 == 0;
            Block("Pillar" + i, p + Vector3.up * 1.5f, tall ? new Vector3(1.6f, 3f, 1.6f) : new Vector3(3f, 1.5f, 1f) ,
                  new Color(0.95f, 0.55f, 0.15f)).transform.position = p + Vector3.up * (tall ? 1.5f : 0.75f);
        }
    }

    void SpawnPlayer()
    {
        var prefab = Resources.Load<GameObject>("Bolt");
        if (prefab == null) { Debug.LogError("Assets/Resources/Bolt.obj not found!"); return; }

        var root = new GameObject("Player");
        root.transform.position = Vector3.zero;
        var model = Instantiate(prefab, root.transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        var rend = model.GetComponentInChildren<Renderer>();
        Color[] cols = { new Color(0.2f, 0.45f, 0.85f), new Color(1f, 0.55f, 0.1f), new Color(0.2f, 1f, 0.9f), new Color(0.15f, 0.16f, 0.2f) };
        var ms = new Material[rend.sharedMaterials.Length];
        for (int i = 0; i < ms.Length; i++) ms[i] = Mats.Make(cols[i % cols.Length]);
        rend.sharedMaterials = ms;

        var b = model.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
        var box = root.AddComponent<BoxCollider>();
        box.center = b.center;
        box.size = b.size;

        var rb = root.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        player = root.AddComponent<PlayerController>();
    }

    void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null) { cam = new GameObject("Main Camera").AddComponent<Camera>(); cam.tag = "MainCamera"; }
        cam.backgroundColor = new Color(0.08f, 0.09f, 0.12f);
        cam.clearFlags = CameraClearFlags.SolidColor;
        var f = cam.gameObject.AddComponent<CameraFollow>();
        f.target = player.transform;
    }

    void Update()
    {
        if (player == null || player.Dead) return;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f && FindObjectsByType<Hazard>(FindObjectsSortMode.None).Length < maxHazards)
        {
            SpawnHazard();
            spawnTimer = 2f;
        }
    }

    void SpawnHazard()
    {
        float h = arenaHalf - 1.5f;
        Vector3 pos;
        do { pos = new Vector3(Random.Range(-h, h), 0.6f, Random.Range(-h, h)); }
        while ((pos - player.transform.position).magnitude < 8f);

        var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        g.name = "Hazard";
        g.transform.position = pos;
        g.transform.localScale = Vector3.one * 1.2f;
        g.GetComponent<Renderer>().material = Mats.Make(new Color(0.9f, 0.15f, 0.2f));
        var mat = new PhysicsMaterial { bounciness = 1f, dynamicFriction = 0f, staticFriction = 0f,
            frictionCombine = PhysicsMaterialCombine.Minimum, bounceCombine = PhysicsMaterialCombine.Maximum };
        g.GetComponent<SphereCollider>().material = mat;
        var rb = g.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        var hz = g.AddComponent<Hazard>();
        Vector3 dir = player.transform.position - pos + new Vector3(Random.Range(-6f, 6f), 0, Random.Range(-6f, 6f));
        hz.Launch(dir, Random.Range(3f, 5.5f));
    }
}
