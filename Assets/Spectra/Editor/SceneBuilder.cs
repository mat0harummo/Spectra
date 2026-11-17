using System.IO;
using Meta.XR.MRUtilityKit;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Собирает сцену игры одним пунктом меню: Spectra > Собрать сцену.
// Камера шлема + passthrough + MRUK + все наши скрипты.
public static class SceneBuilder
{
    private const string RigPath = "Packages/com.meta.xr.sdk.core/Prefabs/OVRCameraRig.prefab";
    private const string ScenePath = "Assets/Spectra/Scenes/Main.unity";
    private const string RoomsPath = "Packages/com.meta.xr.mrutilitykit/Core/Rooms/Prefabs/LivingRoom/";

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
        rig.AddComponent<OVRPassthroughLayer>();

        // Прозрачный фон у всех камер рига, чтобы сквозь него было видно комнату.
        foreach (Camera cam in rig.GetComponentsInChildren<Camera>(true))
        {
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.clear;
        }

        // MRUK загружает скан комнаты из шлема.
        // Без шлема (в редакторе) берёт одну из тестовых гостиных Meta.
        var mruk = new GameObject("MRUK").AddComponent<MRUK>();
        if (mruk.SceneSettings == null)
            mruk.SceneSettings = new MRUK.MRUKSettings();
        mruk.SceneSettings.DataSource = MRUK.SceneDataSource.DeviceWithPrefabFallback;
        mruk.SceneSettings.RoomPrefabs = new[]
        {
            AssetDatabase.LoadAssetAtPath<GameObject>(RoomsPath + "LivingRoom00.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(RoomsPath + "LivingRoom01.prefab"),
            AssetDatabase.LoadAssetAtPath<GameObject>(RoomsPath + "LivingRoom02.prefab"),
        };
        mruk.SceneSettings.LoadSceneOnStartup = true;

        // Наши скрипты. SurfaceMarkers (отладочные шарики) в игру больше не добавляем.
        var room = new GameObject("Room");
        room.AddComponent<RoomScanner>();
        room.AddComponent<SlimePlacer>();
        room.AddComponent<SlimeWiper>();

        var game = new GameObject("Game");
        game.AddComponent<GameManager>();
        game.AddComponent<GhostSpawner>();
        game.AddComponent<LaserVacuum>();
        game.AddComponent<Shop>();
        new GameObject("Hud").AddComponent<Hud>();

        Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };

        Debug.Log("[Spectra] Сцена собрана: " + ScenePath);
    }
}
