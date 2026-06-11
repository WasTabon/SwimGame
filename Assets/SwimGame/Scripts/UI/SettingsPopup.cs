using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : PopupBase
{
    private static readonly Color OnColor = new Color32(245, 166, 35, 255);
    private static readonly Color OffColor = new Color32(90, 100, 115, 255);

    [SerializeField] private Button soundButton;
    [SerializeField] private Button musicButton;
    [SerializeField] private Button vibrationButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        soundButton.onClick.AddListener(ToggleSound);
        musicButton.onClick.AddListener(ToggleMusic);
        vibrationButton.onClick.AddListener(ToggleVibration);
        closeButton.onClick.AddListener(() => Hide());
    }

    private void OnEnable()
    {
        SoundManager.Ensure();
        HapticManager.Ensure();
        RefreshAll(false);
    }

    private void ToggleSound()
    {
        SoundManager.Instance.SfxMuted = !SoundManager.Instance.SfxMuted;
        if (!SoundManager.Instance.SfxMuted) SoundManager.Instance.PlaySfx(SfxType.Tap);
        RefreshToggle(soundButton, !SoundManager.Instance.SfxMuted, true);
    }

    private void ToggleMusic()
    {
        SoundManager.Instance.MusicMuted = !SoundManager.Instance.MusicMuted;
        RefreshToggle(musicButton, !SoundManager.Instance.MusicMuted, true);
    }

    private void ToggleVibration()
    {
        HapticManager.Instance.HapticsEnabled = !HapticManager.Instance.HapticsEnabled;
        if (HapticManager.Instance.HapticsEnabled) HapticManager.Instance.Vibrate();
        RefreshToggle(vibrationButton, HapticManager.Instance.HapticsEnabled, true);
    }

    private void RefreshAll(bool animated)
    {
        RefreshToggle(soundButton, !SoundManager.Instance.SfxMuted, animated);
        RefreshToggle(musicButton, !SoundManager.Instance.MusicMuted, animated);
        RefreshToggle(vibrationButton, HapticManager.Instance.HapticsEnabled, animated);
    }

    private void RefreshToggle(Button button, bool isOn, bool animated)
    {
        var image = button.GetComponent<Image>();
        if (image != null) image.color = isOn ? OnColor : OffColor;
        var label = button.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null) label.text = isOn ? "ON" : "OFF";
        if (animated)
        {
            button.transform.DOKill(true);
            button.transform.DOPunchScale(Vector3.one * 0.12f, 0.2f, 4, 0.6f);
        }
    }
}
