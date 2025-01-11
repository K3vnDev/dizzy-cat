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

    private void Awake()
    {
        if (Ins == null) Ins = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetCurrentLevelIndicatorText();

        InputManager.Ins.UI.Pause.performed += HandleTogglePause;
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

    private void OnDisable()
    {
        InputManager.Ins.UI.Pause.performed -= HandleTogglePause;
    }
}
