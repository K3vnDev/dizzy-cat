using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D rb;
    private ScenarioController scenarioController;
    private float initialGravity;

    public Collider2D circleCollider;

    public bool isFalling = false;
    private bool fishCollected = false;
    [SerializeField] private GameObject cantRotateSign;

    [Header ("Ground Checker")]
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private LayerMask groundLayer;
    private bool hasLanded = false;

    [Header ("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    public bool gameIsPaused = false;

    [Header ("Player Particles")]
    [SerializeField] private GameObject playerParticles;
    public bool playerCanMove = true;

    [Header("SFX")]
    [SerializeField] private AudioClip getFishSound;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip rotateSound;
    [SerializeField] private AudioClip catMeowSound;
    private float horizontalAxis;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scenarioController = GameObject.FindGameObjectWithTag("Scenario").GetComponent<ScenarioController>();

        initialGravity = rb.gravityScale;
        Cursor.visible = false;

        SetCharacterSprite();

        StartCoroutine(MeowSound());
    }

    void SetCharacterSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite characterSprite = GameManager.Ins.characterSprites[GameManager.Ins.selectedCharacter];

        spriteRenderer.sprite = characterSprite;
    }

    private void Update()
    {
        if (!gameIsPaused)
        {
            horizontalAxis = Input.GetAxisRaw("Horizontal");
        }

        HasLandedLogic();

        if (hasLanded)
        {
            rb.velocity = new Vector2 (0, rb.velocity.y);

            if (!scenarioController.rotating && playerCanMove && horizontalAxis != 0)
            {
                scenarioController.SetRotation(horizontalAxis);
                SFXPlayerSingleton.Instance.PlaySound(rotateSound, .2f);
            }
            else if (!playerCanMove && horizontalAxis != 0 
                && GameObject.FindWithTag("Cant Rotate") == null && !fishCollected)
            {
                Instantiate(cantRotateSign);
                SFXPlayerSingleton.Instance.PlaySound(errorSound, .1f);
            }
        }

        if (!gameIsPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            gameIsPaused = true;
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            Cursor.visible = true;
        }

        if (scenarioController.rotating)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = initialGravity;
        }
    }

    private void HasLandedLogic()
    {
        Vector2 boxPosition = new Vector2(
            transform.position.x, transform.position.y - transform.localScale.y/2);

        hasLanded = Physics2D.OverlapBox(boxPosition, boxSize, 6, groundLayer);
    }

    public void PickFish()
    {
        Vector2 particlesPos = new Vector2(
            transform.position.x, transform.position.y - .25f);

        Instantiate(playerParticles, particlesPos, Quaternion.Euler(Vector3.zero));

        SFXPlayerSingleton.Instance.PlaySound(getFishSound, 0.1f);
        playerCanMove = false;
        fishCollected = true;
    }

    private IEnumerator MeowSound()
    {
        yield return new WaitForSeconds(.25f);
        SFXPlayerSingleton.Instance.PlaySound(catMeowSound, .2f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(
            new Vector2(transform.position.x, transform.position.y 
                - transform.localScale.y / 2), boxSize);
    }
}
