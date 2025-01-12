using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LensCircleController : MonoBehaviour
{
    Material material;
    [SerializeField][Range (0, 3)] float transitionInTime, transitionOutTime;
    readonly string RADIUS = "_Radius";

    private void Awake()
    {
        material = GetComponent<Image>().material;
    }

    public void SetRadius(float radius)
    {
        material.SetFloat(RADIUS, radius);
    }

    public float TransitionIn()
    {
        Transitionate(1, 0, transitionInTime, Ease.OutCubic);
        return transitionInTime;
    }
    public float TransitionOut()
    {
        Transitionate(0, 1, transitionOutTime, Ease.OutSine);
        return transitionOutTime;
    }

    void Transitionate(float startRadius, float targetRadius, float time, Ease easing)
    {
        SetRadius(startRadius);
        material.DOKill();

        material.DOFloat(targetRadius, RADIUS, time).SetEase(easing);
    }
}
