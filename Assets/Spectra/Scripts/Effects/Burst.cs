using UnityEngine;

// Маленький «взрыв» из капелек: когда стёрто пятно или побеждён призрак.
public class Burst : MonoBehaviour
{
    private const float Lifetime = 0.6f;
    private const float DropSize = 0.04f;

    private Vector3 velocity;
    private float life = Lifetime;

    public static void Spawn(Vector3 position, Color color, int count = 10)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject drop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(drop.GetComponent<Collider>());
            drop.transform.position = position;
            drop.transform.localScale = Vector3.one * DropSize;
            drop.GetComponent<Renderer>().material.color = color;
            drop.AddComponent<Burst>().velocity = Random.onUnitSphere * Random.Range(0.5f, 1.5f) + Vector3.up;
        }
    }

    private void Update()
    {
        velocity += Physics.gravity * 0.3f * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        life -= Time.deltaTime;
        transform.localScale = Vector3.one * DropSize * Mathf.Max(0f, life / Lifetime);
        if (life <= 0f)
            Destroy(gameObject);
    }
}
