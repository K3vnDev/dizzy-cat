using UnityEngine;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] private float maxScale, scaleSpeed;
    [SerializeField] private GameObject[] characterButtons;
    [SerializeField] private GameObject mainMenu, okayButton, _selector;
    [SerializeField] private BackgroundController backgroundController;


    private void Start()
    {
        UpdateSelectorPosition();
    }

    public void TriggerCharacterButton(int characterId)
    {
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("select"), .1f);

        InfoPlayerSingleton.Instance.selectedCharacter = characterId;
        UpdateSelectorPosition();
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
        int character = InfoPlayerSingleton.Instance.selectedCharacter;

        _selector.transform.position = 
            characterButtons[character].transform.position;
    }
}
