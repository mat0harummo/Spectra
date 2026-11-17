using Meta.XR.MRUtilityKit;
using UnityEngine;

// Отладка: ставит маленький шарик в центр каждой найденной поверхности.
// Так в шлеме видно, что игра правильно поняла комнату.
public class SurfaceMarkers : MonoBehaviour
{
    [SerializeField] private float markerSize = 0.1f;

    private void Start()
    {
        if (RoomScanner.Instance.IsReady)
            PlaceMarkers();
        else
            RoomScanner.Instance.OnRoomReady += PlaceMarkers;
    }

    private void OnDestroy()
    {
        if (RoomScanner.Instance != null)
            RoomScanner.Instance.OnRoomReady -= PlaceMarkers;
    }

    private void PlaceMarkers()
    {
        foreach (MRUKAnchor surface in RoomScanner.Instance.Surfaces)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Destroy(marker.GetComponent<Collider>());
            marker.name = "Marker_" + surface.name;
            marker.transform.SetParent(transform);
            marker.transform.position = surface.transform.position;
            marker.transform.localScale = Vector3.one * markerSize;

            bool isFloor = (surface.Label & MRUKAnchor.SceneLabels.FLOOR) != 0;
            marker.GetComponent<Renderer>().material.color = isFloor ? Color.green : Color.cyan;
        }
    }
}
