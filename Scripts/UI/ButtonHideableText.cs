using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ButtonHideableText : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] string content;

    [Header("Animation")]
    [SerializeField] float gap = 50;
    [SerializeField] float moveTime = 0.5f;

    [Header("Prefab")]
    [SerializeField] GameObject extraButtonTextPrefab;

    GameObject hideableText;
    RectTransform rectTransform;
    ButtonsController buttonController;
    TextMeshProUGUI textMeshPro;
    NavigationTarget navigationTarget;

    float initialPosition, extendedPosition;
    bool isHiding = true;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        float canvasScale = GameObject.FindWithTag("Canvas")
            .GetComponent<RectTransform>().localScale.x;

        navigationTarget = GetComponent<NavigationTarget>();

        buttonController = GetComponent<ButtonsController>();

        // Instantiate text gameObject
        hideableText = Instantiate(extraButtonTextPrefab, transform.parent);
        Transform textTransform = hideableText.transform;
        textTransform.SetAsFirstSibling();

        // Set initial text position
        float buttonHeight = rectTransform.rect.height * canvasScale;
        initialPosition = transform.position.y - buttonHeight / 2;
        textTransform.position = new(transform.position.x, initialPosition);

        // Set extended position
        float textHeight = hideableText.GetComponent<RectTransform>().rect.height * canvasScale;
        extendedPosition = initialPosition - (gap * canvasScale) - (textHeight / 2);

        textMeshPro = hideableText.GetComponent<TextMeshProUGUI>();
        textMeshPro.text = GetParsedTextContent();
    }

    private void Update()
    {
        bool isSelectedByNavigation = NavigationSystem.I.CurrentSelected == navigationTarget 
            && NavigationSystem.I.IsNavigating;

        SetIsHiding(!isSelectedByNavigation && !buttonController.PointerIsHovering);
    }

    void SetIsHiding(bool hide)
    {
        if (hide && !isHiding)
        {
            isHiding = true;
            MoveTo(initialPosition, Ease.InCubic);
        }
        else if (!hide && isHiding)
        {
            isHiding = false;
            MoveTo(extendedPosition, Ease.OutBack);
        }
    }

    void MoveTo(float position, Ease ease)
    {
        hideableText.transform.DOKill();

        hideableText.transform
            .DOMoveY(position, moveTime)
            .SetUpdate(true)
            .SetEase(ease);
    }

    string GetParsedTextContent()
    {
        var (kwStartIndex, kwEndIndex) = (content.IndexOf("{"), content.IndexOf("}"));

        if (kwStartIndex == -1 || kwEndIndex == -1)
        {
            if (kwStartIndex != kwEndIndex)
            {
                Debug.LogWarning($"The text of a ButtonHideableText wasn't in a valid format. Recieved: \"{content}\"");
            }
            return content;
        }

        string keyword = content[(kwStartIndex + 1)..kwEndIndex];
        string keywordValue;

        switch (keyword)
        {
            case "level_number":
                keywordValue = GameManager.I.currentLevel.ToString();
                break;

            default:
                Debug.LogWarning($"Unknown keyword on a ButtonHideableText. Recieved: \"{content}\"");
                return content;
        }
        return content.Replace($"{{{keyword}}}", keywordValue);
    }
}
