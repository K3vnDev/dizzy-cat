using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public bool GameIsPaused { get; private set; } = false;
    public bool gameCanBePaused = true;

    [SerializeField] TextMeshProUGUI currentLevelIndicator;
    [SerializeField] NavigationTarget firstSelected;

    CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetCurrentLevelIndicatorText();

        InputManager.I.Game.Pause.performed += HandleGamePauseInput;
        InputManager.I.UI.Exit.performed += HandleUIExitInput;

        NavigationSystem.I.Initialize(gameObject, firstSelected);
    }

    public void SetGameIsPaused(bool value)
    {
        if (TransitionManager.I.IsTransitioning || (!gameCanBePaused && value) || (value == GameIsPaused)) return;

        GameIsPaused = value;

        canvasGroup.interactable = GameIsPaused;
        canvasGroup.alpha = GameIsPaused ? 1 : 0;

        Time.timeScale = GameIsPaused ? 0 : 1;
        Cursor.visible = GameIsPaused && !NavigationSystem.I.IsNavigating;

        if (GameIsPaused)
        {
            InputManager.I.UseActionMap(IMActionMap.UI);
            MusicPlayer.I.LowVolume();
            NavigationSystem.I.Select(firstSelected);
        }
        else
        {
            InputManager.I.UseActionMap(IMActionMap.Game);
            SFXPlayer.I.PlaySound(SFXPlayer.Sound.Enter, .1f);
            MusicPlayer.I.DefaultVolume();
        }
    }

    public void MainMenuButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Exit, .1f);
        TransitionManager.I.LoadScene(TMScene.MainMenu, TMTransition.LensCircle);
    }

    void SetCurrentLevelIndicatorText()
    {
        int currentLevel = GameManager.I.currentLevel;
        currentLevelIndicator.text = $"Level {currentLevel}/10";
    }

    void HandleGamePauseInput(InputAction.CallbackContext _)
    {
        SetGameIsPaused(true);
    }

    void HandleUIExitInput(InputAction.CallbackContext _)
    {
        SetGameIsPaused(false);
    }

    private void OnDisable()
    {
        InputManager.I.Game.Pause.performed -= HandleGamePauseInput;
        InputManager.I.UI.Exit.performed -= HandleUIExitInput;
    }
}
