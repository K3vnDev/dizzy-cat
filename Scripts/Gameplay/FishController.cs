using System.Collections;
using UnityEngine;

public class FishController : MonoBehaviour, ICollectable
{
    [SerializeField] bool isFinal;
    [SerializeField] float sceneLoadDelay = 0.9f;

    SpriteRenderer spriteRenderer;
    EndManager endManager;
    PlayerController playerController;

    bool alreadyCollected = false;

    private void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

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

        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Eating, 0.1f);

        spriteRenderer.enabled = false;
        alreadyCollected = true;

        StopParticles();

        if (isFinal )
        {
            endManager.Trigger();
            return;
        }

        StartCoroutine(LoadNextScene());
    }

    void StopParticles()
    {
        ParticleSystem[] particles = transform.parent
            .GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particle in particles)
            particle.Stop();
    }

    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        TransitionManager.I.LoadScene(TMScene.NextLevel);
    }
}
