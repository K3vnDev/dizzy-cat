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
        //SFXPlayer.Ins.PlayButtonSound(SFXPlayer.ButtonSound.Play, .1f);
        TransitionManager.Ins.LoadScene(TMScene.CurrentLevel, TMTransition.LensCircle);
    }

    public void SkinsButton()
    {
        backgroundController.SetIsOnGrayBackground(true);
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        ResetButtonsScale();

        SwapMenuManager.Ins.ToSkins();
    }

    public void SettingsButton()
    {
        backgroundController.SetIsOnGrayBackground(true);
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        SwapMenuManager.Ins.ToSettings();
    }

    public void ExitButtom()
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);
        Application.Quit();
    }

    private void ResetButtonsScale()
    {
        foreach (GameObject button  in mainMenuButtons)
            button.transform.localScale = Vector2.one;
    }
}
