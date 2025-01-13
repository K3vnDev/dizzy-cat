using System.Collections;
using UnityEngine;

public class GoalController : MonoBehaviour, ICollectable
{
    [SerializeField] bool isFinal;
    [SerializeField] float sceneLoadDelay;
    [SerializeField] AudioClip soundEffect;

    SpriteRenderer spriteRenderer;
    EndManager endManager;
    PlayerController playerController;

    bool alreadyCollected = false;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        endManager = GameObject.FindWithTag("End Manager")
            .GetComponent<EndManager>();

        playerController = Utils.GetPlayer();
    }

    public void Collect()
    {
        if (alreadyCollected) return;

        playerController.PlayParticles();
        playerController.playerCanMove = false;
        playerController.levelCompleted = true;

        SFXPlayer.I.PlaySound(soundEffect, 0.1f);

        spriteRenderer.enabled = false;
        alreadyCollected = true;

        ParticleSystem particles = transform.parent.GetComponentInChildren<ParticleSystem>();
        particles.Stop();


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
        TransitionManager.I.LoadScene(TMScene.NextLevel);
    }
}
