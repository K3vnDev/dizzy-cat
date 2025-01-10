using System;
using UnityEngine;
using DG.Tweening;

public class PlayerAnimationsController : MonoBehaviour
{
    [SerializeField] Transform visualTransform;
    [SerializeField] [Range(1, 2)] float maxScale;

    [Header ("Animation Values")]
    [SerializeField] float fallTime;
    [SerializeField] float groundTime;

    SpriteRenderer spriteRenderer;
    PlayerEventsManager playerEvents;


    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerEvents = GetComponent<PlayerEventsManager>();

        SetCharacterSprite();

        playerEvents.OnFall += PlayFallingAnimation;
        playerEvents.OnHitGround += PlayHitGroundAnimation;
    }

    void SetCharacterSprite()
    {
        int index = GameManager.Ins.selectedCharacter;
        Sprite characterSprite = GameManager.Ins.characterSprites[index];

        spriteRenderer.sprite = characterSprite;
    }

    void PlayFallingAnimation() => HandleTween(() => 
        visualTransform.DOScale(GetAxisScale(), fallTime).SetEase(Ease.InOutSine));

    void PlayHitGroundAnimation() => HandleTween(() => 
        visualTransform.DOScale(Vector3.one, groundTime).SetEase(Ease.OutCubic));


    void HandleTween(Action tween)
    {
        visualTransform.DOKill();
        tween();
    }

    Vector3 GetAxisScale()
    {
        float zRotation = transform.rotation.eulerAngles.z;
        int rotationValue = Mathf.RoundToInt(zRotation / 90);
        bool isOnYAxis = rotationValue % 2 == 0;

        Vector3 newScale = visualTransform.localScale;
        
        if (isOnYAxis) newScale.y = maxScale;
        else newScale.x = maxScale;

        return newScale;
    }

    void OnDisable()
    {
        DOTween.Kill(visualTransform);

        playerEvents.OnFall -= PlayFallingAnimation;
        playerEvents.OnHitGround -= PlayHitGroundAnimation;
    }
}
