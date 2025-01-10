using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public bool gameIsPaused = false;
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
        gameIsPaused = !gameIsPaused;

        canvasGroup.interactable = gameIsPaused;
        canvasGroup.alpha = gameIsPaused ? 1 : 0;

        Time.timeScale = gameIsPaused ? 0 : 1;
        Cursor.visible = gameIsPaused;

        MusicPlayerSingleton.Ins.ToggleVolume(gameIsPaused);

        if (!gameIsPaused)
        {
            SFXPlayer.Ins.PlayButtonSound(SFXPlayer.ButtonSound.Enter, .1f);
        }
    }
    public void MainMenuButton()
    {
        SFXPlayer.Ins.PlayButtonSound(SFXPlayer.ButtonSound.Exit, .1f);

        MusicPlayerSingleton.Ins.ToggleVolume(false);
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
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
