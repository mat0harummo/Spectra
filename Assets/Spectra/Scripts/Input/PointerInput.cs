using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Мышь и клавиатура для проверки игры в редакторе без шлема.
// Левая кнопка: поставить слизь. Правая кнопка (зажать и водить): стирать. Пробел: «готово».
// В бою левая кнопка (зажать): стрелять. В магазине: стрелки и Enter.
public static class PointerInput
{
#if ENABLE_INPUT_SYSTEM
    public static bool LeftClicked => Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
    public static bool LeftHeld => Mouse.current != null && Mouse.current.leftButton.isPressed;
    public static bool RightHeld => Mouse.current != null && Mouse.current.rightButton.isPressed;
    public static Vector2 Position => Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
    public static bool NextPressed => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
    public static bool UpPressed => Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame;
    public static bool DownPressed => Keyboard.current != null && Keyboard.current.downArrowKey.wasPressedThisFrame;
    public static bool EnterPressed => Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame;
#else
    public static bool LeftClicked => Input.GetMouseButtonDown(0);
    public static bool LeftHeld => Input.GetMouseButton(0);
    public static bool RightHeld => Input.GetMouseButton(1);
    public static Vector2 Position => Input.mousePosition;
    public static bool NextPressed => Input.GetKeyDown(KeyCode.Space);
    public static bool UpPressed => Input.GetKeyDown(KeyCode.UpArrow);
    public static bool DownPressed => Input.GetKeyDown(KeyCode.DownArrow);
    public static bool EnterPressed => Input.GetKeyDown(KeyCode.Return);
#endif
}
