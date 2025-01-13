using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject eventFistSelected;
    [SerializeField] float scaleTime, scaleFactor;
    [SerializeField] GameObject[] mainMenuButtons;

    [Space]
    [SerializeField] NavigationTarget initialTarget;

    private void Awake()
    {
        Cursor.visible = true;
        NavigationSystem.Ins.Initialize(gameObject, initialTarget);
    }

    private void OnEnable()
    {
        NavigationSystem.Ins.Refresh(gameObject);
    }

    public void PlayButton()
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Play, .1f);
        TransitionManager.Ins.LoadScene(TMScene.CurrentLevel, TMTransition.LensCircle);

        NavigationSystem.Ins.Unselect();
    }

    public void SkinsButton()
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        SwapMenuManager.Ins.ToSkins();

        NavigationSystem.Ins.Unselect();
    }

    public void SettingsButton()
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        SwapMenuManager.Ins.ToSettings();

        NavigationSystem.Ins.Unselect();
    }

    public void ExitButtom()
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);
        Application.Quit();
    }
}
