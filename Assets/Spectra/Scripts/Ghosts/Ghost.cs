using System.Collections.Generic;
using UnityEngine;

// Призрак: летит к убранному месту и снова пачкает его,
// потом подлетает к игроку и бьёт, отлетает и всё по новой.
public class Ghost : MonoBehaviour
{
    public static readonly List<Ghost> All = new List<Ghost>();
    public static int Defeated { get; private set; } // сколько призраков побеждено за игру

    private enum Mode { Dirty, Attack, Retreat }

    [SerializeField] private float speed = 0.6f;  // м/с
    [SerializeField] private int damage = 10;
    [SerializeField] private int health = 3;   // сколько попаданий выдерживает
    [SerializeField] private int reward = 5;   // монетки за победу над призраком

    private Transform head;
    private Transform body;
    private Mode mode;
    private Vector3 target;
    private SlimeSpot.Place place;

    public static Ghost Create(Vector3 position, Transform head)
    {
        var ghost = new GameObject("Ghost").AddComponent<Ghost>();
        ghost.transform.position = position;
        ghost.head = head;

        // Белое тело и два чёрных глаза, смотрящих на игрока (вперёд по оси Z).
        ghost.body = Part(ghost.transform, Vector3.zero, new Vector3(0.3f, 0.35f, 0.3f), new Color(0.9f, 0.95f, 1f));
        Part(ghost.body, new Vector3(-0.17f, 0.12f, 0.42f), Vector3.one * 0.18f, Color.black);
        Part(ghost.body, new Vector3(0.17f, 0.12f, 0.42f), Vector3.one * 0.18f, Color.black);
        return ghost;
    }

    private static Transform Part(Transform parent, Vector3 position, Vector3 scale, Color color)
    {
        GameObject part = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Destroy(part.GetComponent<Collider>());
        part.transform.SetParent(parent, false);
        part.transform.localPosition = position;
        part.transform.localScale = scale;
        part.GetComponent<Renderer>().material.color = color;
        return part.transform;
    }

    private void Start() => PickTarget();

    private void Update()
    {
        // Бой закончился — призрак исчезает.
        if (GameManager.Instance.Phase != GamePhase.Battle)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 goal = mode == Mode.Attack ? head.position : target;
        transform.position = Vector3.MoveTowards(transform.position, goal, speed * Time.deltaTime);
        transform.LookAt(head);
        body.localPosition = Vector3.up * Mathf.Sin(Time.time * 3f) * 0.03f; // покачивается в воздухе

        if (Vector3.Distance(transform.position, goal) < 0.3f)
            Arrive();
    }

    private void PickTarget()
    {
        if (SlimeSpot.Cleaned.Count > 0)
        {
            // Забираем место из списка, чтобы два призрака не летели в одно.
            int i = Random.Range(0, SlimeSpot.Cleaned.Count);
            place = SlimeSpot.Cleaned[i];
            SlimeSpot.Cleaned.RemoveAt(i);
            target = place.Position + place.Normal * 0.4f;
            mode = Mode.Dirty;
        }
        else
        {
            mode = Mode.Attack;
        }
    }

    private void Arrive()
    {
        switch (mode)
        {
            case Mode.Dirty:
                SlimeSpot.Create(place.Position, place.Normal, place.Size);
                mode = Mode.Attack;
                break;

            case Mode.Attack:
                PlayerHealth.Damage(damage);
                Sfx.Play(Sfx.Hurt, head.position);
                Vector2 away = Random.insideUnitCircle.normalized * 2f;
                target = head.position + new Vector3(away.x, 0.3f, away.y);
                mode = Mode.Retreat;
                break;

            case Mode.Retreat:
                PickTarget();
                break;
        }
    }

    // Попадание из оружия: призрак уменьшается, а когда здоровье кончится, исчезает.
    public void Hit(int amount)
    {
        health -= amount;
        body.localScale *= 0.85f;
        if (health > 0)
        {
            Sfx.Play(Sfx.GhostHit, transform.position);
            return;
        }

        // Не успел напачкать — возвращаем место в список, туда прилетит другой.
        if (mode == Mode.Dirty)
            SlimeSpot.Cleaned.Add(place);

        Defeated++;
        Sfx.Play(Sfx.GhostPop, transform.position);
        Burst.Spawn(transform.position, Color.white, 15);
        Wallet.Add(reward);
        Debug.Log($"[Spectra] Призрак побеждён! Всего: {Defeated}");
        Destroy(gameObject);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetDefeated() => Defeated = 0;

    private void OnEnable() => All.Add(this);
    private void OnDisable() => All.Remove(this);
}
