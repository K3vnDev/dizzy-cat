using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LVLTextIndicatorManager : MonoBehaviour
{
    private TextMeshProUGUI tmpText;
    private PlayerController playerController;
    [SerializeField] private float delay, fadeIn, hold, fadeOut;
    private Color currentAnimationColor = new();

    void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        tmpText = GetComponent<TextMeshProUGUI>();
        int currentLevel = GameManager.Ins.currentLevel;
        tmpText.text = $"Level {currentLevel}/10";
        StartCoroutine(TextAnimation());
    }

    private void Update()
    {
        tmpText.color = playerController.gameIsPaused ?
            tmpText.color = Color.white : currentAnimationColor;
    }

    private IEnumerator TextAnimation()
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(FadeTextAlpha(0, 1, fadeIn));
        yield return new WaitForSeconds(hold + fadeIn);
        StartCoroutine(FadeTextAlpha(1, 0, fadeOut));
        yield return new WaitForSeconds(fadeOut);
        tmpText.fontSize *= .7f;
    }

    private IEnumerator FadeTextAlpha(float start, float target, float time)
    {
        Color startColor = new(255, 255, 255, start), 
            targetColor = new(255, 255, 255, target);

        float step = 0;

        while (step < time)
        {
            currentAnimationColor = Color.Lerp(startColor, targetColor, step / time);
            tmpText.color = currentAnimationColor;

            step += Time.deltaTime;
            yield return null;
        }
        currentAnimationColor = targetColor;
        tmpText.color = targetColor;
    }
}
