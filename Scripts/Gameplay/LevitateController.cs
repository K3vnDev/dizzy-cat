using System.Collections;
using UnityEngine;
using DG.Tweening;

public class LevitateController : MonoBehaviour
{
    [SerializeField] float range, period;

    void Start()
    {
        transform.DOLocalMoveY(range / 2, 0);

        transform
            .DOLocalMoveY(-range, period / 4)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void Stop()
    {
        transform.DOKill();
    }
}
