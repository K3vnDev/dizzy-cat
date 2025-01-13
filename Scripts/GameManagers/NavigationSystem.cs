using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NavigationSystem : MonoBehaviour
{
    public static NavigationSystem Ins;

    NavigationTarget[] targets;
    NavigationTarget currentSelected;

    public bool IsNavigating = false;
    [SerializeField] Material originalMaterial;

    [Header("Animation")]
    [SerializeField][Range(0, 2)] float fadeInTime = 0.3f;
    [SerializeField][Range(0, 2)] float fadeOutTime = 0.9f;

    [Header ("Move Repeat")]
    [SerializeField][Range (0, 2)] float deafultMoveRepeatRate = 0.2f;

    float rateTimer = 0f;

    readonly string OPACITY_NAME = "_Opacity", FADE_OUT_ID = "unset-mat";

    void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this);
            return;
        }
        Ins = this;
    }

    private void FixedUpdate()
    {
        if (currentSelected == null) return;

        Vector2 dir = InputManager.Ins.UI.Navigate.ReadValue<Vector2>();
        bool onXAxis = Mathf.Abs(dir.x) > Mathf.Abs(dir.y);

        float moveRepeatRate = currentSelected.type == NavigationTarget.Type.Slider && onXAxis
            ? 0 : deafultMoveRepeatRate;

        if ((dir.x != 0 || dir.y != 0) && rateTimer <= 0)
        {
            HandleNavigate(dir, onXAxis);
            rateTimer = moveRepeatRate;
        }
        else if (rateTimer > 0)
        {
            rateTimer -= Time.fixedUnscaledDeltaTime;
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
            if (currentSelected.type == NavigationTarget.Type.Slider)
            {
                currentSelected.Trigger(Mathf.Sign(dir.x));
            }
            else
            {
                if (dir.x < 0)
                    Navigate(currentSelected.neighbours.left);

                else if (dir.x > 0)
                    Navigate(currentSelected.neighbours.right);
            }
        }
        else
        {
            if (dir.y < 0)
                Navigate(currentSelected.neighbours.bottom);

            else if (dir.y > 0)
                Navigate(currentSelected.neighbours.top);
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

        if (selectTarget != null) currentSelected = selectTarget;

        if (IsNavigating) StartNavigating();
    }

    /// <summary>Unselects the material of the selected target</summary>
    public void Unselect()
    {
        UnSetMaterialOf(currentSelected);
    }

    /// <summary>Forces the select of a given target</summary>
    public void Select(NavigationTarget target, bool onlyIfNavigating = false)
    {
        if (onlyIfNavigating && !IsNavigating) return;

        Unselect();

        currentSelected = target;
        SetMaterialOf(currentSelected);
    }

    public void StartNavigating()
    {
        SetMaterialOf(currentSelected);
        IsNavigating = true;
    }

    public void StopNavigating()
    {
        IsNavigating = false;
        UnSetMaterialOf(currentSelected, true);
    }

    void Navigate(NavigationTarget target)
    {
        if (target == null) return;

        UnSetMaterialOf(currentSelected);
        SetMaterialOf(target);

        currentSelected = target;
    }

    private void OnEnable()
    {
        InputManager.Ins.UI.Accept.performed += HandleUIAccept;
    }

    private void OnDisable()
    {
        InputManager.Ins.UI.Accept.performed -= HandleUIAccept;
    }

    private void HandleUIAccept(UnityEngine.InputSystem.InputAction.CallbackContext _)
    {
        if (currentSelected == null) return;

        if (!IsNavigating)
        {
            StartNavigating();
            return;
        }

        currentSelected.Trigger();
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
                .OnComplete(() =>
                {
                    Debug.Log($"Graph material of {graph.name} was unset from timeout");
                    graph.material = null;
                });
        }
    }
}
