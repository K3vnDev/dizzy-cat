using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NavigationSystem : MonoBehaviour
{
    public static NavigationSystem Ins;

    EventSystem eventSystem;
    public bool IsNavigating { get; private set; } = false;

    public GameObject CurrentSelected;
    public GameObject  PreviousSelected { get; private set; }

    [SerializeField] Material originalMaterial;

    readonly float MAX_TIME = 5f, FADE_IN_TIME = 0.3f, FADE_OUT_TIME = 0.9f;
    readonly string OPACITY_NAME = "_Opacity", FADE_OUT_ID = "unset-mat";
    float timeLeft;

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this);
            return;
        }
        Ins = this;

        eventSystem = GetComponent<EventSystem>();
    }

    public void SetSelected(GameObject obj, bool initial = true)
    {
        eventSystem.SetSelectedGameObject(obj);
    }

    public void ClearSelected()
    {
        UnSetMaterial(CurrentSelected);
        SetSelected(null);
    }

    public void SetIsNavigating(bool navigating)
    {
        IsNavigating = navigating;
    }

    void Update()
    {
        if (eventSystem.currentSelectedGameObject != null)
        {
            Utils.OnVariableChange(eventSystem.currentSelectedGameObject, ref CurrentSelected, HandleNavigation);
        }

        if (IsNavigating )
        {
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
            }
            else
            {
                IsNavigating = false;
                UnSetMaterial(CurrentSelected, true);
            }
        }
    }

    void HandleNavigation()
    {
        if (!IsNavigating && PreviousSelected != null)
        {
            eventSystem.SetSelectedGameObject(PreviousSelected);
            IsNavigating = true;
        }

        if (IsNavigating)
        {
            timeLeft = MAX_TIME;
            SetMaterial(CurrentSelected);
        }

        if (PreviousSelected != null && !PreviousSelected.Equals(CurrentSelected))
        {
            UnSetMaterial(PreviousSelected);
        }

        PreviousSelected = CurrentSelected;
    }

    void SetMaterial(GameObject obj)
    {
        Image img = obj.GetComponent<Image>();
        if (img == null) return;

        img.material = Instantiate(originalMaterial);
        img.material.DOKill();

        img.material
            .DOFloat(1f, OPACITY_NAME, FADE_IN_TIME)
            .SetEase(Ease.OutSine)
            .SetUpdate(true);
    }

    void UnSetMaterial(GameObject obj, bool withTransition = false)
    {
        Image img = obj.GetComponent<Image>();
        if (img == null || img.material == null) return;

        if (DOTween.IsTweening(FADE_OUT_ID) || !withTransition)
        {
            DOTween.Kill(FADE_OUT_ID);
            img.material.DOKill();
            img.material = null;
            return;
        }

        img.material
            .DOFloat(0f, OPACITY_NAME, FADE_OUT_TIME)
            .SetEase(Ease.InSine)
            .SetId(FADE_OUT_ID)
            .SetUpdate(true)
            .OnComplete(() => img.material = null);
    }

    private void OnEnable()
    {
        InputManager.Ins.UI.Accept.performed += HandleUIAccept;
    }

    private void OnDisable()
    {
        InputManager.Ins.UI.Accept.performed -= HandleUIAccept;
    }

    void HandleUIAccept(InputAction.CallbackContext _)
    {
        if (!IsNavigating) 
        {
            IsNavigating = true;
            HandleNavigation();
            return;
        }

        ButtonsController buttonController = CurrentSelected.GetComponent<ButtonsController>();
        buttonController.OnPointerClick(null);
    }
}
