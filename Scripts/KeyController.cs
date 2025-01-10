using System.Collections;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    [SerializeField] float moveTime;
    [SerializeField] AnimationCurve easeInOut;

    ParticleSystem keyParticles;

    Animator lockAnimator, animator;
    PlayerController playerController;

    [Header("SFX")]
    [SerializeField] AudioClip getKeySound;
    [SerializeField] AudioClip openLockSound;

    bool alreadyTriggered = false;


    void Start()
    {
        lockAnimator = GameObject.FindWithTag("Lock").GetComponent<Animator>();
        keyParticles = GameObject.FindWithTag("Key Particles").GetComponent<ParticleSystem>();
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        animator = GetComponent<Animator>();
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
        }
    }

    void Trigger()
    {
        // Stop levitanting effect
        GetComponent<LevitateController>().StopAllCoroutines();
        playerController.playerCanMove = false;
        animator.SetTrigger("pick");

        StartCoroutine(MoveToLock());

        keyParticles.Stop();
        SFXPlayer.Ins.PlaySound(getKeySound, .15f);
        alreadyTriggered = true;
    }

    private IEnumerator MoveToLock()
    {
        Vector2 start = transform.position, target = lockAnimator.transform.position;
        float elapsed = 0;

        while (elapsed < moveTime)
        {
            transform.position = Vector2.Lerp(start, target, easeInOut.Evaluate(elapsed / moveTime));

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = target;

        SFXPlayer.Ins.PlaySound(openLockSound, .15f);

        lockAnimator.SetTrigger("unlock");
        playerController.playerCanMove = true;
    }
}