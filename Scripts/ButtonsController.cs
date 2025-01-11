using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonsController : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public bool specialButton, pointerHovering = false, disabled = false, disabledInGame = false;

    Vector2 maxScale = Vector2.one, minScale = Vector2.one;
    readonly float ANIMATION_TIME = 0.3f;

    [SerializeField] UnityEvent onClick;
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
    
    //Hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disabled || IsDisabled()) return;

        StopAllCoroutines();
        StartCoroutine(LerpScale(transform.localScale, maxScale, ANIMATION_TIME, easeOutCurve));
        pointerHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(LerpScale(transform.localScale, minScale, ANIMATION_TIME * .75f, easeOutCurve));
        pointerHovering = false;
    }

    //Click
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsDisabled()) return;

        StopAllCoroutines();
        StartCoroutine(LerpScale(transform.localScale, minScale, ANIMATION_TIME * .75f, easeOutCurve));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (pointerHovering)
        {
            StopAllCoroutines();
            StartCoroutine(LerpScale(transform.localScale, maxScale, ANIMATION_TIME, easeOutCurve));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsDisabled()) return;

        onClick?.Invoke();
    }
    private bool IsDisabled()
    {
        bool isTransitioning = TransitionManager.Ins.CurrentTransition != TMTransition.None;

        bool isDisabledInGame = false;

        if (disabledInGame)
        {
            PauseMenuController pauseMenu = GameObject
                .FindWithTag("Pause Menu").GetComponent<PauseMenuController>();

            if (pauseMenu != null) isDisabledInGame = !pauseMenu.gameIsPaused;
        }
        return isTransitioning || disabled || isDisabledInGame;
    }
}
