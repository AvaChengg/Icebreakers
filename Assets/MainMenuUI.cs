using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu Music")]
    [SerializeField] private AudioClip _musicClip;

    [Header("Button SFX")]
    [SerializeField] private AudioClip _buttonClip;

    [Header("References")]
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;

    private bool _isLoading = false;

    private void Awake()
    {
        _startButton?.onClick?.AddListener(StartGame);
        _quitButton?.onClick?.AddListener(QuitGame);
    }

    private void Start()
    {
        AudioManager.instance?.PlayMusic(_musicClip);
    }

    private void StartGame()
    {
        if (_isLoading == true)
            return;

        _isLoading = true;
        LoadingSceneController.LoadScene("GameScene");

        AudioManager.instance?.PlaySFX(_buttonClip);
    }

    public void QuitGame()
    {
        AudioManager.instance?.PlaySFX(_buttonClip);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void OnDestroy()
    {
        _startButton?.onClick?.RemoveListener(StartGame);
        _quitButton?.onClick?.RemoveListener(QuitGame);
    }
}
