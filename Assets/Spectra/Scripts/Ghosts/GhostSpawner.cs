using UnityEngine;

// Во время боя время от времени выпускает призраков вокруг игрока.
public class GhostSpawner : MonoBehaviour
{
    [SerializeField] private float interval = 6f;      // секунд между призраками
    [SerializeField] private int maxGhosts = 3;        // сколько их может быть сразу
    [SerializeField] private float spawnDistance = 3f; // как далеко от игрока появляются, м

    private Transform head;
    private float timer;

    private void Start()
    {
        head = FindAnyObjectByType<OVRCameraRig>().centerEyeAnchor;
    }

    private void Update()
    {
        if (GameManager.Instance.Phase != GamePhase.Battle)
            return;

        timer -= Time.deltaTime;
        if (timer > 0f || Ghost.All.Count >= maxGhosts)
            return;

        timer = interval;
        Vector2 dir = Random.insideUnitCircle.normalized * spawnDistance;
        Ghost.Create(head.position + new Vector3(dir.x, 0.3f, dir.y), head);
        Debug.Log("[Spectra] Появился призрак!");
    }
}
