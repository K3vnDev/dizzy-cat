using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuLevelIndicatorController : MonoBehaviour
{
    readonly float GAP = 0.02f, SPEED = 35f;

    [SerializeField] ButtonsController buttonController;
    RectTransform rectTransform;
    Vector2 initialPosition, extendedPosition;
    TextMeshProUGUI textMeshPro;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();

        initialPosition = transform.position;
        extendedPosition = new(transform.position.x, transform.position.y - GAP - rectTransform.rect.height);
        textMeshPro.text = $"Level {GameManager.Ins.currentLevel}";
    }

    void Update()
    {
        Vector2 towardsPosition = buttonController.pointerHovering ? extendedPosition : initialPosition;
        float deltaSpeed = Time.deltaTime * SPEED;

        transform.position = Vector2.MoveTowards(transform.position, towardsPosition, deltaSpeed);
    }
}
