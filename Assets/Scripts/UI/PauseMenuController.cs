using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PauseMenuController : MonoBehaviour
{
    public bool GameIsPaused { get; private set; } = false;
    public bool gameCanBePaused = true;

    [SerializeField] TextMeshProUGUI currentLevelIndicator;
    [SerializeField] NavigationTarget firstSelected;

    [Header ("Canvas Groups")]
    [SerializeField] CanvasGroup inGameMenuCanvas;
    [SerializeField] CanvasGroup pausedMenuCanvas;

    void Awake()
    {
        SetCurrentLevelIndicatorText();
        RefreshCanvasGroups();

        NavigationSystem.I.Initialize(gameObject, firstSelected);
    }

    public void SetGameIsPaused(bool value)
    {
        if (TransitionManager.I.IsTransitioning || (!gameCanBePaused && value) || (value == GameIsPaused)) return;

        GameIsPaused = value;

        RefreshCanvasGroups();
        Time.timeScale = GameIsPaused ? 0 : 1;
        //Cursor.visible = GameIsPaused && !NavigationSystem.I.IsNavigating;

        if (GameIsPaused)
        {
            InputManager.I.UseActionMap(IMActionMap.UI);
            MusicPlayer.I.LowSourceVolume();
            NavigationSystem.I.Select(firstSelected);
        }
        else
        {
            InputManager.I.UseActionMap(IMActionMap.Game);
            SFXPlayer.I.PlaySound(SFXPlayer.Sound.Enter, .1f);
            MusicPlayer.I.DefaultSourceVolume();
        }
    }

    public void MainMenuButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Exit, .1f);
        TransitionManager.I.LoadScene(TMScene.MainMenu, TMTransition.LensCircle);
    }

    void RefreshCanvasGroups()
    {
        static void SetValues(CanvasGroup canvasGroup, bool value)
        {
            canvasGroup.interactable = value;
            canvasGroup.alpha = value ? 1 : 0;
        }

        SetValues(pausedMenuCanvas, GameIsPaused);
        SetValues(inGameMenuCanvas, !GameIsPaused);
    }

    void SetCurrentLevelIndicatorText()
    {
        int currentLevel = GameManager.I.currentLevel;
        currentLevelIndicator.text = $"Level {currentLevel}/10";
    }

    void HandleGamePauseInput(InputAction.CallbackContext _) => SetGameIsPaused(true);

    void HandleUIExitInput(InputAction.CallbackContext _) => SetGameIsPaused(false);

    void OnEnable()
    {
        InputManager.I.Game.Pause.performed += HandleGamePauseInput;
        InputManager.I.UI.Exit.performed += HandleUIExitInput;
    }

    void OnDisable()
    {
        InputManager.I.Game.Pause.performed -= HandleGamePauseInput;
        InputManager.I.UI.Exit.performed -= HandleUIExitInput;
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus) SetGameIsPaused(true);
    }
}
