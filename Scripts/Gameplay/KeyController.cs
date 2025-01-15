using UnityEngine;
using DG.Tweening;
using System;

public class KeyController : MonoBehaviour, ICollectable
{
    [SerializeField] float targetSpeed, minRotationTime;

    ParticleSystem keyParticles;
    Animator lockAnimator, animator;
    LevitateController levitateController;
    PlayerController playerController;

    bool alreadyCollected = false;

    void Start()
    {
        lockAnimator = GameObject.FindWithTag("Lock").GetComponent<Animator>();
        animator = GetComponentInChildren<Animator>();

        keyParticles = GetComponentInChildren<ParticleSystem>();
        levitateController = GetComponentInChildren<LevitateController>();
        playerController = Utils.GetPlayer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lock"))
        {
            animator.SetTrigger("enter");
            lockAnimator.SetTrigger("unlock");
            SFXPlayer.I.PlaySound(SFXPlayer.Sound.Lock, 0.15f);
        }
    }

    public void Collect()
    {
        if (!alreadyCollected)
        {
            alreadyCollected = true;
            playerController.playerCanMove = false;

            levitateController.Stop();
            animator.SetTrigger("pick");
            SFXPlayer.I.PlaySound(SFXPlayer.Sound.Key, 0.15f);
            keyParticles.Stop();

            MoveToLock();
        }
    }

    void MoveToLock()
    {
        Vector2 lockPosition = lockAnimator.transform.position;
        float distance = Vector2.Distance(lockPosition, transform.position);
        float animationTime = distance / targetSpeed;

        transform
            .DOMove(lockPosition, animationTime)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => playerController.playerCanMove = true);

        if (animationTime < minRotationTime) return;

        transform
            .DOLocalRotate(Vector3.forward * 360, animationTime, RotateMode.FastBeyond360)
            .SetEase(Ease.OutSine);
    }
}
