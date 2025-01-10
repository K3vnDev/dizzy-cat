using TMPro;
using UnityEngine;
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();
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
            AudioClip enterSound = SFXPlayerSingleton.Ins.GetButtonSound("enter");
            SFXPlayerSingleton.Ins.PlaySound(enterSound, .1f);
        }
    }
    public void MainMenuButton()
    {
        AudioClip exitSound = SFXPlayerSingleton.Ins.GetButtonSound("exit");
        SFXPlayerSingleton.Ins.PlaySound(exitSound, .1f);

        MusicPlayerSingleton.Ins.ToggleVolume(false);
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    void SetCurrentLevelIndicatorText()
    {
        int currentLevel = GameManager.Ins.currentLevel;
        currentLevelIndicator.text = $"Level {currentLevel}/10";
    }
}
