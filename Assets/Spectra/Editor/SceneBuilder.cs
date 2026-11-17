using System.IO;
using Meta.XR.MRUtilityKit;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Собирает сцену этапа 1 одним пунктом меню: Spectra > Собрать сцену.
// Камера шлема + passthrough + MRUK + наши скрипты комнаты.
public static class SceneBuilder
{
    private const string RigPath = "Packages/com.meta.xr.sdk.core/Prefabs/OVRCameraRig.prefab";
    private const string ScenePath = "Assets/Spectra/Scenes/Main.unity";

    [MenuItem("Spectra/Собрать сцену")]
    public static void Build()
    {
        var rigPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(RigPath);
        if (rigPrefab == null)
        {
            Debug.LogError("[Spectra] Не найден OVRCameraRig. Сначала установите Meta XR SDK.");
            return;
        }

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);

        // Свет, чтобы виртуальные объекты не были чёрными.
        var light = new GameObject("Light").AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

        // Камера шлема с включённым passthrough (реальная комната вместо фона).
        var rig = (GameObject)PrefabUtility.InstantiatePrefab(rigPrefab);
        rig.GetComponent<OVRManager>().isInsightPassthroughEnabled = true;
        var passthrough = rig.AddComponent<OVRPassthroughLayer>();
        passthrough.overlayType = OVROverlay.OverlayType.Underlay;

        Camera cam = rig.GetComponent<OVRCameraRig>().centerEyeAnchor.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.clear;

        // MRUK загружает скан комнаты из шлема.
        var mruk = new GameObject("MRUK").AddComponent<MRUK>();
        mruk.SceneSettings.DataSource = MRUK.SceneDataSource.Device;
        mruk.SceneSettings.LoadSceneOnStartup = true;

        // Наши скрипты.
        var room = new GameObject("Room");
        room.AddComponent<RoomScanner>();
        room.AddComponent<SurfaceMarkers>();

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

        Debug.Log("[Spectra] Сцена собрана: " + ScenePath);
    }
}
