using UnityEngine;

// Здоровье игрока. Призраки отнимают его, когда подлетают вплотную.
public static class PlayerHealth
{
    public const int Max = 100;
    public static int Health { get; private set; }

    public static void Damage(int amount)
    {
        Health = Mathf.Max(0, Health - amount);
        Debug.Log($"[Spectra] Призрак ударил! Здоровье: {Health}");
    }

    public static void Heal(int amount)
    {
        Health = Mathf.Min(Max, Health + amount);
        Debug.Log($"[Spectra] Лечение! Здоровье: {Health}");
    }

    // Полное здоровье при каждом запуске Play.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset() => Health = Max;
}
