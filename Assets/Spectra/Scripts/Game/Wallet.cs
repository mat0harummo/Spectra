using System;
using UnityEngine;

// Монетки игрока. Начисляются за стёртую слизь, тратятся в магазине (этап 7).
public static class Wallet
{
    public static int Coins { get; private set; }
    public static event Action<int> Changed;

    public static void Add(int amount)
    {
        Coins += amount;
        Debug.Log($"[Spectra] +{amount} монет, всего: {Coins}");
        Changed?.Invoke(Coins);
    }

    // Обнуляем при каждом запуске Play, даже если Unity не перезагружает скрипты.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Reset()
    {
        Coins = 0;
        Changed = null;
    }
}
