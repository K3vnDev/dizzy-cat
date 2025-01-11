using System.Collections;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    [SerializeField] bool isFinal;
    [SerializeField] float sceneLoadDelay;
    [SerializeField] GameObject selfParticles;

    PlayerController playerController;
    SpriteRenderer spriteRenderer;
    EndManager endManager;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerController = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();

        endManager = GameObject.FindWithTag("End Manager")
            .GetComponent<EndManager>();
    }

    public void Trigger()
    {
        playerController.HandlePickFish();
        GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer.enabled = false;
        selfParticles.SetActive(false);

        if (isFinal )
        {
            endManager.Trigger();
            return;
        }
       StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        TransitionManager.Ins.LoadScene(TMScene.NextLevel);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Trigger();
    }
}
