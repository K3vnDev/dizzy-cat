using UnityEngine;
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
            RotateDirection = rotateDirection;
        }
    }

    private void Awake()
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

    private void OnEnable() => controls?.Enable();
    private void OnDisable() => controls?.Disable();

    void HandleGameActions()
    {
        Vector2 parsedInput = Utils.ParseRawInput(controls.Game.Rotate.ReadValue<Vector2>());
        Game.SetRotateDirection(parsedInput.x);
    }

    void Update()
    {
        HandleGameActions();
    }

    public void UseActionMap(IMActionMap actionMap)
    {
        SelectedMap = actionMap;

        if (actionMap == IMActionMap.UI)
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
}
