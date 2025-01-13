using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NavigationSystem : MonoBehaviour
{
    public static NavigationSystem I;
    public enum SetMaterial { Navigating, Always, Never }

    NavigationTarget[] targets;
    public NavigationTarget CurrentSelected { get; private set; }

    public bool IsNavigating { get; private set; } = false;
    [SerializeField] Material originalMaterial;

    [Header("Animation")]
    [SerializeField][Range(0, 2)] float fadeInTime = 0.3f;
    [SerializeField][Range(0, 2)] float fadeOutTime = 0.9f;

    [Header ("Move Repeat")]
    [SerializeField][Range (0, 2)] float deafultMoveRepeatRate = 0.2f;
    [SerializeField][Range (0, 2)] float sliderMoveRepeatRate = 0.025f;

    float rateTimer = 0f;

    readonly string OPACITY_NAME = "_Opacity", FADE_OUT_ID = "unset-mat";

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        I = this;
    }

    private void Update()
    {
        if (CurrentSelected == null && InputManager.I.SelectedMap != IMActionMap.UI) return;

        Vector2 dir = InputManager.I.UI.Navigate.ReadValue<Vector2>();
        bool onXAxis = Mathf.Abs(dir.x) > Mathf.Abs(dir.y);

        float moveRepeatRate = CurrentSelected.type == NTType.Slider && onXAxis
            ? sliderMoveRepeatRate : deafultMoveRepeatRate;

        if ((dir.x != 0 || dir.y != 0) && rateTimer <= 0)
        {
            HandleNavigate(dir, onXAxis);
            rateTimer = moveRepeatRate;
        }
        else if (rateTimer > 0)
        {
            rateTimer -= Time.unscaledDeltaTime;
        }
    }

    void HandleNavigate(Vector2 dir, bool onXAxis)
    {
        if (!IsNavigating)
        {
            StartNavigating();
            return;
        }

        if (onXAxis)
        {
            if (CurrentSelected.type == NTType.Slider)
            {
                CurrentSelected.Trigger(Mathf.Sign(dir.x));
            }
            else
            {
                if (dir.x < 0)
                    Navigate(CurrentSelected.neighbours.left);

                else if (dir.x > 0)
                    Navigate(CurrentSelected.neighbours.right);
            }
        }
        else
        {
            if (dir.y < 0)
                Navigate(CurrentSelected.neighbours.bottom);

            else if (dir.y > 0)
                Navigate(CurrentSelected.neighbours.top);
        }
    }

    /// <summary>Initializes the NavigationSystem layout and calculates all NavigationTargets neighbours</summary>
    public void Initialize(GameObject inputsContainer, NavigationTarget firstSelected)
    {
        Refresh(inputsContainer, firstSelected);
        StartCoroutine(HandleInitialize(inputsContainer));
    }

    // Waits for the UI to rebuild before calculating the NavigationTargets neighbours
    private IEnumerator HandleInitialize(GameObject obj)
    {
        var rectTransform = obj.GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

        yield return null;

        foreach (NavigationTarget target in targets)
            target.neighbours.Calculate(targets);
    }

    /// <summary>Refreshes the assumed already calculated NavigationSystem layout</summary>
    public void Refresh(GameObject inputsContainer, NavigationTarget selectTarget = null)
    {
        targets = inputsContainer
            .GetComponentsInChildren<NavigationTarget>();

        if (selectTarget != null) CurrentSelected = selectTarget;

        if (IsNavigating) StartNavigating();
    }

    /// <summary>Unselects the material of the selected target</summary>
    public void Unselect()
    {
        UnSetMaterialOf(CurrentSelected);
    }

    /// <summary>Forces the select of a given target</summary>
    public void Select(NavigationTarget target, SetMaterial setMaterial = SetMaterial.Navigating, bool onlyIfNavigating = false)
    {
        if (onlyIfNavigating && !IsNavigating) return;

        Unselect();
        CurrentSelected = target;

        if (setMaterial == SetMaterial.Always || (setMaterial == SetMaterial.Navigating && IsNavigating))
        {
            SetMaterialOf(CurrentSelected);
        }
    }

    public void StartNavigating(bool onlyIfNotNavigating = false)
    {
        if (onlyIfNotNavigating && IsNavigating) return;

        IsNavigating = true;
        SetMaterialOf(CurrentSelected);
    }

    public void StopNavigating()
    {
        if (!IsNavigating) return;

        IsNavigating = false;
        UnSetMaterialOf(CurrentSelected, true);
    }

    void Navigate(NavigationTarget target)
    {
        if (target == null) return;

        UnSetMaterialOf(CurrentSelected);
        SetMaterialOf(target);

        CurrentSelected = target;
    }

    private void OnEnable()
    {
        InputManager.I.UI.Accept.performed += HandleUIAccept;
        InputManager.I.UI.Click.performed += HandleUIClick;
        InputManager.I.UI.Exit.performed += HandleUIExit;
    }

    private void OnDisable()
    {
        InputManager.I.UI.Accept.performed -= HandleUIAccept;
        InputManager.I.UI.Click.performed -= HandleUIClick;
        InputManager.I.UI.Exit.performed -= HandleUIExit;
    }

    private void HandleUIAccept(InputAction.CallbackContext _)
    {
        if (CurrentSelected == null) return;

        if (!IsNavigating)
        {
            StartNavigating();
            return;
        }

        CurrentSelected.Trigger();
    }

    private void HandleUIClick(InputAction.CallbackContext _)
    {
        if (CurrentSelected == null) return;

        StopNavigating();
    }

    private void HandleUIExit(InputAction.CallbackContext _)
    {
        StartNavigating(true);
    }

    void SetMaterialOf(NavigationTarget target)
    {
        Graphic[] graphs = target.graphs;

        foreach (Graphic graph in graphs)
        {
            graph.material = Instantiate(originalMaterial);
            graph.material.DOKill();

            graph.material
                .DOFloat(1f, OPACITY_NAME, fadeInTime)
                .SetEase(Ease.OutSine)
                .SetUpdate(true);
        }
    }

    void UnSetMaterialOf(NavigationTarget target, bool withTransition = false)
    {
        Graphic[] graphs = target.graphs;

        foreach (Graphic graph in graphs)
        {
            if (graph.material == null) continue;

            if (DOTween.IsTweening(FADE_OUT_ID) || !withTransition)
            {
                DOTween.Kill(FADE_OUT_ID);
                graph.material.DOKill();
                graph.material = null;
                continue;
            }

            graph.material
                .DOFloat(0f, OPACITY_NAME, fadeOutTime)
                .SetEase(Ease.InSine)
                .SetId(FADE_OUT_ID)
                .SetUpdate(true)
                .OnComplete(() => graph.material = null);
        }
    }
}
