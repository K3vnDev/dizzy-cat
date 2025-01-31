using UnityEngine;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, okayButton, _selector, characterButtonPrefab;
    [SerializeField] NavigationTarget skinsButton;

    readonly GameObject[] characterButtons = new GameObject[8];
    GameObject charactersGrid;



    void Awake()
    {
        charactersGrid = GameObject.FindWithTag("CharactersGrid");
        InitCharacterButtons();

        NavigationSystem.I.Initialize(gameObject, GetTarget());
    }

    void OnEnable()
    {
        NavigationSystem.I.Refresh(gameObject, GetTarget());
    }

    NavigationTarget GetTarget()
    {
        return characterButtons[GameManager.I.selectedCharacter]
            .GetComponentInChildren<NavigationTarget>();
    }

    public void SwapCharacter(int characterIndex)
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Select, .1f);
        GameManager.I.selectedCharacter = characterIndex;
        UpdateSelectorPosition();

        if (!NavigationSystem.I.IsNavigating)
        {
            NavigationSystem.I.Select(GetTarget(), NavigationSystem.MaterialMode.Never);
        }
    }

    public void OkayButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Exit, 0.1f);
        NavigationSystem.I.Select(skinsButton);

        SwapMenuManager.I.SwapMenu(SwapMenuManager.Menu.Main);
    }

    void UpdateSelectorPosition()
    {
        int index = GameManager.I.selectedCharacter;
        _selector.transform.SetParent(characterButtons[index].transform, false);
        _selector.transform.SetAsFirstSibling();
    }

    void InitCharacterButtons()
    {
        // Destroy old character buttons if theres any
        foreach (Transform oldCharacterButton in charactersGrid.transform)
            Destroy(oldCharacterButton.gameObject);


        // Create new character buttons 
        for (int i = 0; i < characterButtons.Length; i++)
        {
            GameObject newCharacterButton = Instantiate(characterButtonPrefab);
            newCharacterButton.transform.SetParent(charactersGrid.transform, false);

            characterButtons[i] = newCharacterButton;

            CharacterSelector characterSelector = newCharacterButton
                .GetComponentInChildren<CharacterSelector>();

            characterSelector.SetCharacterIndex(i);
        }

        UpdateSelectorPosition();
    }
}
