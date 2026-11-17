using UnityEngine;

// Звуки игры. Они создаются прямо в коде, поэтому аудиофайлы не нужны.
public static class Sfx
{
    public static AudioClip Splat, Coin, Laser, GhostHit, GhostPop, Hurt;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        Splat = Tone("Splat", 200f, 60f, 0.2f, 0.5f);      // шлепок слизи
        Coin = Tone("Coin", 900f, 1400f, 0.15f, 0f);       // монетка
        Laser = Tone("Laser", 1800f, 400f, 0.12f, 0.1f);   // выстрел
        GhostHit = Tone("GhostHit", 500f, 250f, 0.1f, 0.3f);
        GhostPop = Tone("GhostPop", 300f, 1200f, 0.3f, 0.1f);
        Hurt = Tone("Hurt", 150f, 80f, 0.3f, 0.4f);        // призрак ударил игрока
    }

    // Звук раздаётся из точки в комнате (в шлеме слышно, откуда он).
    public static void Play(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }

    // Тон, который плавно меняет высоту от from до to (Гц) и затихает.
    // noise — доля шума от 0 до 1: чем больше, тем «грязнее» звук.
    private static AudioClip Tone(string name, float from, float to, float duration, float noise)
    {
        const int rate = 44100;
        int count = (int)(rate * duration);
        float[] data = new float[count];
        float phase = 0f;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / count;
            phase += 2f * Mathf.PI * Mathf.Lerp(from, to, t) / rate;
            float wave = Mathf.Sin(phase) * (1f - noise) + (Random.value * 2f - 1f) * noise;
            data[i] = wave * (1f - t) * 0.5f;
        }

        AudioClip clip = AudioClip.Create(name, count, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
