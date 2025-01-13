using UnityEngine;
using DG.Tweening;
using System;

public class KeyController : MonoBehaviour, ICollectable
{
    [SerializeField] float targetSpeed, minRotationTime;
    [SerializeField][Range(0, 1)] float rotationTimeFactor;

    ParticleSystem keyParticles;
    Animator lockAnimator, animator;
    LevitateController levitateController;
    PlayerController playerController;

    [Header("SFX")]
    [SerializeField] AudioClip getKeySound;
    [SerializeField] AudioClip openLockSound;

    bool alreadyCollected = false;

    void Start()
    {
        lockAnimator = GameObject.FindWithTag("Lock").GetComponent<Animator>();
        animator = GetComponent<Animator>();

        keyParticles = transform.parent.GetComponentInChildren<ParticleSystem>();
        levitateController = GetComponent<LevitateController>();
        playerController = Utils.GetPlayer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Lock"))
        {
            animator.SetTrigger("enter");
            lockAnimator.SetTrigger("unlock");

            SFXPlayer.I.PlaySound(openLockSound, .15f);
        }
    }

    public void Collect()
    {
        if (alreadyCollected) return;

        levitateController.Stop();
        playerController.playerCanMove = false;

        animator.SetTrigger("pick");
        keyParticles.Stop();
        SFXPlayer.I.PlaySound(getKeySound, .15f);
        alreadyCollected = true;

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
