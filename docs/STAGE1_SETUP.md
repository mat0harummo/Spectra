# Этап 1: основа MR

Цель: в шлеме видна реальная комната, а на полу и столах стоят цветные шарики.
Зелёный шарик означает пол, голубой означает стол.

## 1. Создать проект Unity

1. Установите **Unity 6 LTS** через Unity Hub с модулем **Android Build Support**
   (вместе с OpenJDK и Android SDK/NDK).
2. Создайте новый проект по шаблону **Universal 3D** с именем `Spectra`.
3. Закройте Unity и перенесите содержимое папки проекта в корень этого репозитория
   (рядом с `Assets/Spectra` и `.gitignore`). Скрипты из репозитория окажутся внутри проекта.

## 2. Подключить Meta XR SDK

1. Откройте Asset Store и добавьте в свои ассеты **Meta XR All-in-One SDK**.
2. В Unity: `Window > Package Manager > My Assets`, установите его. Unity попросит
   перезапуск, согласитесь.
3. `File > Build Settings`: выберите **Android** и нажмите **Switch Platform**.
4. `Meta > Tools > Project Setup Tool`: нажмите **Fix All**, потом **Apply All**.
   Повторите, пока не исчезнут красные пункты.

## 3. Собрать сцену через Building Blocks

1. Создайте сцену `Assets/Spectra/Scenes/Main.unity` и удалите из неё `Main Camera`.
2. Откройте `Meta > Tools > Building Blocks` и перетащите в сцену:
   - **Camera Rig**
   - **Passthrough**
   - **Controller Tracking**
   - **MR Utility Kit**
3. Выберите объект **MRUK** в сцене. В поле **Data Source** поставьте
   **Device With Prefab Fallback**. Тогда в редакторе загрузится тестовая комната,
   а в шлеме настоящая.

## 4. Подключить скрипты Spectra

1. Создайте пустой объект `Room` (`GameObject > Create Empty`).
2. Добавьте на него компоненты **RoomScanner** и **SurfaceMarkers**.

## 5. Проверить

- **В редакторе:** нажмите Play. В консоли появится
  `[Spectra] Комната загружена, поверхностей: N`, в сцене появятся шарики.
- **В шлеме:** в настройках Quest 3 отсканируйте комнату (`Пространство > Настройка пространства`).
  Подключите шлем по USB, `File > Build And Run`. При первом запуске разрешите
  доступ к данным пространства.

Если шарики видны на полу и на столе, этап 1 готов.
