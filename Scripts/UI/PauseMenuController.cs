using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public bool GameIsPaused { get; private set; } = false;
    public bool gameCanBePaused = true;
    CanvasGroup canvasGroup;

    bool isOnFirstPause = true;

    [SerializeField] TextMeshProUGUI currentLevelIndicator;

    [SerializeField] NavigationTarget firstSelected;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetCurrentLevelIndicatorText();

        NavigationSystem.I.Initialize(gameObject, firstSelected);
    }

    public void SetGameIsPaused(bool value)
    {
        if ((!gameCanBePaused && value) || (value == GameIsPaused)) return;

        GameIsPaused = value;

        canvasGroup.interactable = GameIsPaused;
        canvasGroup.alpha = GameIsPaused ? 1 : 0;

        Time.timeScale = GameIsPaused ? 0 : 1;
        Cursor.visible = GameIsPaused;

        MusicPlayer.I.LowerVolume(GameIsPaused);

        if (GameIsPaused)
        {
            InputManager.I.UseActionMap(IMActionMap.UI);

            if (isOnFirstPause)
            {
                isOnFirstPause = false;
                return;
            }
            NavigationSystem.I.Select(firstSelected);
        }
        else
        {
            InputManager.I.UseActionMap(IMActionMap.Game);
            SFXPlayer.I.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        }
    }

    public void MainMenuButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);

        MusicPlayer.I.LowerVolume(false);
        Time.timeScale = 1f;

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

    private void OnEnable()
    {
        InputManager.I.Game.Pause.performed += HandleGamePauseInput;
        InputManager.I.UI.Exit.performed += HandleUIExitInput;
    }

    private void OnDisable()
    {
        InputManager.I.Game.Pause.performed -= HandleGamePauseInput;
        InputManager.I.UI.Exit.performed -= HandleUIExitInput;
    }
}
