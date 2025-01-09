using UnityEngine;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, okayButton, _selector, characterButtonPrefab;
    [SerializeField] BackgroundController backgroundController;

    readonly GameObject[] characterButtons = new GameObject[8];
    GameObject charactersGrid;

    public delegate void SwapCharacterEventHandler(int characterIndex);
    public event SwapCharacterEventHandler OnSwapCharacter;
    

    private void Start()
    {
        charactersGrid = GameObject.FindWithTag("CharactersGrid");
        InitCharacterButtons();
    }

    public void SwapCharacter(int characterIndex)
    {
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("select"), .1f);

        GameManager.Ins.selectedCharacter = characterIndex;
        UpdateSelectorPosition();

        OnSwapCharacter?.Invoke(characterIndex);
    }

    public void OkayButton()
    {
        backgroundController.GrayBackgroundOut();
        okayButton.transform.localScale = Vector2.one;
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("exit"), .1f);

        mainMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    private void UpdateSelectorPosition()
    {
        int character = GameManager.Ins.selectedCharacter;
        _selector.transform.SetParent(characterButtons[character].transform, false);
    }

    private void InitCharacterButtons()
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

            CharacterSelector characterSelector = newCharacterButton.GetComponent<CharacterSelector>();
            characterSelector.SetCharacterIndex(i);
        }

        UpdateSelectorPosition();
    }
}
