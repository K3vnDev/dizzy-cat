using System.Collections;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] GameObject mainMenu;

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
    Renderer _renderer;


    void Start()
    {
        _renderer = GetComponent<Renderer>();
        StartCoroutine(LoopColors());

        int newColorIndex = GetRandomColorIndex();
        currentColorIndex = newColorIndex;

        SetRendererColor(loopColors[currentColorIndex]);
    }

    Color GetRendererColor() => _renderer.material.GetColor("_PrimaryCol");
    void SetRendererColor(Color color) => _renderer.material.SetColor("_PrimaryCol", color);

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
