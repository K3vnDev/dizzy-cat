using System.Collections;
using UnityEngine;

public class LockController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float unlockDelay;
    [SerializeField] private float disableCollisionDelay;
    private float timer = 0;
    public bool triggered = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (triggered)
        {
            if (timer < unlockDelay)
            {
                timer += Time.deltaTime;
            }
            else
            {
                animator.SetTrigger("unlock");

                GameObject.FindGameObjectWithTag("Key")
                    .GetComponent<Animator>().SetTrigger("enter");

                StartCoroutine(DisableCollision());
            }
        }
    }

    private IEnumerator DisableCollision()
    {
        yield return new WaitForSeconds(disableCollisionDelay);
        GetComponent<BoxCollider2D>().enabled = false;
    }
}
