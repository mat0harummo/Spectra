using UnityEngine;

// Пятно слизи на реальной поверхности: несколько приплюснутых зелёных капель.
// На этапе 3 его можно будет стереть контроллером.
public class SlimeSpot : MonoBehaviour
{
    private static readonly Color SlimeColor = new Color(0.35f, 0.9f, 0.2f);

    public static SlimeSpot Create(Vector3 position, Vector3 normal, float size)
    {
        var spot = new GameObject("Slime").AddComponent<SlimeSpot>();
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
        return spot;
    }
}
