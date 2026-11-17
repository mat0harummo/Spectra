using UnityEngine;

// Табло перед глазами игрока: монеты и подсказка, что делать сейчас.
// Плавно следует за взглядом, чтобы не мешать и не теряться.
public class Hud : MonoBehaviour
{
    [SerializeField] private float distance = 1.2f;  // как далеко перед игроком, м
    [SerializeField] private float height = -0.3f;   // чуть ниже уровня глаз
    [SerializeField] private float followSpeed = 3f;

    private Transform head;
    private TextMesh text;

    private void Start()
    {
        head = FindAnyObjectByType<OVRCameraRig>().centerEyeAnchor;

        // Встроенный шрифт Unity: ничего не нужно импортировать.
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        gameObject.AddComponent<MeshRenderer>().material = font.material;
        text = gameObject.AddComponent<TextMesh>();
        text.font = font;
        text.fontSize = 64;
        text.characterSize = 0.01f;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
    }

    private void LateUpdate()
    {
        Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;
        Vector3 target = head.position + forward * distance + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, target, followSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(transform.position - head.position);

        bool menu = GameManager.Instance.Phase == GamePhase.Menu;
        text.text = menu ? Hint() : $"Монеты: {Wallet.Coins}\n{Hint()}";
    }

    private static string Hint()
    {
        switch (GameManager.Instance.Phase)
        {
            case GamePhase.Menu:
                return "SPECTRA\n\nОтметь грязные места в комнате,\nубери их по-настоящему и защити чистоту от призраков\n\nНачать: кнопка A (пробел)";
            case GamePhase.Marking:
                return "Отметь, что нужно убрать: курок (в редакторе ЛКМ)\nГотово: кнопка A (пробел)";
            case GamePhase.Cleaning:
                return $"Убери и сотри слизь: зажми grip (ПКМ)\nОсталось пятен: {SlimeSpot.All.Count}";
            case GamePhase.Battle:
                return "Стреляй: курок (ЛКМ). Стирай слизь: grip (ПКМ)\n" +
                       $"Здоровье: {PlayerHealth.Health}  Призраки: {GameManager.Instance.GhostsDefeated}/{GameManager.Instance.GhostsToWin}";
            case GamePhase.Shop:
                return "МАГАЗИН\n" + Shop.Instance.Describe() +
                       "Выбор: стик (стрелки). Купить: A (Enter). В бой: B (пробел)";
            case GamePhase.Victory:
                return "Победа! Призраки изгнаны\nНовый раунд уборки: кнопка A (пробел)";
            default:
                return "Призраки победили...\nЕщё раз: кнопка A (пробел)";
        }
    }
}
