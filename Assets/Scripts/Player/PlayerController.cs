using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Ground Checker")]
    [SerializeField] Vector2 groundCheckerSize = new(0.2f, 0.03f);
    [SerializeField] float groundCheckerOffsetY = -0.035f;
    [SerializeField] LayerMask groundLayer;

    public bool isGrounded = false;
    public bool isFalling = false;

    [Header ("Player Particles")]
    [SerializeField] GameObject playerParticles;
    public bool playerCanMove = true;

    [Space]

    [HideInInspector] public Rigidbody2D rb;
    float initialGravity;

    [Header ("Colliders")]
    [SerializeField] Collider2D defaultCollider;
    [SerializeField] Collider2D fallingCollider;
    float fallingColliderOffset;

    ScenarioController scenarioController;
    PlayerEventsManager playerEvents;
    CantRotateAnimation cantRotateAnimation;
    PauseMenuController pauseMenuController;

    float horizontalAxis;
    Queue<float> inputBuffer;
    int ignoreCatSoundsLeft = 0;
    public bool levelCompleted = false;

    readonly float FISH_PARTICLES_OFFSET = 0.25f;
    readonly float INPUT_BUFFER_TIME = 0.13f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerEvents = GetComponent<PlayerEventsManager>();
        defaultCollider = GetComponent<CircleCollider2D>();

        initialGravity = rb.gravityScale;
        inputBuffer = new();

        InputManager.I.UseActionMap(IMActionMap.Game);
    }

    void Start()
    {
        scenarioController = GameObject.FindWithTag("Scenario").GetComponent<ScenarioController>();
        cantRotateAnimation = GameObject.FindWithTag("CantRotateSign").GetComponent<CantRotateAnimation>();
        pauseMenuController = GameObject.FindWithTag("Pause Menu").GetComponent<PauseMenuController>();

        playerEvents.OnHitGround += HandleCatSound;
        fallingColliderOffset = -fallingCollider.offset.y;
    }

    void Update()
    {
        GroundCheckingLogic();

        if (!pauseMenuController.GameIsPaused)
        {
            horizontalAxis = InputManager.I.Game.RotateDirection;

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

        if (scenarioController.IsRotating)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
        }
        else rb.gravityScale = initialGravity;

        isFalling = !scenarioController.IsRotating && !isGrounded;

        CircleCollidersLogic();
    }

    void HandleRotate()
    {
        float newHorizontalAxis = PeekInputBuffer();

        if (!scenarioController.IsRotating && playerCanMove && newHorizontalAxis != 0)
        {
            scenarioController.Rotate(newHorizontalAxis);
            inputBuffer.Clear();
        }
        else if (!playerCanMove && horizontalAxis != 0 && !levelCompleted)
        {
            cantRotateAnimation.Trigger();
        }
    }

    Vector2 GetGroundCheckerBoxPosition()
    {
        return new(transform.position.x, transform.position.y - (transform.localScale.y / 2) + groundCheckerOffsetY);
    }

    void GroundCheckingLogic()
    {
        isGrounded = Physics2D.OverlapBox(GetGroundCheckerBoxPosition(), groundCheckerSize, 6, groundLayer);
    }

    public void PlayParticles()
    {
        Vector2 particlesPos = new (transform.position.x, transform.position.y - FISH_PARTICLES_OFFSET);
        Instantiate(playerParticles, particlesPos, Quaternion.Euler(Vector3.zero));
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

    void HandleCatSound()
    {
        if (isGrounded && !scenarioController.IsRotating)
        {
            if (ignoreCatSoundsLeft <= 0 && playerCanMove && !levelCompleted)
            {
                SFXPlayer.I.PlaySound(SFXPlayer.Sound.Meow, 0.25f);
                ignoreCatSoundsLeft = Random.Range(4, 10);
                return;
            }
            ignoreCatSoundsLeft--;
        }
    }

    void CircleCollidersLogic()
    {
        defaultCollider.enabled = !isFalling;
        fallingCollider.enabled = isFalling;

        if (isFalling)
        {
            float degAngle = scenarioController.transform.localRotation.eulerAngles.z;
            degAngle = Mathf.Repeat(degAngle, 360f);

            float radAngle = degAngle * Mathf.Deg2Rad;
            var (x, y) = (Mathf.Round(Mathf.Cos(radAngle)), Mathf.Round(Mathf.Sin(radAngle)));

            Vector2 direction = new(-y, -x);
            fallingCollider.offset = direction * fallingColliderOffset;
        }
    }

    void OnDisable()
    {
        playerEvents.OnHitGround -= HandleCatSound;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(GetGroundCheckerBoxPosition(), groundCheckerSize);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ICollectable collectable = collision.GetComponent<ICollectable>();
        collectable?.Collect();
    }
}
