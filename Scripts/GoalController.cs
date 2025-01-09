using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject[] points;

    [SerializeField] private bool isFinal;
    private bool goingUp = true;

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

    private void Update()
    {
        MovementLoop();
    }

    private void MovementLoop()
    {
        Vector3 pos = transform.localPosition;

        if (goingUp)
        {
            transform.localPosition = Vector2.MoveTowards(
                pos, points[0].transform.localPosition, speed * Time.deltaTime);

            goingUp = !(pos == points[0].transform.localPosition);
        }
        else
        {
            transform.localPosition = Vector2.MoveTowards(
                pos, points[1].transform.localPosition, speed * Time.deltaTime);

            goingUp = (pos == points[1].transform.localPosition);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PickedByPlayer();
        }
    }

    private void PickedByPlayer()
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
