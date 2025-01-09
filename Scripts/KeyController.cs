using System.Collections;
using UnityEngine;

public class KeyController : MonoBehaviour
{
    [Header ("Values")]
    [SerializeField] private float movRotTime;
    [SerializeField] private float floatingTime;
    [SerializeField] private float floatingRange;

    [Header ("Animation Curves")]
    [SerializeField] private AnimationCurve easeInOut;

    [Header("Particles")]
    private ParticleSystem keyParticles;


    private Transform lockTr;
    private LockController lockController;
    private PlayerController playerController;

    [Header ("SFX")]
    [SerializeField] private AudioClip getKeySound;
    [SerializeField] private AudioClip openLockSound;


    void Start()
    {
        lockTr = GameObject.FindWithTag("Lock").transform;
        lockController = GameObject.FindWithTag("Lock")
            .GetComponent<LockController>();
        keyParticles = GameObject.FindWithTag("Key Particles")
            .GetComponent<ParticleSystem>();
        playerController = GameObject.FindWithTag("Player")
            .GetComponent<PlayerController>();

        StartCoroutine(FloatingAnimation());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(MoveAndRotateToLock());
            keyParticles.Stop();
            SFXPlayerSingleton.Ins.PlaySound(getKeySound, .15f);
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    private IEnumerator MoveAndRotateToLock()
    {
        playerController.playerCanMove = false;

        Vector2 startPos = transform.position, targetPos = lockTr.position;
        Quaternion startRot = transform.localRotation, targetRot = Quaternion.Euler(
            0, 0, transform.localRotation.z - 90);
        
        float step = 0;
        while (step < movRotTime)
        {
            transform.position = Vector2.Lerp(
                startPos, targetPos, easeInOut.Evaluate(step/movRotTime));

            transform.localRotation = Quaternion.Lerp(
                startRot, targetRot, easeInOut.Evaluate(step / movRotTime));

            step += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        transform.localRotation = targetRot;

        SFXPlayerSingleton.Ins.PlaySound(openLockSound, .15f);
        lockController.triggered = true;
        playerController.playerCanMove = true;
    }

    private IEnumerator FloatingAnimation()
    {
        Vector2 upTarget = new Vector2(
            transform.localPosition.x, transform.localPosition.y + floatingRange);

        Vector2 downTarget = new Vector2(
            transform.localPosition.x, transform.localPosition.y - floatingRange);

        while (true)
        {
            float step = 0;
            Vector2 start = transform.localPosition;

            while (step < floatingTime)
            {
                transform.localPosition = Vector2.Lerp(start, upTarget, step / floatingTime);
                step += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = upTarget;

            step = 0;
            start = transform.localPosition;

            while (step < floatingTime)
            {
                transform.localPosition = Vector2.Lerp(start, downTarget, step / floatingTime);
                step += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = downTarget;
        }
    }
}
