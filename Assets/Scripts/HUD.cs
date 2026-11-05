using UnityEngine;

// Simple IMGUI HUD: shows how much the character was moved, rotated and scaled.
public class HUD : MonoBehaviour
{
    public PlayerController player;
    GUIStyle label, title, big;

    void Init()
    {
        label = new GUIStyle(GUI.skin.label) { fontSize = 18 };
        label.normal.textColor = Color.white;
        title = new GUIStyle(label) { fontSize = 20, fontStyle = FontStyle.Bold };
        title.normal.textColor = new Color(0.2f, 1f, 0.9f);
        big = new GUIStyle(label) { fontSize = 48, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
    }

    static void Rect(Rect r, Color c)
    {
        var old = GUI.color; GUI.color = c;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = old;
    }

    void OnGUI()
    {
        if (player == null) return;
        if (label == null) Init();

        Rect(new Rect(10, 10, 360, 290), new Color(0, 0, 0, 0.65f));
        float y = 16;
        GUI.Label(new Rect(20, y, 340, 28), "TRANSFORM CHANGES", title); y += 34;

        Vector3 d = player.transform.position - player.StartPos;
        GUI.Label(new Rect(20, y, 340, 26), $"Position  Δx {d.x:+0.0;-0.0;0.0}  Δz {d.z:+0.0;-0.0;0.0}", label); y += 28;
        GUI.Label(new Rect(20, y, 340, 26), $"Distance travelled: {player.Distance:0.0} m", label); y += 28;

        float rot = player.NetRotation;
        GUI.Label(new Rect(20, y, 340, 26), $"Rotation  {rot:+0;-0;0}°  (facing {player.transform.eulerAngles.y:0}°)", label); y += 28;

        float s = player.CurrentScale;
        GUI.Label(new Rect(20, y, 340, 26), $"Scale  x{s:0.00}  ({(s - 1f) * 100f:+0;-0;0}%)", label); y += 30;
        float t = Mathf.InverseLerp(player.minScale, player.maxScale, s);
        Rect(new Rect(20, y, 320, 14), new Color(1, 1, 1, 0.2f));
        Rect(new Rect(20, y, 320 * t, 14), new Color(1f, 0.55f, 0.1f));
        y += 26;

        GUI.Label(new Rect(20, y, 340, 26), "Health", label);
        for (int i = 0; i < player.maxHealth; i++)
            Rect(new Rect(100 + i * 32, y + 3, 26, 20), i < player.Health ? new Color(0.9f, 0.2f, 0.25f) : new Color(1, 1, 1, 0.15f));
        y += 32;
        GUI.Label(new Rect(20, y, 340, 26), $"Score: {GameManager.Score}", label);

        string help = "WASD move   X rotate   Y scale   Shift = reverse (rotate/shrink)   Space shoot";
        Rect(new Rect(10, Screen.height - 44, 780, 34), new Color(0, 0, 0, 0.65f));
        GUI.Label(new Rect(20, Screen.height - 42, 770, 30), help, label);

        if (player.Dead)
        {
            Rect(new Rect(0, 0, Screen.width, Screen.height), new Color(0, 0, 0, 0.6f));
            GUI.Label(new Rect(0, Screen.height / 2 - 60, Screen.width, 80), "GAME OVER", big);
            GUI.Label(new Rect(0, Screen.height / 2 + 10, Screen.width, 40), $"Score {GameManager.Score}  -  press R to restart",
                new GUIStyle(label) { alignment = TextAnchor.MiddleCenter });
        }
    }
}
