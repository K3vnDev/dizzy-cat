using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CantRotateAnimation : MonoBehaviour
{
    [SerializeField] float moveOffset, moveTime, fadeTime;
    [SerializeField] [Range(1, 4)] int shakesCount;
    [Space]
    [SerializeField] AudioClip soundEffect;

    Image image;

    public bool IsActive { get; private set; }

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public void Trigger()
    {
        if (IsActive) return;
        StartCoroutine(TriggerAnimation());
    }

    IEnumerator TriggerAnimation()
    {
        IsActive = true;

        RectTransform imageRect = image.rectTransform;
        SFXPlayer.Ins.PlaySound(soundEffect, 0.15f);

        image.DOFade(1, fadeTime).SetAutoKill();
        int loopsCount = shakesCount * 2;

        imageRect
            .DOMoveX(imageRect.position.x - moveOffset, moveTime)
            .SetEase(Ease.InOutSine)
            .SetLoops(loopsCount, LoopType.Yoyo);

        yield return new WaitForSeconds(moveTime * loopsCount);

        image.DOFade(0, fadeTime);
        IsActive = false;
    }
}
