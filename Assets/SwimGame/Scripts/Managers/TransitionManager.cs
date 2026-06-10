using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance { get; private set; }

    private Image fadeImage;
    private bool isTransitioning;

    public static void Ensure()
    {
        if (Instance != null) return;
        new GameObject("TransitionManager").AddComponent<TransitionManager>();
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
        BuildOverlay();
    }

    private void BuildOverlay()
    {
        var canvasGo = new GameObject("TransitionCanvas");
        canvasGo.transform.SetParent(transform, false);
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;
        canvasGo.AddComponent<GraphicRaycaster>();

        var imageGo = new GameObject("FadeImage");
        imageGo.transform.SetParent(canvasGo.transform, false);
        fadeImage = imageGo.AddComponent<Image>();
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.raycastTarget = false;
        var rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        fadeImage.raycastTarget = true;
        fadeImage.DOFade(1f, 0.35f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
            fadeImage.DOFade(0f, 0.35f).SetEase(Ease.OutQuad).SetDelay(0.05f).OnComplete(() =>
            {
                fadeImage.raycastTarget = false;
                isTransitioning = false;
            });
        });
    }
}
