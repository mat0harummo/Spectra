using UnityEngine;

public enum GamePhase { Menu, Marking, Cleaning, Shop, Battle, Victory, GameOver }

// Фазы игры: меню → разметка грязи → уборка → магазин → бой с призраками → победа или поражение.
// После победы начинается новый раунд: можно отметить и убрать следующие места.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GamePhase Phase { get; private set; } = GamePhase.Menu;

    [SerializeField] private int ghostsToWin = 8; // сколько призраков нужно победить
    public int GhostsToWin => ghostsToWin;

    // Сколько призраков побеждено в текущем бою.
    public int GhostsDefeated => Ghost.Defeated - defeatedBeforeBattle;
    private int defeatedBeforeBattle;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        switch (Phase)
        {
            case GamePhase.Menu:
                if (NextPressed())
                    SetPhase(GamePhase.Marking);
                break;

            case GamePhase.Marking:
                // Отметили хотя бы одно место и нажали «готово» — начинаем уборку.
                if (NextPressed() && SlimeSpot.All.Count > 0)
                    SetPhase(GamePhase.Cleaning);
                break;

            case GamePhase.Cleaning:
                // Всё стёрто — заходим в магазин перед боем.
                if (SlimeSpot.All.Count == 0)
                    SetPhase(GamePhase.Shop);
                break;

            case GamePhase.Shop:
                // Кнопка B (пробел) — закончить покупки и начать бой.
                if (DonePressed())
                {
                    defeatedBeforeBattle = Ghost.Defeated;
                    SetPhase(GamePhase.Battle);
                }
                break;

            case GamePhase.Battle:
                // Призраки отняли всё здоровье — проигрыш. Победили всех — победа.
                if (PlayerHealth.Health <= 0)
                    SetPhase(GamePhase.GameOver);
                else if (GhostsDefeated >= ghostsToWin)
                    SetPhase(GamePhase.Victory);
                break;

            case GamePhase.Victory:
                // Кнопка A (пробел) — новый раунд уборки.
                if (NextPressed())
                    SetPhase(GamePhase.Marking);
                break;

            case GamePhase.GameOver:
                // Кнопка A (пробел) — попробовать ещё раз с полным здоровьем, монетки остаются.
                if (NextPressed())
                {
                    PlayerHealth.Heal(PlayerHealth.Max);
                    SetPhase(GamePhase.Marking);
                }
                break;
        }
    }

    private void SetPhase(GamePhase phase)
    {
        Phase = phase;
        Debug.Log($"[Spectra] Фаза: {phase}");
    }

    // В шлеме: кнопка A на правом контроллере. В редакторе: пробел.
    private static bool NextPressed()
    {
        return OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch)
            || (Application.isEditor && PointerInput.NextPressed);
    }

    // В шлеме: кнопка B на правом контроллере. В редакторе: пробел.
    private static bool DonePressed()
    {
        return OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch)
            || (Application.isEditor && PointerInput.NextPressed);
    }
}
