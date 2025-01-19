using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LevitateController : MonoBehaviour
{
    [SerializeField] float range, period;

    void Start()
    {
        float moveY = range / 2;

        transform.DOLocalMoveY(moveY, 0);

        transform
            .DOLocalMoveY(-moveY, period / 4)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void Stop()
    {
        transform.DOKill();
    }
}
