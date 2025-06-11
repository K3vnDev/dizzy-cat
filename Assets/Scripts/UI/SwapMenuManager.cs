using UnityEngine;
using UnityEngine.InputSystem;

public class SwapMenuManager : MonoBehaviour
{
    public enum Menu { Main, Skins, Settings }
    [SerializeField] Menu currentMenu;

    public GameObject[] orderedMenus = new GameObject[3];

    public NavigationTarget[] orderedExitButtons = new NavigationTarget[2];

    public static SwapMenuManager I;
    BackgroundController backgroundController;

    readonly float layoutBreakAspectRatio = 1.9f;
    public bool isOnBrokenLayout = false;

    [SerializeField] GameObject skinsButton, settingsButton;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        I = this;

        backgroundController = GameObject.FindWithTag("Background")
           .GetComponent<BackgroundController>();

        InputManager.I.UseActionMap(IMActionMap.UI);

        float aspectRatio = (float)Screen.width / Screen.height;
        isOnBrokenLayout = aspectRatio > layoutBreakAspectRatio;
    }

    public void SwapMenu(Menu menu)
    {
        currentMenu = menu;
        backgroundController.SetIsOnGrayBackground(menu != Menu.Main);

        for (int i = 0; i < orderedMenus.Length; i++)
            orderedMenus[i].SetActive((int) menu == i);
    }

    private void OnEnable()
    {
        if (InputManager.I == null) return;
        InputManager.I.UI.Exit.performed += OnUIBackHandler;
    }

    private void OnDisable()
    {
        if (InputManager.I == null) return;
        InputManager.I.UI.Exit.performed -= OnUIBackHandler;
    }

    void OnUIBackHandler(InputAction.CallbackContext e)
    {
        if (currentMenu == Menu.Main) return;

        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Exit, 0.1f);
        backgroundController.SetIsOnGrayBackground(false);

        orderedExitButtons[(int) currentMenu - 1].Trigger();
    }
}
