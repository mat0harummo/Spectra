using System;
using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

// Ждёт, пока MRUK загрузит комнату, и собирает поверхности,
// на которых потом будет появляться грязь (пол и столы).
// Запускается раньше MRUK, чтобы успеть выбрать источник комнаты.
[DefaultExecutionOrder(-1000)]
public class RoomScanner : MonoBehaviour
{
    public static RoomScanner Instance { get; private set; }

    // Вызывается один раз, когда комната готова.
    public event Action OnRoomReady;

    public List<MRUKAnchor> Surfaces { get; } = new List<MRUKAnchor>();
    public bool IsReady { get; private set; }

    [SerializeField] private MRUKAnchor.SceneLabels surfaceLabels =
        MRUKAnchor.SceneLabels.FLOOR | MRUKAnchor.SceneLabels.TABLE;

    // Выключено: в редакторе берём тестовую комнату. Включить, когда шлем подключён по Link.
    [SerializeField] private bool useHeadsetInEditor = false;

    private void Awake()
    {
        Instance = this;

        if (Application.isEditor && !useHeadsetInEditor)
        {
            MRUK mruk = FindAnyObjectByType<MRUK>();
            if (mruk != null)
                mruk.SceneSettings.DataSource = MRUK.SceneDataSource.Prefab;
        }
    }

    private void Start()
    {
        MRUK.Instance.RegisterSceneLoadedCallback(HandleSceneLoaded);
    }

    private void HandleSceneLoaded()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room == null)
        {
            Debug.LogWarning("[Spectra] Комната не найдена. Отсканируйте комнату в настройках шлема.");
            return;
        }

        Surfaces.Clear();
        foreach (MRUKAnchor anchor in room.Anchors)
        {
            if ((anchor.Label & surfaceLabels) != 0 && anchor.PlaneRect.HasValue)
                Surfaces.Add(anchor);
        }

        Debug.Log($"[Spectra] Комната загружена, поверхностей: {Surfaces.Count}");
        IsReady = true;
        OnRoomReady?.Invoke();
    }
}
