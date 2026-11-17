using System.Collections.Generic;
using UnityEngine;

// Пятно слизи на реальной поверхности: несколько приплюснутых зелёных капель.
// Его стирают трением (SlimeWiper): пятно уменьшается, а когда исчезает, даёт монетки.
public class SlimeSpot : MonoBehaviour
{
    public static readonly List<SlimeSpot> All = new List<SlimeSpot>();

    // Места, которые уже убрали. Сюда прилетают призраки, чтобы снова напачкать.
    public struct Place { public Vector3 Position, Normal; public float Size; }
    public static readonly List<Place> Cleaned = new List<Place>();

    private static readonly Color SlimeColor = new Color(0.35f, 0.9f, 0.2f);
    private const int Reward = 10;

    public float Size { get; private set; }
    private Place place;
    private float wiped; // 0 = грязно, 1 = чисто

    public static SlimeSpot Create(Vector3 position, Vector3 normal, float size)
    {
        var spot = new GameObject("Slime").AddComponent<SlimeSpot>();
        spot.Size = size;
        spot.place = new Place { Position = position, Normal = normal, Size = size };
        spot.transform.position = position + normal * 0.005f;
        spot.transform.up = normal;

        // 4–6 капель разного размера, чтобы пятно выглядело неровным.
        int drops = Random.Range(4, 7);
        for (int i = 0; i < drops; i++)
        {
            GameObject drop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(drop.GetComponent<Collider>());
            drop.transform.SetParent(spot.transform, false);

            Vector2 offset = Random.insideUnitCircle * size * 0.35f;
            float scale = size * Random.Range(0.4f, 0.8f);
            drop.transform.localPosition = new Vector3(offset.x, 0f, offset.y);
            drop.transform.localScale = new Vector3(scale, 0.01f, scale);
            drop.GetComponent<Renderer>().material.color = SlimeColor;
        }
        Sfx.Play(Sfx.Splat, position);
        return spot;
    }

    // amount: какую долю пятна стёрли за этот кадр (от 0 до 1).
    public void Wipe(float amount)
    {
        wiped += amount;
        transform.localScale = Vector3.one * Mathf.Max(0.1f, 1f - wiped);

        if (wiped >= 1f)
        {
            Wallet.Add(Reward);
            Cleaned.Add(place);
            Sfx.Play(Sfx.Coin, transform.position);
            Burst.Spawn(transform.position, SlimeColor);
            Destroy(gameObject);
        }
    }

    private void OnEnable() => All.Add(this);
    private void OnDisable() => All.Remove(this);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCleaned() => Cleaned.Clear();
}
