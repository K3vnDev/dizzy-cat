using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    [Header ("Colors")]
    [SerializeField] Color grayColor;
    [SerializeField] Color[] loopColors;

    [Header ("Time")]
    [SerializeField] float loopColorsTransitionTime = 2.5f;
    [SerializeField] float waitTime = 4f;
    [SerializeField] float grayColorTransitionTime = 0.15f;

    [Header ("Curve")]
    [SerializeField] AnimationCurve colorTransitionCurve;

    int currentColorIndex = -1;
    Image image;


    void Start()
    {
        image = GetComponent<Image>();
        StartCoroutine(LoopColors());

        int newColorIndex = GetRandomColorIndex();
        currentColorIndex = newColorIndex;

        SetRendererColor(loopColors[currentColorIndex]);
    }

    Color GetRendererColor() => image.material.GetColor("_PrimaryCol");
    void SetRendererColor(Color color) => image.material.SetColor("_PrimaryCol", color);

    int GetRandomColorIndex()
    {
        int randomIndex;

        do randomIndex = Random.Range(0, loopColors.Length);
        while (randomIndex == currentColorIndex);

        return randomIndex;
    }

    public void SetIsOnGrayBackground(bool isOnGrayBackground)
    {
        StopAllCoroutines();

        Color color = isOnGrayBackground ? grayColor : loopColors[currentColorIndex];
        StartCoroutine(LerpColor(color, grayColorTransitionTime));

        if (!isOnGrayBackground) StartCoroutine(LoopColors());
    }

    private IEnumerator LoopColors()
    {
        while (true)
        {
            // Wait some time
            yield return new WaitForSeconds(waitTime);

            currentColorIndex = GetRandomColorIndex();
            Color targetColor = loopColors[currentColorIndex];

            // Transitionate color
            yield return LerpColor(targetColor, loopColorsTransitionTime);
        }
    }

    private IEnumerator LerpColor(Color target, float time)
    {
        Color start = GetRendererColor();
        float elapsed = 0;

        while (elapsed < time)
        {
            Color color = Color.Lerp(start, target, colorTransitionCurve.Evaluate(elapsed / time));
            SetRendererColor(color);

            elapsed += Time.deltaTime;
            yield return null;
        }
        SetRendererColor(target);
    }
}
