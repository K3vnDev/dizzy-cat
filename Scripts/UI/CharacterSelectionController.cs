using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, okayButton, _selector, characterButtonPrefab, skinsButton;
    [SerializeField] BackgroundController backgroundController;

    readonly GameObject[] characterButtons = new GameObject[8];
    GameObject charactersGrid;

    private void Awake()
    {
        charactersGrid = GameObject.FindWithTag("CharactersGrid");
        InitCharacterButtons();
    }

    private void OnEnable()
    {
        GameObject button = characterButtons[GameManager.Ins.selectedCharacter]
            .GetComponentInChildren<ButtonsController>().gameObject;

        NavigationSystem.Ins.SetSelected(button);
    }

    public void SwapCharacter(int characterIndex)
    {
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Select, .1f);

        GameManager.Ins.selectedCharacter = characterIndex;
        UpdateSelectorPosition();
    }

    public void OkayButton()
    {
        backgroundController.SetIsOnGrayBackground(false);
        okayButton.transform.localScale = Vector2.one;

        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);

        SwapMenuManager.Ins.ToMain();

        NavigationSystem.Ins.ClearSelected();
        NavigationSystem.Ins.SetSelected(skinsButton, false);
    }

    private void UpdateSelectorPosition()
    {
        int index = GameManager.Ins.selectedCharacter;
        _selector.transform.SetParent(characterButtons[index].transform, false);
        _selector.transform.SetAsFirstSibling();
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

            CharacterSelector characterSelector = newCharacterButton
                .GetComponentInChildren<CharacterSelector>();

            characterSelector.SetCharacterIndex(i);
        }

        UpdateSelectorPosition();
    }
}
