using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    [SerializeField] private bool isFinal;

    [SerializeField] private float sceneLoadDelay;
    [SerializeField] private GameObject selfParticles;

    private PlayerController playerController;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        playerController = GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) Trigger();
    }

    private void Trigger()
    {
        playerController.PickFish();
        GetComponent<BoxCollider2D>().enabled = false;
        spriteRenderer.enabled = false;
        selfParticles.SetActive(false);

        if (isFinal)
        {
            StartCoroutine(LoadNextSceneFromLastLevel());
        }
        else
        {
            int activeScene = SceneManager.GetActiveScene().buildIndex + 1;
            GameManager.Ins.currentLevel = activeScene;
            StartCoroutine(LoadNextScene(activeScene));
        }
    }

    private IEnumerator LoadNextScene(int scene)
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(scene);
    }

    private IEnumerator LoadNextSceneFromLastLevel()
    {
        EndManager endManager = GameObject.FindWithTag("End Manager")
            .GetComponent<EndManager>();
        endManager.TriggerEndManager();

        yield return new WaitForSeconds(endManager.nextSceneTime);

        GameManager.Ins.currentLevel = 1;
        StartCoroutine(endManager.LoadMainMenu());
    }
}
