using UnityEngine;

[ExecuteInEditMode]
public class SwapMenuManager : MonoBehaviour
{
    public enum Menus { Main, Skins, Settings }
    [SerializeField] Menus currentMenu;

    public GameObject[] orderedMenus = new GameObject[3];

    public static SwapMenuManager Ins;

    private void Awake()
    {
        if (Ins == null) Ins = this;
        else Destroy(gameObject);
    }

    void HandleCurrentMenu()
    {
        for (int i = 0; i < orderedMenus.Length; i++)
        {
            bool isSelected = (int) currentMenu == i;
            orderedMenus[i].SetActive(isSelected);
        }
    }

    public void ToMain() { currentMenu = Menus.Main; }
    public void ToSkins() { currentMenu = Menus.Skins; }
    public void ToSettings() { currentMenu = Menus.Settings; }


    void Update()
    {
        HandleCurrentMenu();
    }
}
