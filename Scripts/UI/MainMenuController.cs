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
        NavigationSystem.I.Initialize(gameObject, initialTarget);
    }

    private void OnEnable()
    {
        NavigationSystem.I.Refresh(gameObject);
    }

    public void PlayButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.ButtonSound.Play, .1f);
        TransitionManager.I.LoadScene(TMScene.CurrentLevel, TMTransition.LensCircle);
    }

    public void SkinsButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        SwapMenuManager.I.ToSkins();

        NavigationSystem.I.Unselect();
    }

    public void SettingsButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.ButtonSound.Enter, .1f);
        SwapMenuManager.I.ToSettings();

        NavigationSystem.I.Unselect();
    }

    public void ExitButtom()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);
        Application.Quit();
    }
}
