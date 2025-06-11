using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LensCircleController : MonoBehaviour
{
    Material material;
    [SerializeField][Range (0, 3)] float transitionInTime = 0.6f, transitionOutTime = 0.3f;
    readonly string RADIUS = "_Radius";

    private void Awake()
    {
        material = GetComponent<Image>().material;
    }

    public void SetRadius(float radius)
    {
        material.SetFloat(RADIUS, radius);
    }

    public float TransitionIn(float t)
    {
        Transitionate(1, 0, t, Ease.OutCubic);
        return t;
    }
    public float TransitionIn()
    {
        return TransitionIn(transitionInTime);
    }

    public float TransitionOut(float t)
    {
        Transitionate(0, 1, t, Ease.OutSine);
        return t;
    }
    public float TransitionOut()
    {
        return TransitionOut(transitionOutTime);
    }

    void Transitionate(float startRadius, float targetRadius, float time, Ease easing)
    {
        SetRadius(startRadius);
        material.DOKill();

        material
            .DOFloat(targetRadius, RADIUS, time)
            .SetUpdate(true)
            .SetEase(easing);
    }
}
