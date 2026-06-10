using UnityEngine;

public class HapticManager : MonoBehaviour
{
    public static HapticManager Instance { get; private set; }

    private bool hapticsEnabled;

    public static void Ensure()
    {
        if (Instance != null) return;
        new GameObject("HapticManager").AddComponent<HapticManager>();
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        hapticsEnabled = PlayerPrefs.GetInt("haptics_enabled", 1) == 1;
    }

    public bool HapticsEnabled
    {
        get => hapticsEnabled;
        set
        {
            hapticsEnabled = value;
            PlayerPrefs.SetInt("haptics_enabled", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public void Vibrate()
    {
        if (!hapticsEnabled) return;
#if UNITY_IOS || UNITY_ANDROID
        Handheld.Vibrate();
#endif
    }
}
