using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Мышь для проверки игры в редакторе без шлема.
// Левая кнопка: поставить слизь. Правая кнопка (зажать и водить): стирать.
public static class PointerInput
{
#if ENABLE_INPUT_SYSTEM
    public static bool LeftClicked => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    public static bool RightHeld => Mouse.current != null && Mouse.current.rightButton.isPressed;
    public static Vector2 Position => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
    public static bool LeftClicked => Input.GetMouseButtonDown(0);
    public static bool RightHeld => Input.GetMouseButton(1);
    public static Vector2 Position => Input.mousePosition;
#endif
}
