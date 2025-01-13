using UnityEngine;

public class CanvasCameraSetter : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
