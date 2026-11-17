# Этап 1: основа MR

Цель: в шлеме видна реальная комната, а на полу и столах стоят цветные шарики.
Зелёный шарик означает пол, голубой означает стол.

Играем через кабель Quest Link, поэтому платформа сборки **Windows** (стоит по умолчанию).

## 1. Проект

Unity 6, шаблон **Universal 3D**, путь без кириллицы (например `C:\Spectra\Spectra`).

## 2. Meta XR SDK

1. В `Packages/manifest.json` добавлен реестр Meta (`scopedRegistries`, `https://npm.developer.oculus.com`).
2. `Window > Package Manager`, слева раздел **My Registries > Meta XR**.
   Установите **Meta MR Utility Kit**: он сам подтянет **Meta XR Core SDK**.
3. `Meta > Tools > Project Setup Tool`, вкладка **Windows**: **Fix All**, затем **Apply All**.

## 3. Сцена

Меню **Spectra > Собрать сцену**. Скрипт `SceneBuilder` создаст `Assets/Spectra/Scenes/Main.unity`:
камера шлема (OVRCameraRig) с passthrough, объект MRUK и объект `Room`
со скриптами `RoomScanner` и `SurfaceMarkers`.
После этого ещё раз откройте Project Setup Tool и нажмите **Fix All**:
он включит разрешения для passthrough и данных комнаты.

## 4. Meta Horizon Link (на ПК)

Настройки > Бета: включить **Developer Runtime Features**, **Passthrough over Meta Quest Link**,
**Spatial Data over Meta Quest Link**. В шлеме отсканировать комнату
(Настройки > Физическое пространство > Настройка пространства).

## 5. Проверка

Подключить шлем кабелем, включить Link, нажать **Play**.
В Console: `[Spectra] Комната загружена, поверхностей: N`, в шлеме видны шарики.
