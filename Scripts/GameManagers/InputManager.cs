using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Ins;
    Controls controls;

    public GameActions Game = new (0f);
    public Controls.UIActions UI;

    public class GameActions
    {
        public float RotateDirection { get; }

        public GameActions(float rotateDirection)
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
    }

    private void OnEnable() => controls?.Enable();
    private void OnDisable() => controls?.Disable();

    void HandleGameActions()
    {
        float rotateDirection = controls.Game.Rotate.ReadValue<Vector2>().x;
        Game = new GameActions (rotateDirection);
    }

    void Update()
    {
        HandleGameActions();
    }
}
