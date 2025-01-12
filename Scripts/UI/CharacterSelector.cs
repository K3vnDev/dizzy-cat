using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    [HideInInspector] public int characterIndex;
    CharacterSelectionController controller;

    void Start()
    {
        controller = GameObject.FindWithTag("CharacterController")
            .GetComponent<CharacterSelectionController>();
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
