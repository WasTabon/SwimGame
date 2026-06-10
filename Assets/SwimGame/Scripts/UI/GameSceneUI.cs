using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    [SerializeField] private Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(OnBack);
    }

    private void OnBack()
    {
        TransitionManager.Instance.LoadScene("MainMenu");
    }
}
