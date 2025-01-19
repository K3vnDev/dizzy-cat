using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] GameObject eventFistSelected;
    [SerializeField] float scaleTime, scaleFactor;
    [SerializeField] GameObject[] mainMenuButtons;

    [Space]
    [SerializeField] NavigationTarget initialTarget;

    LensCircleController lensCircle;

    private void Awake()
    {
        NavigationSystem.I.Initialize(gameObject, initialTarget);

        HandleInitialAnimation();
    }

    private void OnEnable()
    {
        NavigationSystem.I.Refresh(gameObject);
    }

    public void PlayButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Play, .1f);
        TransitionManager.I.LoadScene(TMScene.CurrentLevel, TMTransition.LensCircle);
    }

    public void SkinsButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Enter, .1f);
        SwapMenuManager.I.SwapMenu(SwapMenuManager.Menu.Skins);
    }

    public void SettingsButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Enter, .1f);
        SwapMenuManager.I.SwapMenu(SwapMenuManager.Menu.Settings);
    }

    public void ExitButtom()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Exit, .1f);
        Application.Quit();
    }


    public void HandleInitialAnimation()
    {
        lensCircle = GameObject.FindWithTag("LensCircle")
            .GetComponent<LensCircleController>();

        if (!TransitionManager.I.IsTransitioning && !GameManager.I.openingAlreadyShown)
        {
            GameManager.I.openingAlreadyShown = true;

            lensCircle.SetRadius(0);
            lensCircle.TransitionOut(0.8f);
        }
    }
}
