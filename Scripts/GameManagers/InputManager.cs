using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public enum IMActionMap { UI, Game }

public class InputManager : MonoBehaviour
{
    public static InputManager I;
    Controls controls;

    public GameActions Game;
    public Controls.UIActions UI;

    public IMActionMap SelectedMap { get; private set; }

    public class GameActions
    {
        public float RotateDirection { get; private set; }
        public InputAction Pause { get; private set; }

        public GameActions(Controls.GameActions gameActions)
        {
            Pause = gameActions.Pause;
            Pause.Enable();
        }

        public void SetRotateDirection(float rotateDirection)
        {
            if (new float[] { -1, 0, 1 }.Contains(rotateDirection))
            {
                RotateDirection = rotateDirection;
                return;
            }
            Debug.LogWarning($"GameActions.SetRotateDirection was given a " +
                $"different value than -1, 0 or 1. Value recieved: {rotateDirection}");
        }
    }

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        I = this;

        controls = new();
        UI = controls.UI;

        Game = new (controls.Game);
    }

    public void UseActionMap(IMActionMap actionMap)
    {
        SelectedMap = actionMap;

        if (SelectedMap == IMActionMap.UI)
        {
            controls.UI.Enable();
            controls.Game.Disable();
        }
        else
        {
            controls.Game.Enable();
            controls.UI.Disable();
        }
    }

    void HandleGameDirection(InputAction.CallbackContext context)
    {
        Vector2 parsedInput = Utils.ParseRawInput(context.ReadValue<Vector2>());
        Game.SetRotateDirection(parsedInput.x);
    }

    void HandleGameTouchPressed(InputAction.CallbackContext _)
    {
        Vector2 rawPos = controls.Game.TouchPosition.ReadValue<Vector2>();

        static bool Valid (float val, float max) => val >= 0 && val <= max;
        if (!Valid(rawPos.x, Screen.width) || !Valid(rawPos.y, Screen.height)) return;

        if (rawPos.x > 0)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = rawPos };

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (RaycastResult result in results)
            {
                GameObject go = result.gameObject;
                if (go.GetComponent<ButtonsController>()) return;
            }
        }

        float deadZoneFactor = 0.25f;
        float x = rawPos.x - (Screen.width / 2);
        float threshold = Screen.width * (deadZoneFactor / 2);

        if (Mathf.Abs(x) > threshold) 
            Game.SetRotateDirection(Mathf.Sign(x));
    }

    private void HandleGameTouchCanceled(InputAction.CallbackContext _)
    {
        Game.SetRotateDirection(0f);
    }

    void OnEnable()
    {
        controls.Enable();

        controls.Game.Direction.performed += HandleGameDirection;

        controls.Game.TouchPress.started += HandleGameTouchPressed;
        controls.Game.TouchPress.canceled += HandleGameTouchCanceled;
    }

    void OnDisable()
    {
        controls.Disable();

        controls.Game.Direction.performed -= HandleGameDirection;

        controls.Game.TouchPress.started -= HandleGameTouchPressed;
        controls.Game.TouchPress.canceled -= HandleGameTouchCanceled;
    }
}
