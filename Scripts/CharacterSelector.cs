using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public int characterIndex;
    CharacterSelectionController controller;
    ButtonsController buttonsController;

    void Start()
    {
        controller =  GameObject.FindWithTag("CharacterController")
            .GetComponent<CharacterSelectionController>();

        controller.OnSwapCharacter += HandleCharacterSwap;
        buttonsController = GetComponent<ButtonsController>();
    }

    public void HandleCharacterSwap(int characterIndex)
    {
        buttonsController.isDisabled = characterIndex == this.characterIndex;
    }

    public void SetCharacterIndex(int index)
    {
        Image image = GetComponent<Image>();
        characterIndex = index;

        image.sprite = GameManager.Ins.characterSprites[index];
    }

    public void HandleClick()
    {
        controller.SwapCharacter(characterIndex);
    }
}
