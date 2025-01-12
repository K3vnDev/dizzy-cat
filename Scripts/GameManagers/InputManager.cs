using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Ins;
    Controls controls;

    public GameActions Game;
    public Controls.UIActions UI;

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
        if (Ins != null && Ins != this)
        {
            Destroy(this);
            return;
        }
        Ins = this;

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
}
