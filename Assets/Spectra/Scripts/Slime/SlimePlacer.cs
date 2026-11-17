using Meta.XR.MRUtilityKit;
using UnityEngine;

// Разметка: игрок указывает на место, которое хочет убрать, и там появляется слизь.
// В шлеме: луч из правого контроллера + курок. В редакторе: клик мышью в окне Game.
public class SlimePlacer : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float slimeSize = 0.3f;

    private Transform controller;
    private Camera cam;

    private void Start()
    {
        OVRCameraRig rig = FindAnyObjectByType<OVRCameraRig>();
        controller = rig.rightControllerAnchor;
        cam = rig.centerEyeAnchor.GetComponent<Camera>();
    }

    private void Update()
    {
        if (!RoomScanner.Instance.IsReady)
            return;

        if (TryGetPointerRay(out Ray ray))
            PlaceSlime(ray);
    }

    private bool TryGetPointerRay(out Ray ray)
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            ray = new Ray(controller.position, controller.forward);
            return true;
        }

        if (Application.isEditor && PointerInput.LeftClicked)
        {
            ray = cam.ScreenPointToRay(PointerInput.Position);
            return true;
        }

        ray = default;
        return false;
    }

    private void PlaceSlime(Ray ray)
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room.Raycast(ray, maxDistance, out RaycastHit hit, out MRUKAnchor surface))
        {
            SlimeSpot.Create(hit.point, hit.normal, slimeSize);
            Debug.Log($"[Spectra] Слизь на поверхности: {surface.Label}");
        }
    }
}
