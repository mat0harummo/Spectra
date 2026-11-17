using Meta.XR.MRUtilityKit;
using UnityEngine;

// Уборка: игрок трёт слизь, и она исчезает.
// В шлеме: зажать боковую кнопку (grip) правого контроллера и тереть им по пятну.
// В редакторе: зажать правую кнопку мыши и водить по пятну в окне Game.
public class SlimeWiper : MonoBehaviour
{
    [SerializeField] private float wipeRadius = 0.1f;   // насколько близко к пятну нужно тереть, м
    [SerializeField] private float rubDistance = 1.5f;  // сколько метров «натереть», чтобы стереть пятно

    private Transform controller;
    private Camera cam;
    private Vector3? lastPoint;

    private void Start()
    {
        OVRCameraRig rig = FindAnyObjectByType<OVRCameraRig>();
        controller = rig.rightControllerAnchor;
        cam = rig.centerEyeAnchor.GetComponent<Camera>();
    }

    private void Update()
    {
        if (TryGetWipePoint(out Vector3 point))
        {
            if (lastPoint.HasValue)
                WipeAround(point, Vector3.Distance(point, lastPoint.Value));
            lastPoint = point;
        }
        else
        {
            lastPoint = null;
        }
    }

    private bool TryGetWipePoint(out Vector3 point)
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            point = controller.position;
            return true;
        }

        if (Application.isEditor && PointerInput.RightHeld && RoomScanner.Instance.IsReady)
        {
            Ray ray = cam.ScreenPointToRay(PointerInput.Position);
            if (MRUK.Instance.GetCurrentRoom().Raycast(ray, 10f, out RaycastHit hit))
            {
                point = hit.point;
                return true;
            }
        }

        point = default;
        return false;
    }

    private void WipeAround(Vector3 point, float moved)
    {
        // Идём с конца: пятно может удалиться из списка прямо во время стирания.
        for (int i = SlimeSpot.All.Count - 1; i >= 0; i--)
        {
            SlimeSpot spot = SlimeSpot.All[i];
            float reach = spot.Size * 0.5f + wipeRadius;
            if (Vector3.Distance(point, spot.transform.position) < reach)
                spot.Wipe(moved / rubDistance);
        }
    }
}
