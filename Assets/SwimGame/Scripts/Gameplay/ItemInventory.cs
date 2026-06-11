using UnityEngine;

public static class ItemInventory
{
    public static event System.Action OnChanged;

    private static string Key(ItemType type)
    {
        return "item_" + type.ToString().ToLowerInvariant();
    }

    public static int Get(ItemType type)
    {
        return PlayerPrefs.GetInt(Key(type), 0);
    }

    public static void Add(ItemType type, int amount)
    {
        if (amount <= 0) return;
        PlayerPrefs.SetInt(Key(type), Get(type) + amount);
        PlayerPrefs.Save();
        OnChanged?.Invoke();
    }

    public static bool TryConsume(ItemType type)
    {
        int count = Get(type);
        if (count <= 0) return false;
        PlayerPrefs.SetInt(Key(type), count - 1);
        PlayerPrefs.Save();
        OnChanged?.Invoke();
        return true;
    }
}
