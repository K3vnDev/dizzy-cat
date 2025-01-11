using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header ("Ground Checker")]
    [SerializeField] Vector2 boxSize;
    [SerializeField] LayerMask groundLayer;

    public bool isGrounded = false;
    public bool isFalling = false;

    [Header ("Player Particles")]
    [SerializeField] GameObject playerParticles;
    public bool playerCanMove = true;

    [Header("SFX")]
    [SerializeField] AudioClip getFishSound;
    [SerializeField] AudioClip errorSound;
    [SerializeField] AudioClip rotateSound;
    [SerializeField] AudioClip catMeowSound;

    [Space]
    [SerializeField] GameObject cantRotateSign;

    [HideInInspector] public Rigidbody2D rb;
    float initialGravity;

    [HideInInspector] public Collider2D circleCollider;
    ScenarioController scenarioController;
    PlayerEventsManager playerEvents;

    float horizontalAxis;
    bool fishCollected = false;
    Queue<float> inputBuffer;
    int ignoreCatSoundsLeft = 0;

    readonly float FISH_PARTICLES_OFFSET = 0.25f;
    readonly float INPUT_BUFFER_TIME = 0.13f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scenarioController = GameObject.FindGameObjectWithTag("Scenario").GetComponent<ScenarioController>();
        playerEvents = GetComponent<PlayerEventsManager>();

        initialGravity = rb.gravityScale;
        inputBuffer = new();

        playerEvents.OnHitGround += HandleCatSound;
    }

    void Update()
    {
        GroundCheckingLogic();

        if (!PauseMenuController.Ins.gameIsPaused)
        {
            horizontalAxis = InputManager.Ins.Game.RotateDirection;

            if (horizontalAxis != 0)
            {
                inputBuffer.Enqueue(horizontalAxis);
                Invoke(nameof(DequeueInputBuffer), INPUT_BUFFER_TIME);
            }
        }

        if (isGrounded) 
        {
            rb.velocity = new Vector2 (0, rb.velocity.y);
            HandleRotate();
        }

        if (scenarioController.rotating)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        else rb.gravityScale = initialGravity;

        isFalling = !scenarioController.rotating && !isGrounded;
    }

    void HandleRotate()
    {
        float horizontalAxis = PeekInputBuffer();

        if (!scenarioController.rotating && playerCanMove && horizontalAxis != 0)
        {
            scenarioController.SetRotation(horizontalAxis);
            PlaySound(rotateSound, .2f);
            inputBuffer.Clear();
        }
        else if (!playerCanMove && this.horizontalAxis != 0
            && GameObject.FindWithTag("Cant Rotate") == null && !fishCollected)
        {
            Instantiate(cantRotateSign);
            PlaySound(errorSound, .1f);
        }
    }

    void GroundCheckingLogic()
    {
        Vector2 boxPosition = new (transform.position.x, transform.position.y - transform.localScale.y / 2);
        isGrounded = Physics2D.OverlapBox(boxPosition, boxSize, 6, groundLayer);
    }

    public void PickFish()
    {
        Vector2 particlesPos = new (transform.position.x, transform.position.y - FISH_PARTICLES_OFFSET);

        Instantiate(playerParticles, particlesPos, Quaternion.Euler(Vector3.zero));

        PlaySound(getFishSound, 0.1f);

        playerCanMove = false;
        fishCollected = true;
    }

    void DequeueInputBuffer()
    {
        if (inputBuffer.Count > 0) inputBuffer.Dequeue();
    }

    float PeekInputBuffer()
    {
        try { return inputBuffer.Peek(); }
        catch { return 0; }
    }

    void PlaySound(AudioClip sound, float pitchRange)
    {
        SFXPlayer.Ins.PlaySound(sound, pitchRange);
    }

    void HandleCatSound()
    {
        if (ignoreCatSoundsLeft <= 0 && !fishCollected)
        {
            PlaySound(catMeowSound, 0.33f);
            ignoreCatSoundsLeft = Random.Range(4, 10);
            return;
        }
        ignoreCatSoundsLeft--;
    }

    void OnDisable()
    {
        playerEvents.OnHitGround -= HandleCatSound;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 position = new(transform.position.x, transform.position.y - transform.localScale.y / 2);
        Gizmos.DrawWireCube(position, boxSize);
    }
}
