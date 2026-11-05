using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Works with both the new Input System and the legacy Input Manager.
public static class Inp
{
#if ENABLE_INPUT_SYSTEM
    static bool Held(Key k) { return Keyboard.current != null && Keyboard.current[k].isPressed; }
    public static bool X => Held(Key.X);
    public static bool Y => Held(Key.Y);
    public static bool Shift => Held(Key.LeftShift) || Held(Key.RightShift);
    public static bool Fire => Held(Key.Space);
    public static bool Restart => Held(Key.R);
    public static Vector2 Move => new Vector2((Held(Key.D) ? 1 : 0) - (Held(Key.A) ? 1 : 0),
                                              (Held(Key.W) ? 1 : 0) - (Held(Key.S) ? 1 : 0));
#else
    public static bool X => Input.GetKey(KeyCode.X);
    public static bool Y => Input.GetKey(KeyCode.Y);
    public static bool Shift => Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    public static bool Fire => Input.GetKey(KeyCode.Space);
    public static bool Restart => Input.GetKey(KeyCode.R);
    public static Vector2 Move => new Vector2((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
                                              (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
#endif
}
