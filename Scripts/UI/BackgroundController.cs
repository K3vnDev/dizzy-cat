using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [Header ("Colors")]
    [SerializeField] Color grayColor;
    [SerializeField] Color[] loopColors;

    [Header ("Time")]
    [SerializeField] float defaultColorsTime = 1.5f;
    [SerializeField] float grayColorTime = 0.1f;
    [SerializeField] float waitTime = 3f;

    int colorIndex = -1;
    Image image;
    readonly string COLOR_PROPERTY = "_PrimaryCol";


    void Awake()
    {
        image = GetComponent<Image>();
        StartCoroutine(LoopColors());

        RandomizeColorIndex();
        image.material.SetColor(COLOR_PROPERTY, loopColors[colorIndex]);
    }

    void RandomizeColorIndex()
    {
        int randomIndex;

        do randomIndex = Random.Range(0, loopColors.Length);
        while (Mathf.Abs(randomIndex - colorIndex) <= 1);

        colorIndex = randomIndex;
    }

    void TransitionateColor(Color color, float time)
    {
        image.material.DOColor(color, COLOR_PROPERTY, time).SetEase(Ease.OutCubic);
    }

    void StopTransition()
    {
        StopAllCoroutines();
        image.material.DOKill();
    }

    public void SetIsOnGrayBackground(bool isOnGrayBackground)
    {
        StopTransition();

        Color newColor = isOnGrayBackground 
            ? grayColor 
            : loopColors[colorIndex];

        TransitionateColor(newColor, grayColorTime);

        if (!isOnGrayBackground)
        {
            StartCoroutine(LoopColors());
        }
    }

    private IEnumerator LoopColors()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            Color targetColor = loopColors[colorIndex];
            TransitionateColor(targetColor, defaultColorsTime);

            yield return new WaitForSeconds(defaultColorsTime);
            RandomizeColorIndex();
        }
    }
}
