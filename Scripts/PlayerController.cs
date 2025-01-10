using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    ScenarioController scenarioController;
    float initialGravity;

    public Collider2D circleCollider;

    public bool isFalling = false;
    bool fishCollected = false;
    [SerializeField] GameObject cantRotateSign;

    [Header ("Ground Checker")]
    [SerializeField] Vector2 boxSize;
    [SerializeField] LayerMask groundLayer;

    [Header ("Player Particles")]
    [SerializeField] GameObject playerParticles;
    public bool playerCanMove = true;

    [Header("SFX")]
    [SerializeField] AudioClip getFishSound;
    [SerializeField] AudioClip errorSound;
    [SerializeField] AudioClip rotateSound;
    [SerializeField] AudioClip catMeowSound;

    float horizontalAxis;

    readonly float FISH_PARTICLES_OFFSET = 0.25f;
    readonly float MEOW_SOUND_DELAY = 0.25f;
    readonly float INPUT_BUFFER_TIME = 0.13f;

    Queue<float> inputBuffer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        scenarioController = GameObject.FindGameObjectWithTag("Scenario").GetComponent<ScenarioController>();

        initialGravity = rb.gravityScale;

        SetCharacterSprite();

        Invoke(nameof(PlayMeowSound), MEOW_SOUND_DELAY);

        inputBuffer = new();
    }

    void PlayMeowSound() => SFXPlayerSingleton.Ins.PlaySound(catMeowSound, 0.2f);

    void SetCharacterSprite()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite characterSprite = GameManager.Ins.characterSprites[GameManager.Ins.selectedCharacter];

        spriteRenderer.sprite = characterSprite;
    }

    void Update()
    {
        if (!PauseMenuController.Ins.gameIsPaused)
        {
            horizontalAxis = InputManager.Ins.Game.RotateDirection;

            if (horizontalAxis != 0)
            {
                inputBuffer.Enqueue(horizontalAxis);
                Invoke(nameof(DequeueInputBuffer), INPUT_BUFFER_TIME);
            }
        }

        if (IsOnGround()) 
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
    }

    void HandleRotate()
    {
        float horizontalAxis = PeekInputBuffer();

        if (!scenarioController.rotating && playerCanMove && horizontalAxis != 0)
        {
            scenarioController.SetRotation(horizontalAxis);
            SFXPlayerSingleton.Ins.PlaySound(rotateSound, .2f);
            inputBuffer.Clear();
        }
        else if (!playerCanMove && this.horizontalAxis != 0
            && GameObject.FindWithTag("Cant Rotate") == null && !fishCollected)
        {
            Instantiate(cantRotateSign);
            SFXPlayerSingleton.Ins.PlaySound(errorSound, .1f);
        }
    }

    bool IsOnGround()
    {
        Vector2 boxPosition = new (transform.position.x, transform.position.y - transform.localScale.y / 2);
        return Physics2D.OverlapBox(boxPosition, boxSize, 6, groundLayer);
    }

    public void PickFish()
    {
        Vector2 particlesPos = new (transform.position.x, transform.position.y - FISH_PARTICLES_OFFSET);

        Instantiate(playerParticles, particlesPos, Quaternion.Euler(Vector3.zero));

        SFXPlayerSingleton.Ins.PlaySound(getFishSound, 0.1f);

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 position = new(transform.position.x, transform.position.y - transform.localScale.y / 2);
        Gizmos.DrawWireCube(position, boxSize);
    }
}
