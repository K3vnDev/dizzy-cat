using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ExtraButtonTextController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animation")]
    [SerializeField] float gap = 50;
    [SerializeField] float moveTime = 0.5f;

    [Header("Prefab")]
    [SerializeField] GameObject extraButtonTextPrefab;

    [Header("Content")]
    [SerializeField] string textContent;

    GameObject extraBtnText;

    RectTransform rectTransform;
    float initialPosition, extendedPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        float canvasScale = GameObject.FindWithTag("Canvas")
            .GetComponent<RectTransform>().localScale.x;

        // Instantiate text gameObject
        extraBtnText = Instantiate(extraButtonTextPrefab, transform.parent);
        Transform textTransform = extraBtnText.transform;
        textTransform.SetAsFirstSibling();

        // Set initial text position
        float buttonHeight = rectTransform.rect.height * canvasScale;
        initialPosition = transform.position.y - buttonHeight / 2;
        textTransform.position = new(transform.position.x, initialPosition);

        // Set extended position
        float textHeight = extraBtnText.GetComponent<RectTransform>().rect.height * canvasScale;
        extendedPosition = initialPosition - (gap * canvasScale) - (textHeight / 2);
    }

    private void Start()
    {
        SetTextContent();
    }

    void SetTextContent()
    {
        TextMeshProUGUI tmPro = extraBtnText.GetComponent<TextMeshProUGUI>();
        var (startIndex, endIndex) = (textContent.IndexOf("("), textContent.IndexOf(")"));

        if (startIndex == -1 || endIndex == -1)
        {
            tmPro.text = textContent;
            return;
        }

        bool typingKeyword = false;
        string keyWord = "", phraseStart = "", phraseEnd = "";

        foreach (char c in textContent)
        {
            if (c == '(' || c == ')')
            {
                typingKeyword = c == '(';
                continue;
            }
            if (typingKeyword) keyWord += c;
            else if (keyWord == "") phraseStart += c;
            else phraseEnd += c;
        }


        if (keyWord == "level_number")
        {
            keyWord = GameManager.Ins.currentLevel.ToString();
        }
        tmPro.text = $"{phraseStart} {keyWord} {phraseEnd}";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        extraBtnText.transform.DOKill();
        extraBtnText.transform.DOMoveY(extendedPosition, moveTime).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        extraBtnText.transform.DOKill();
        extraBtnText.transform.DOMoveY(initialPosition, moveTime).SetEase(Ease.InCubic);
    }
}
