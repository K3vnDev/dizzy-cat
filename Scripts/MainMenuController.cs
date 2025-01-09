using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private float scaleTime, scaleFactor;
    [SerializeField] private GameObject characterSelection, settingsMenu;
    [SerializeField] private BackgroundController backgroundController;

    [SerializeField] private GameObject[] mainMenuButtons;

    private void Start()
    {
        Cursor.visible = true;
    }

    public void PlayButton()
    {
        SFXPlayerSingleton.Ins.PlaySound(
            SFXPlayerSingleton.Ins.GetButtonSound("play"), .1f);
        SceneManager.LoadScene(GameManager.Ins.currentLevel);
    }

    public void SkinsButton()
    {
        backgroundController.GrayBackgroundIn();
        SFXPlayerSingleton.Ins.PlaySound(
            SFXPlayerSingleton.Ins.GetButtonSound("enter"), .1f);
        ResetButtonsScale();

        SwapMenuManager.Ins.ToSkins();
    }

    public void SettingsButton()
    {
        backgroundController.GrayBackgroundIn();
        SFXPlayerSingleton.Ins.PlaySound(
            SFXPlayerSingleton.Ins.GetButtonSound("enter"), .1f);

        SwapMenuManager.Ins.ToSettings();
    }

    public void ExitButtom()
    {
        SFXPlayerSingleton.Ins.PlaySound(SFXPlayerSingleton.Ins.GetButtonSound("exit"), .1f);
        Application.Quit();
    }

    private void ResetButtonsScale()
    {
        foreach (GameObject button  in mainMenuButtons)
        {
            button.transform.localScale = Vector2.one;
        }
    }
}
