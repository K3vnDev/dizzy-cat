using System.Collections;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject mainMenu;

    [Header ("Color Loop")]
    [SerializeField] private Color[] bgcolors;
    [SerializeField] private float transitionTime, waitTime;
    [SerializeField] private AnimationCurve colorTransitionCurve;
    [SerializeField] private float grayBgTransitionTime;
    
    private Vector3 finalPos = new Vector3(-15, -5, 0);
    private Vector3 originalPos = new Vector3(15, 5, 0);
    private SpriteRenderer spriteRenderer;


    void Start()
    {
        transform.position = originalPos;
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(LoopColors(transitionTime, waitTime,
            GameManager.Ins.currentBgColor));
    }

    void Update()
    {
        float deltaSpeed = Time.deltaTime * speed;

        if (transform.position != finalPos)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, finalPos, deltaSpeed);
        }
        else
        {
            transform.position = originalPos;
        }

    }

    public void GrayBackgroundIn()
    {
        StopAllCoroutines();

        StartCoroutine(LerpColor(spriteRenderer.color, 
            bgcolors[bgcolors.Length - 1], grayBgTransitionTime, false));
    }

    public void GrayBackgroundOut()
    {
        StopAllCoroutines();

        Color targetColor = bgcolors[GameManager.Ins.currentBgColor];

        StartCoroutine(LerpColor(spriteRenderer.color,
            targetColor, grayBgTransitionTime / 2, true));
    }

    private IEnumerator LoopColors(float trTime, float wtTime, int currentColor)
    {
        while (true)
        {
            float step = 0;
            Color start = spriteRenderer.color;
            Color target = bgcolors[currentColor];

            while (step < trTime)
            {
                spriteRenderer.color = Color.Lerp(
                    start, target, colorTransitionCurve.Evaluate(step/trTime));

                step += Time.deltaTime;
                yield return null;
            }
            spriteRenderer.color = target;
            yield return new WaitForSeconds(wtTime);

            currentColor++;
            if (currentColor >= bgcolors.Length - 1)
            {
                currentColor = 0;
            }
            GameManager.Ins.currentBgColor = currentColor;
        }
    }

    private IEnumerator LerpColor(Color start, Color target, float time, bool outMode)
    {
        float step = 0;
        while (step < time)
        {
            spriteRenderer.color = Color.Lerp(
                start, target, colorTransitionCurve.Evaluate(step/time));

            step += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = target;

        if (outMode) StartCoroutine(LoopColors(transitionTime, waitTime,
            GameManager.Ins.currentBgColor));
    }
}
