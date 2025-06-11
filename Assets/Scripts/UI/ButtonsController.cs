using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonsController : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public bool PointerIsHovering { get; private set; } = false;

    public enum Behavior { Default, Disabled, DisabledInGame }
    public Behavior behavior = Behavior.Default;

    [SerializeField] UnityEvent onClick;

    readonly Vector2 HOVERING_SCALE = Vector2.one * 1.05f,
        PRESSED_SCALE = Vector2.one * 0.9f;

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
    
    public void OnPointerEnter(PointerEventData _)
    {
        if (IsDisabled() || !Cursor.visible) return;

        ScaleTo(HOVERING_SCALE);
        PointerIsHovering = true;
    }

    public void OnPointerExit(PointerEventData _)
    {
        ScaleTo(Vector2.one);
        PointerIsHovering = false;
    }

    public void OnPointerDown(PointerEventData _)
    {
        if (IsDisabled()) return;

        ScaleTo(PRESSED_SCALE);
    }

    public void OnPointerUp(PointerEventData _)
    {
        if (PointerIsHovering) ScaleTo(HOVERING_SCALE);
        else ScaleTo(Vector2.one);
    }

    public void OnPointerClick(PointerEventData _)
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

            if (pauseMenu != null)
            {
                isDisabledInGame = !pauseMenu.GameIsPaused;
            }
        }

        return
            TransitionManager.I.IsTransitioning
            || behavior == Behavior.Disabled
            || isDisabledInGame;
    }

    private void OnDisable()
    {
        ScaleTo(Vector2.one, true);
    }
}
