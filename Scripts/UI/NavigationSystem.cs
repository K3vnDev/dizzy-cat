using DG.Tweening;
using System;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class NavigationSystem : MonoBehaviour
{
    public static NavigationSystem Ins;

    EventSystem eventSystem;
    InputSystemUIInputModule eventSystemInput;
    public bool IsNavigating { get; private set; } = false;

    public GameObject CurrentSelected;
    public GameObject  PreviousSelected { get; private set; }

    [SerializeField] Material originalMaterial;

    [Header("Move Repeat Speed")]
    [SerializeField] Vector2 normalDelayRate;
    [SerializeField] Vector2 fastDelayRate;

    [Header ("Time")]
    [SerializeField] float maxTime = 5f;
    [SerializeField][Range(0, 2)] float fadeInTime = 0.3f;
    [SerializeField][Range(0, 2)] float fadeOutTime = 0.9f;

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
        eventSystemInput = GetComponent<InputSystemUIInputModule>();
    }

    public void Select(GameObject obj, bool alsoSetPrevious = false)
    {
        eventSystem.SetSelectedGameObject(obj);
    }

    public void ClearSelected()
    {
        UnSetMaterialOf(CurrentSelected);
        Select(null);
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
                UnSetMaterialOf(CurrentSelected, true);
            }
        }
    }

    void HandleNavigation()
    {
        if (!IsNavigating && PreviousSelected != null)
        {
            Select(PreviousSelected);
            IsNavigating = true;
        }

        if (IsNavigating)
        {
            timeLeft = maxTime;
            SetMaterialOf(CurrentSelected);
        }

        if (PreviousSelected != null && !PreviousSelected.Equals(CurrentSelected))
        {
            UnSetMaterialOf(PreviousSelected);
        }

        SetNavigationSpeed();
        PreviousSelected = CurrentSelected;
    }

    void SetMaterialOf(GameObject obj)
    {
        Graphic[] graphs = GetNavigationTarget(obj).graphs;

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

    void UnSetMaterialOf(GameObject obj, bool withTransition = false)
    {
        Graphic[] graphs = GetNavigationTarget(obj).graphs;

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

    void HandleOnUIAccept(InputAction.CallbackContext _)
    {
        if (CurrentSelected == null) return;

        if (!IsNavigating) 
        {
            IsNavigating = true;
            HandleNavigation();
            return;
        }
        GetNavigationTarget().Trigger();
    }

    void HandleOnUINavigate(InputAction.CallbackContext _)
    {
        if (IsNavigating)
        {
            timeLeft = maxTime;
        }
    }

    private void OnEnable()
    {
        InputManager.Ins.UI.Accept.performed += HandleOnUIAccept;
        InputManager.Ins.UI.Navigate.performed += HandleOnUINavigate;
    }

    private void OnDisable()
    {
        InputManager.Ins.UI.Accept.performed -= HandleOnUIAccept;
        InputManager.Ins.UI.Navigate.performed += HandleOnUINavigate;
    }

    void SetNavigationSpeed()
    {
        if (CurrentSelected == null) return;

        if (GetNavigationTarget().type == NavigationTarget.Type.Slider)
        {
            eventSystemInput.moveRepeatDelay = fastDelayRate.x;
            eventSystemInput.moveRepeatRate = fastDelayRate.y;
        }
        else
        {
            eventSystemInput.moveRepeatDelay = normalDelayRate.x;
            eventSystemInput.moveRepeatRate = normalDelayRate.y;
        }
    }

    NavigationTarget GetNavigationTarget(GameObject obj = null)
    {
        if (obj == null) obj = CurrentSelected;
        NavigationTarget target = obj.GetComponent<NavigationTarget>();

        if (target == null)
        {
            throw new Exception($"Coultn't found a NavigationTarget at GameObject {obj.name}");
        }
        return target;
    }
}
