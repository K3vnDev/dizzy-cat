using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NavigationSystem : MonoBehaviour
{
    public static NavigationSystem I;
    public enum MaterialMode { Navigating, Always, Never }

    NavigationTarget[] targets;
    public NavigationTarget CurrentSelected { get; private set; }
    NavigationTarget defaultSelected;

    public bool IsNavigating { get; private set; } = false;
    [SerializeField] Material originalMaterial;

    [Header("Material Animation")]
    [SerializeField][Range(0, 2)] float fadeInTime = 0.3f;

    [Header ("Move Repeat")]
    [SerializeField][Range (0, 2)] float deafultMoveRepeatRate = 0.2f;
    [SerializeField][Range (0, 2)] float sliderMoveRepeatRate = 0.025f;

    float rateTimer = 0f;

    readonly string OPACITY_NAME = "_Opacity";

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    void Update()
    {
        if (NotAbleToNavigate()) return;

        Vector2 dir = Utils.ParseRawInput(InputManager.I.UI.Navigate.ReadValue<Vector2>());
        bool onXAxis = dir.x != 0 && dir.y == 0;

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

    /// <summary>Initializes the NavigationSystem layout and calculates all NavigationTargets neighbours</summary>
    public void Initialize(GameObject inputsContainer, NavigationTarget firstSelected)
    {
        targets = inputsContainer
            .GetComponentsInChildren<NavigationTarget>();

        StartCoroutine(HandleInitialize(inputsContainer, firstSelected));
    }

    // Waits for the UI to rebuild before calculating the NavigationTargets neighbours
    IEnumerator HandleInitialize(GameObject inputsContainer, NavigationTarget firstSelected)
    {
        RectTransform[] rectTransform = inputsContainer.GetComponentsInChildren<RectTransform>();

        foreach (RectTransform rect in rectTransform)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);

        yield return null;

        foreach (NavigationTarget target in targets)
            target.neighbours.Calculate(targets);

        Refresh(inputsContainer, firstSelected);
    }

    /// <summary>Refreshes the assumed already calculated NavigationSystem layout.</summary>
    public void Refresh(GameObject inputsContainer, NavigationTarget selectTarget = null)
    {
        targets = inputsContainer
            .GetComponentsInChildren<NavigationTarget>();

        if (selectTarget != null)
        {
            Select(selectTarget);
            defaultSelected = selectTarget;
        }
    }

    /// <summary>Forces the select of a given NavigationTarget.</summary>
    public void Select(NavigationTarget target, MaterialMode setMaterial = MaterialMode.Navigating, bool onlyIfNavigating = false)
    {
        if (onlyIfNavigating && !IsNavigating) return;

        Unselect();
        CurrentSelected = target;

        if (setMaterial == MaterialMode.Always || (setMaterial == MaterialMode.Navigating && IsNavigating))
        {
            SetTheMaterialOf(CurrentSelected);
        }
    }

    /// <summary>Unselects the material of the selected NavigationTarget.</summary>
    public void Unselect()
    {
        UnSetTheMaterialOf(CurrentSelected);
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

    public void StartNavigating(bool onlyIfNotNavigating = false)
    {
        if (!onlyIfNotNavigating || !IsNavigating)
        {
            if (CurrentSelected != null)
            {
                IsNavigating = true;
                SetTheMaterialOf(CurrentSelected);
                //Cursor.visible = false;
            }
            else if (defaultSelected != null)
            {
                Select(defaultSelected);
            }
        }
    }

    public void StopNavigating()
    {
        if (!IsNavigating) return;

        IsNavigating = false;
        Unselect();
        //Cursor.visible = true;
    }

    void Navigate(NavigationTarget target)
    {
        if (target == null || !target.isActiveAndEnabled || NotAbleToNavigate()) return;

        Select(target, MaterialMode.Always);
    }

    bool NotAbleToNavigate()
    {
        return CurrentSelected == null || InputManager.I.SelectedMap != IMActionMap.UI || TransitionManager.I.IsTransitioning;
    }

    void HandleUIAccept(InputAction.CallbackContext _)
    {
        if (NotAbleToNavigate()) return;

        if (!IsNavigating)
        {
            StartNavigating(true);
            return;
        }

        CurrentSelected.Trigger();
    }

    void HandleUIClick(InputAction.CallbackContext _)
    {
        if (NotAbleToNavigate()) return;

        StopNavigating();
    }

    void HandleUIExit(InputAction.CallbackContext _)
    {
        if (NotAbleToNavigate()) return;

        StartNavigating(true);
    }

    void SetTheMaterialOf(NavigationTarget target)
    {
        if (target == null || target.graphs == null) return;

        Graphic[] graphs = target.graphs;

        foreach (Graphic graph in graphs)
        {
            if (graph == null) continue;

            graph.material = Instantiate(originalMaterial);

            graph.material
                .DOFloat(1f, OPACITY_NAME, fadeInTime)
                .SetEase(Ease.OutSine)
                .SetUpdate(true);
        }
    }

    void UnSetTheMaterialOf(NavigationTarget target)
    {
        if (target == null || !target.isActiveAndEnabled || target.graphs == null) return;

        Graphic[] graphs = target.graphs;

        foreach (Graphic graph in graphs)
        {
            if (graph.material == null) continue;

            graph.material.DOKill();
            graph.material = null;
        }
    }

    void OnEnable()
    {
        InputManager.I.UI.Accept.performed += HandleUIAccept;
        InputManager.I.UI.Click.performed += HandleUIClick;
        InputManager.I.UI.Exit.performed += HandleUIExit;
    }

    void OnDisable()
    {
        InputManager.I.UI.Accept.performed -= HandleUIAccept;
        InputManager.I.UI.Click.performed -= HandleUIClick;
        InputManager.I.UI.Exit.performed -= HandleUIExit;
    }
}
