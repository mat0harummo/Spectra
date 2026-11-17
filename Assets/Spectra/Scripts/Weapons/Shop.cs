using System;
using UnityEngine;

// Магазин между уборкой и боем: за монетки улучшаем оружие и лечимся.
// В шлеме: стик вверх/вниз — выбор, A — купить. В редакторе: стрелки и Enter.
public class Shop : MonoBehaviour
{
    public static Shop Instance { get; private set; }

    private class Item
    {
        public string Name;
        public int Price;
        public bool Once;    // можно купить только один раз
        public bool Bought;
        public Action Apply; // что происходит при покупке
    }

    private Item[] items;
    private int selected;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LaserVacuum laser = FindAnyObjectByType<LaserVacuum>();
        items = new[]
        {
            new Item { Name = "Мощный лазер (+1 урон)", Price = 15, Apply = () => laser.AddDamage(1) },
            new Item { Name = "Плазменная метла", Price = 25, Once = true, Apply = laser.EquipBroom },
            new Item { Name = "Аптечка (+30 здоровья)", Price = 10, Apply = () => PlayerHealth.Heal(30) },
        };
    }

    private void Update()
    {
        if (GameManager.Instance.Phase != GamePhase.Shop)
            return;

        if (Pressed(OVRInput.Button.PrimaryThumbstickUp) || (Application.isEditor && PointerInput.UpPressed))
            selected = (selected + items.Length - 1) % items.Length;
        if (Pressed(OVRInput.Button.PrimaryThumbstickDown) || (Application.isEditor && PointerInput.DownPressed))
            selected = (selected + 1) % items.Length;
        if (Pressed(OVRInput.Button.One) || (Application.isEditor && PointerInput.EnterPressed))
            Buy(items[selected]);
    }

    private static bool Pressed(OVRInput.Button button) => OVRInput.GetDown(button, OVRInput.Controller.RTouch);

    private void Buy(Item item)
    {
        if (item.Once && item.Bought)
            return;
        if (!Wallet.Spend(item.Price))
        {
            Debug.Log("[Spectra] Не хватает монет");
            return;
        }

        item.Bought = true;
        item.Apply();
        Sfx.Play(Sfx.Coin, Camera.main.transform.position);
        Debug.Log($"[Spectra] Куплено: {item.Name}");
    }

    // Текст для табло: список товаров, выбранный отмечен стрелкой.
    public string Describe()
    {
        string text = "";
        for (int i = 0; i < items.Length; i++)
        {
            string price = items[i].Once && items[i].Bought ? "куплено" : items[i].Price + " мон.";
            text += (i == selected ? "> " : "   ") + items[i].Name + " — " + price + "\n";
        }
        return text;
    }
}
