using System.Collections;
using UnityEngine;
using DG.Tweening;
using System;

public class KeyController : MonoBehaviour
{
    [SerializeField] float targetSpeed, minRotationTime;
    [SerializeField][Range (0, 1)] float rotationTimeFactor;

    ParticleSystem keyParticles;
    Animator lockAnimator, animator;

    PlayerController playerController;
    LevitateController levitateController;

    [Header("SFX")]
    [SerializeField] AudioClip getKeySound;
    [SerializeField] AudioClip openLockSound;

    bool alreadyTriggered = false;

    void Start()
    {
        lockAnimator = GameObject.FindWithTag("Lock").GetComponent<Animator>();
        animator = GetComponent<Animator>();

        keyParticles = transform.parent.GetComponentInChildren<ParticleSystem>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        levitateController = GetComponent<LevitateController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !alreadyTriggered)
        {
            Trigger();
        }
        else if (other.CompareTag("Lock"))
        {
            animator.SetTrigger("enter");
            lockAnimator.SetTrigger("unlock");
            SFXPlayer.Ins.PlaySound(openLockSound, .15f);
        }
    }

    void Trigger()
    {
        levitateController.Stop();
        playerController.playerCanMove = false;

        animator.SetTrigger("pick");
        keyParticles.Stop();
        SFXPlayer.Ins.PlaySound(getKeySound, .15f);
        alreadyTriggered = true;

        HandleMoveToLock();
    }

    void HandleMoveToLock()
    {
        Vector2 lockPosition = lockAnimator.transform.position;
        float distance = Vector2.Distance(lockPosition, transform.position);
        float moveTime = distance / targetSpeed;
        float rotationTime = moveTime * rotationTimeFactor;

        transform
            .DOMove(lockPosition, moveTime)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => playerController.playerCanMove = true);

        if (rotationTime < minRotationTime) return;

        transform.parent
            .DORotate(Vector3.forward * 360, rotationTime, RotateMode.FastBeyond360)
            .SetEase(Ease.OutSine);
    }
}