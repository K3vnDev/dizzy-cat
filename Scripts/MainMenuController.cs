using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] float scaleTime, scaleFactor;
    [SerializeField] GameObject characterSelection, settingsMenu;
    [SerializeField] BackgroundController backgroundController;
    [SerializeField] GameObject[] mainMenuButtons;

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
        backgroundController.SetIsOnGrayBackground(true);
        SFXPlayerSingleton.Ins.PlaySound(
            SFXPlayerSingleton.Ins.GetButtonSound("enter"), .1f);
        ResetButtonsScale();

        SwapMenuManager.Ins.ToSkins();
    }

    public void SettingsButton()
    {
        backgroundController.SetIsOnGrayBackground(true);
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
