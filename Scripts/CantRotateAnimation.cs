using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantRotateAnimation : MonoBehaviour
{
    [SerializeField] private float xMod, eachShakeTime, fadeTime;
    [SerializeField] private int numberOfShakes;
    [SerializeField] private AnimationCurve curve;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(255, 255, 255, 0);
        transform.position = new Vector2(7.9f, -3.75f);
        StartCoroutine(SetAnimation());
    }

    private IEnumerator SetAnimation()
    {
        Color startCol = spriteRenderer.color;
        Color endCol = new Color(255, 255, 255, 1);
        float currentStep = 0;

        while (currentStep < fadeTime)
        {
            spriteRenderer.color = Color.Lerp(
                startCol, endCol, curve.Evaluate(currentStep/fadeTime));

            currentStep += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = endCol;

        float currentShake = 1;
        bool onRight = false;

        while (currentShake <= numberOfShakes)
        {
            if (currentShake == 1)
            {
                Vector2 startPos = transform.position;
                Vector2 endPos = new Vector2(startPos.x + (xMod / 2), startPos.y);
                currentStep = 0;

                while(currentStep < eachShakeTime/2)
                {
                    transform.position = Vector2.Lerp(
                        startPos, endPos, curve.Evaluate(currentStep/(eachShakeTime/2)));

                    currentStep += Time.deltaTime;
                    yield return null;
                }
                transform.position = endPos;
                onRight = true;
            }
            else if (currentShake == numberOfShakes)
            {
                Vector2 startPos = transform.position;
                float xValue = onRight ? startPos.x - (xMod/2) : startPos.x + (xMod/2);
                Vector2 endPos = new Vector2(xValue, startPos.y);

                currentStep = 0;
                while (currentStep < eachShakeTime / 2)
                {
                    transform.position = Vector2.Lerp(
                        startPos, endPos, curve.Evaluate(currentStep / (eachShakeTime / 2)));

                    currentStep += Time.deltaTime;
                    yield return null;
                }
                transform.position = endPos;
            }
            else
            {
                Vector2 startPos = transform.position;
                float xValue = onRight ? startPos.x - xMod : startPos.x + xMod;
                Vector2 endPos = new Vector2(xValue, startPos.y);

                currentStep = 0;
                while (currentStep < eachShakeTime)
                {
                    transform.position = Vector2.Lerp(
                        startPos, endPos, curve.Evaluate(currentStep / eachShakeTime));

                    currentStep += Time.deltaTime;
                    yield return null;
                }
                transform.position = endPos;
                onRight = !onRight;
            }
            currentShake++;
        }

        startCol = spriteRenderer.color;
        endCol = new Color(255, 255, 255, 0);
        currentStep = 0;

        while (currentStep < fadeTime)
        {
            spriteRenderer.color = Color.Lerp(
                startCol, endCol, curve.Evaluate(currentStep / fadeTime));

            currentStep += Time.deltaTime;
            yield return null;
        }
        spriteRenderer.color = endCol;
        Destroy(gameObject);
    }
}
