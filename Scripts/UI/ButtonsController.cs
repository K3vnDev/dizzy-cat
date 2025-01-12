using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public bool pointerIsHovering = false;

    public enum Behavior { Default, Disabled, DisabledInGame }
    public Behavior behavior = Behavior.Default;

    [SerializeField] UnityEvent onClick;

    readonly Vector2 HOVERING_SCALE = Vector2.one * 1.05f,
        PRESSED_SCALE = Vector2.one * 0.95f;

    readonly float ANIM_TIME = 0.3f;

    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void ScaleTo(Vector2 target, bool instant = false)
    {
        float time = instant ? 0 : ANIM_TIME;

        image.rectTransform.DOKill();

        image.rectTransform
            .DOScale(target, time)
            .SetEase(Ease.OutSine)
            .SetUpdate(true);
    }
    
    public void OnPointerEnter(PointerEventData e)
    {
        if (IsDisabled()) return;

        ScaleTo(HOVERING_SCALE);
        pointerIsHovering = true;
    }

    public void OnPointerExit(PointerEventData e)
    {
        ScaleTo(Vector2.one);
        pointerIsHovering = false;
    }

    public void OnPointerDown(PointerEventData e)
    {
        if (IsDisabled()) return;

        ScaleTo(PRESSED_SCALE);
    }

    public void OnPointerUp(PointerEventData e)
    {
        if (pointerIsHovering) ScaleTo(HOVERING_SCALE);
        else ScaleTo(Vector2.one);
    }

    public void OnPointerClick(PointerEventData e)
    {
        if (IsDisabled()) return;

        onClick?.Invoke();
    }

    private bool IsDisabled()
    {
        bool isDisabledInGame = false;

        if (behavior == Behavior.DisabledInGame)
        {
            PauseMenuController pauseMenu = GameObject.FindWithTag("Pause Menu")
                ?.GetComponent<PauseMenuController>();

            if (pauseMenu != null) isDisabledInGame = !pauseMenu.GameIsPaused;
        }
        return 
            TransitionManager.Ins.IsTransitioning 
            || behavior == Behavior.Disabled 
            || isDisabledInGame;
    }

    private void OnDisable()
    {
        ScaleTo(Vector2.one, true);
    }
}
