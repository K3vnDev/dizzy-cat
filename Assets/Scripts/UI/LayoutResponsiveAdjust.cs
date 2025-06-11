using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutResponsiveAdjust : MonoBehaviour
{
    [Header ("Rect Transform")]
    [SerializeField] RectTransform rectTransform;
    [SerializeField] float topBottomMargins;

    [Header("Grid Layout Group")]
    [SerializeField] GridLayoutGroup gridLayout;
    [SerializeField] Vector2 gridLayoutGap;
    [SerializeField] Vector2 gridLayoutCellSize;


    void Start()
    {
        if (!SwapMenuManager.I.isOnBrokenLayout) return;

        if (rectTransform != null)
        {
            // Adjust top and bottom margins
            rectTransform.offsetMin = new(rectTransform.offsetMin.x, topBottomMargins);
            rectTransform.offsetMax = new(rectTransform.offsetMax.x, -topBottomMargins);
        }

        if (gridLayout != null)
        {
            gridLayout.cellSize = gridLayoutCellSize;
            gridLayout.spacing = gridLayoutGap;
        }
    }
}
