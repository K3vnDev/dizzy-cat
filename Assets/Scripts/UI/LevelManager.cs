using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        SetColors();
    }

    void SetColors()
    {
        Color tilemapColor = GameObject.FindWithTag("Scenario").GetComponent<Tilemap>().color;
        Camera.main.backgroundColor = tilemapColor;

        GameObject[] bgFills = GameObject.FindGameObjectsWithTag("BlocksFill");
        foreach (GameObject fill in bgFills)
        {
            SpriteRenderer spriteRenderer = fill.GetComponent<SpriteRenderer>();
            spriteRenderer.color = tilemapColor;
        }
    }
}
