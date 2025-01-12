using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject eventFistSelected;

    [SerializeField] float scaleTime, scaleFactor;
    [SerializeField] BackgroundController backgroundController;
    [SerializeField] GameObject[] mainMenuButtons;

    private void Awake()
    {
        Cursor.visible = true;
    }

    private void OnEnable()
    {
        if (NavigationSystem.Ins.CurrentSelected == null)
        {
            Debug.Log("Default set play button");
            NavigationSystem.Ins.Select(eventFistSelected);
        }
    }

    public void PlayButton()
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Play, .1f);

        NavigationSystem.Ins.SetIsNavigating(false);
        TransitionManager.Ins.LoadScene(TMScene.CurrentLevel, TMTransition.LensCircle);
    }

    public void SkinsButton()
    {
        backgroundController.SetIsOnGrayBackground(true);
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        ResetButtonsScale();

        SwapMenuManager.Ins.ToSkins();
        NavigationSystem.Ins.ClearSelected();
    }

    public void SettingsButton()
    {
        backgroundController.SetIsOnGrayBackground(true);
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        SwapMenuManager.Ins.ToSettings();

        NavigationSystem.Ins.ClearSelected();
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
