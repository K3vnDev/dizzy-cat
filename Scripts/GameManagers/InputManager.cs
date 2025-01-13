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
        }

        public void SetRotateDirection(float rotateDirection)
        {
            if (rotateDirection == 0) RotateDirection = rotateDirection;
            else RotateDirection = Mathf.Sign(rotateDirection);
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
        float rotateDirection = controls.Game.Rotate.ReadValue<Vector2>().x;
        Game.SetRotateDirection(rotateDirection);
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
