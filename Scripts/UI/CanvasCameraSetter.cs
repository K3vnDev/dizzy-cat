using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
