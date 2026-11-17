using UnityEngine;

// Лазерный пылесос: пока зажат курок, стреляет лучом по призракам.
// В шлеме: курок правого контроллера. В редакторе: зажать левую кнопку мыши (во время боя).
public class LaserVacuum : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldown = 0.3f;   // секунд между выстрелами
    [SerializeField] private float range = 10f;       // дальность, м
    [SerializeField] private float aimRadius = 0.25f; // насколько можно промахнуться и всё равно попасть, м

    private Transform controller;
    private Camera cam;
    private Transform beam;
    private float nextShot;
    private float beamTimer;

    private void Start()
    {
        OVRCameraRig rig = FindAnyObjectByType<OVRCameraRig>();
        controller = rig.rightControllerAnchor;
        cam = rig.centerEyeAnchor.GetComponent<Camera>();

        // Луч — тонкий голубой цилиндр, который показываем на мгновение при выстреле.
        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(cylinder.GetComponent<Collider>());
        cylinder.name = "LaserBeam";
        cylinder.GetComponent<Renderer>().material.color = new Color(0.3f, 0.9f, 1f);
        cylinder.SetActive(false);
        beam = cylinder.transform;
    }

    // Улучшения из магазина.
    public void AddDamage(int amount) => damage += amount;

    // Плазменная метла: стреляет вдвое чаще, по широкой полосе, оранжевым лучом.
    public void EquipBroom()
    {
        cooldown = 0.15f;
        aimRadius = 0.5f;
        beam.GetComponent<Renderer>().material.color = new Color(1f, 0.5f, 0.1f);
    }

    private void Update()
    {
        beamTimer -= Time.deltaTime;
        if (beamTimer <= 0f)
            beam.gameObject.SetActive(false);

        if (GameManager.Instance.Phase != GamePhase.Battle || Time.time < nextShot)
            return;
        if (!TryGetShot(out Ray ray, out Vector3 from))
            return;

        nextShot = Time.time + cooldown;
        Sfx.Play(Sfx.Laser, from, 0.4f);
        Ghost ghost = FindTarget(ray);
        ShowBeam(from, ghost != null ? ghost.transform.position : ray.GetPoint(range));
        if (ghost != null)
            ghost.Hit(damage);
    }

    private bool TryGetShot(out Ray ray, out Vector3 from)
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            ray = new Ray(controller.position, controller.forward);
            from = controller.position;
            return true;
        }

        if (Application.isEditor && PointerInput.LeftHeld)
        {
            ray = cam.ScreenPointToRay(PointerInput.Position);
            from = cam.transform.position - cam.transform.up * 0.2f + cam.transform.right * 0.15f; // «из руки»
            return true;
        }

        ray = default;
        from = default;
        return false;
    }

    // Ближайший призрак, который почти на линии луча.
    private Ghost FindTarget(Ray ray)
    {
        Ghost best = null;
        float bestDistance = range;
        foreach (Ghost ghost in Ghost.All)
        {
            float along = Vector3.Dot(ghost.transform.position - ray.origin, ray.direction);
            if (along < 0f || along > bestDistance)
                continue;
            if (Vector3.Distance(ray.GetPoint(along), ghost.transform.position) < aimRadius)
            {
                best = ghost;
                bestDistance = along;
            }
        }
        return best;
    }

    private void ShowBeam(Vector3 from, Vector3 to)
    {
        beam.position = (from + to) * 0.5f;
        beam.up = to - from;
        beam.localScale = new Vector3(0.01f, Vector3.Distance(from, to) * 0.5f, 0.01f);
        beam.gameObject.SetActive(true);
        beamTimer = 0.08f;
    }
}
