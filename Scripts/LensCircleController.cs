using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LensCircleController : MonoBehaviour
{
    Image image;
    [SerializeField][Range (0, 3)] float transitionInTime, transitionOutTime;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetRadius(float radius)
    {
        image.material.SetFloat("_Radius", radius);
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
        DOTween.To(
            () => startRadius,
            val => SetRadius(val),
            targetRadius,
            time
        ).SetEase(easing);
    }
}
