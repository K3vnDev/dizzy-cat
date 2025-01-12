using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public bool GameIsPaused { get; private set; } = false;
    public bool gameCanBePaused = true;
    CanvasGroup canvasGroup;

    public static PauseMenuController Ins;
    [SerializeField] TextMeshProUGUI currentLevelIndicator;

    [SerializeField] GameObject resumeButton, mainMenuButton;

    private void Awake()
    {
        if (Ins == null) Ins = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetCurrentLevelIndicatorText();

    }

    public void TogglePause()
    {
        if (!gameCanBePaused) return;

        GameIsPaused = !GameIsPaused;

        canvasGroup.interactable = GameIsPaused;
        canvasGroup.alpha = GameIsPaused ? 1 : 0;

        Time.timeScale = GameIsPaused ? 0 : 1;
        Cursor.visible = GameIsPaused;

        MusicPlayer.Ins.LowerVolume(GameIsPaused);

        if (GameIsPaused)
        {
            NavigationSystem.Ins.SetSelected(resumeButton);
        }
        else
        {
            SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);

            NavigationSystem.Ins.ClearSelected();
            NavigationSystem.Ins.SetIsNavigating(false);
        }
    }
    public void MainMenuButton()
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);

        MusicPlayer.Ins.LowerVolume(false);
        Time.timeScale = 1f;

        NavigationSystem.Ins.ClearSelected();

        TransitionManager.Ins.LoadScene(TMScene.MainMenu, TMTransition.LensCircle);
    }

    void SetCurrentLevelIndicatorText()
    {
        int currentLevel = GameManager.Ins.currentLevel;
        currentLevelIndicator.text = $"Level {currentLevel}/10";
    }

    void HandleTogglePause(InputAction.CallbackContext _) => TogglePause();

    private void OnEnable()
    {
        InputManager.Ins.Game.Pause.performed += HandleTogglePause;
        InputManager.Ins.UI.Exit.performed += HandleTogglePause;

    }

    private void OnDisable()
    {
        InputManager.Ins.Game.Pause.performed -= HandleTogglePause;
        InputManager.Ins.UI.Exit.performed -= HandleTogglePause;
    }
}
