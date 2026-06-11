using UnityEngine;

public static class CurrencyManager
{
    private const string CoinsKey = "coins";

    public static event System.Action<int> OnBalanceChanged;

    public static int Balance => PlayerPrefs.GetInt(CoinsKey, 0);

    public static void Add(int amount)
    {
        if (amount <= 0) return;
        int newBalance = Balance + amount;
        PlayerPrefs.SetInt(CoinsKey, newBalance);
        PlayerPrefs.Save();
        OnBalanceChanged?.Invoke(newBalance);
    }

    public static bool TrySpend(int amount)
    {
        if (amount <= 0) return true;
        int balance = Balance;
        if (balance < amount) return false;
        PlayerPrefs.SetInt(CoinsKey, balance - amount);
        PlayerPrefs.Save();
        OnBalanceChanged?.Invoke(balance - amount);
        return true;
    }
}
