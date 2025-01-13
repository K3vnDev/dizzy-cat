using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuController : MonoBehaviour
{
    public bool GameIsPaused { get; private set; } = false;
    public bool gameCanBePaused = true;
    CanvasGroup canvasGroup;

    [SerializeField] TextMeshProUGUI currentLevelIndicator;

    [SerializeField] NavigationTarget firstSelected;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetCurrentLevelIndicatorText();

        NavigationSystem.Ins.Initialize(gameObject, firstSelected);
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

        if (!GameIsPaused)
        {
            SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        }
    }

    public void MainMenuButton()
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);

        MusicPlayer.Ins.LowerVolume(false);
        Time.timeScale = 1f;

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

        NavigationSystem.Ins.Refresh(gameObject, firstSelected);
    }

    private void OnDisable()
    {
        InputManager.Ins.Game.Pause.performed -= HandleTogglePause;
        InputManager.Ins.UI.Exit.performed -= HandleTogglePause;
    }
}
