using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonsController : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private bool specialButton;
    private bool pointerHovering = false;

    private Vector2 maxScale = Vector2.one, minScale = Vector2.one;
    private float time = .3f;

    [SerializeField] AnimationCurve easeOutCurve;

    private void Awake()
    {
        maxScale *= specialButton ? 1.04f : 1.075f;
    }

    private IEnumerator LerpScale(Vector2 start, 
        Vector2 target, float time, AnimationCurve curve)
    {
        float step = 0;
        while (step < time)
        {
            transform.localScale = Vector2.Lerp(
                start, target, curve.Evaluate(step/time));

            step += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localScale = target;
    }
    
    //On Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(LerpScale(transform.localScale, maxScale, time, easeOutCurve));
        pointerHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(LerpScale(transform.localScale, minScale, time * .75f, easeOutCurve));
        pointerHovering = false;
    }

    //On Click
    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(LerpScale(transform.localScale, minScale, time * .75f, easeOutCurve));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pointerHovering)
        {
            StopAllCoroutines();
            StartCoroutine(LerpScale(transform.localScale, maxScale, time, easeOutCurve));
        }
    }
}
