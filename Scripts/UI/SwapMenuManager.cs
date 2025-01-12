using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class SwapMenuManager : MonoBehaviour
{
    public enum Menu { Main, Skins, Settings }
    [SerializeField] Menu currentMenu;

    public GameObject[] orderedMenus = new GameObject[3];

    public static SwapMenuManager Ins;
    BackgroundController backgroundController;

    [SerializeField] GameObject skinsButton, settingsButton;

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this);
            return;
        }
        Ins = this;

        backgroundController = GameObject.FindWithTag("Background")
           .GetComponent<BackgroundController>();
    }

    void HandleCurrentMenu()
    {
        for (int i = 0; i < orderedMenus.Length; i++)
        {
            bool isSelected = (int) currentMenu == i;
            orderedMenus[i].SetActive(isSelected);
        }
    }

    public void ToMain() { currentMenu = Menu.Main; }
    public void ToSkins() { currentMenu = Menu.Skins; }
    public void ToSettings() { currentMenu = Menu.Settings; }


    void Update()
    {
        HandleCurrentMenu();
    }
    private void OnEnable()
    {
        if (InputManager.Ins == null) return;
        InputManager.Ins.UI.Exit.performed += OnUIBackHandler;
    }

    private void OnDisable()
    {
        if (InputManager.Ins == null) return;
        InputManager.Ins.UI.Exit.performed -= OnUIBackHandler;
    }

    void OnUIBackHandler(InputAction.CallbackContext e)
    {
        if (currentMenu == Menu.Main) return;

        backgroundController.SetIsOnGrayBackground(false);
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);

        if (NavigationSystem.Ins.IsNavigating)
        {
            NavigationSystem.Ins.ClearSelected();

            GameObject newSelected = currentMenu == Menu.Skins ? skinsButton : settingsButton;
            NavigationSystem.Ins.SetSelected(newSelected);
        }
        ToMain();
    }
}
