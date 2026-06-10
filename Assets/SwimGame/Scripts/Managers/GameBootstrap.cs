using DG.Tweening;
using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    private static bool initialized;

    private void Awake()
    {
        if (!initialized)
        {
            initialized = true;
            Application.targetFrameRate = 60;
            DOTween.Init();
            DOTween.SetTweensCapacity(500, 50);
            DOTween.defaultRecyclable = false;
        }
        SoundManager.Ensure();
        HapticManager.Ensure();
        TransitionManager.Ensure();
    }
}