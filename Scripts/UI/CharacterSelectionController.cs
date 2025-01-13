using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu, okayButton, _selector, characterButtonPrefab;
    [SerializeField] NavigationTarget skinsButton;

    readonly GameObject[] characterButtons = new GameObject[8];
    GameObject charactersGrid;

    private void Awake()
    {
        charactersGrid = GameObject.FindWithTag("CharactersGrid");
        InitCharacterButtons();

        NavigationSystem.I.Initialize(gameObject, GetTarget());
    }

    private void OnEnable()
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
        SFXPlayer.I.PlaySound(SFXPlayer.ButtonSound.Select, .1f);
        GameManager.I.selectedCharacter = characterIndex;
        UpdateSelectorPosition();

        if (!NavigationSystem.I.IsNavigating)
        {
            NavigationSystem.I.Select(GetTarget(), NavigationSystem.SetMaterial.Never);
        }
    }

    public void OkayButton()
    {
        okayButton.transform.localScale = Vector2.one;

        SFXPlayer.I.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);
        NavigationSystem.I.Select(skinsButton, onlyIfNavigating: true);

        SwapMenuManager.I.ToMain();
    }

    private void UpdateSelectorPosition()
    {
        int index = GameManager.I.selectedCharacter;
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
