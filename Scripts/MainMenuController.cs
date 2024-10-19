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
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("play"), .1f);
        SceneManager.LoadScene(InfoPlayerSingleton.Instance.currentLevel);
    }

    public void SkinsButton()
    {
        backgroundController.GrayBackgroundIn();
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("enter"), .1f);
        ResetButtonsScale();

        characterSelection.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SettingsButton()
    {
        backgroundController.GrayBackgroundIn();
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("enter"), .1f);

        settingsMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void ExitButtom()
    {
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("exit"), .1f);
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
