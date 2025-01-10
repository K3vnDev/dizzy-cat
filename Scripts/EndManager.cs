using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class EndManager : MonoBehaviour
{
    public float nextSceneTime, stopMusicDelay, stopMusicDuration;
    [SerializeField] AnimationCurve stopMusicCurve, textPopUpCurve, transitionCurve;

    [SerializeField] GameObject[] uiTexts;
    [SerializeField] float textPopUpTime, textPopUpDelay, textStartFontSize;

    [SerializeField] GameObject uiBackground;
    [SerializeField] float bgAppearTime, bgAppearDelay, bgAplha;

    [SerializeField] AudioClip winSound;
    [SerializeField] GameObject blackTransition;
    [SerializeField] float transitionTime;

    public void TriggerEndManager()
    {
        StartCoroutine(StopMusic(stopMusicDuration));
        foreach (GameObject uiText in uiTexts) StartCoroutine(PopUpText(uiText));
        StartCoroutine(AppearBackground());
    }

    private IEnumerator PopUpText(GameObject text)
    {
        TextMeshProUGUI tmPro = text.GetComponent<TextMeshProUGUI>();

        float startFontSize = tmPro.fontSize * textStartFontSize,
            targetFontSize = tmPro.fontSize;

        Color startColor = new Color(255, 255, 255, 0), 
            targetColor = Color.white;

        float step = 0;

        yield return new WaitForSeconds(textPopUpDelay);

        while (step < textPopUpTime)
        {
            tmPro.fontSize = Mathf.Lerp(startFontSize, targetFontSize, 
                textPopUpCurve.Evaluate(step/textPopUpTime));

            tmPro.color = Color.Lerp(startColor, targetColor,
                textPopUpCurve.Evaluate(step / textPopUpTime));

            step += Time.deltaTime;
            yield return null;
        }
        tmPro.fontSize = targetFontSize;
        tmPro.color = targetColor;

    }

    private IEnumerator AppearBackground()
    {
        Image bgImage = 
            uiBackground.GetComponent<Image>();

        Color startColor = new Color(0, 0, 0, 0),
            targetColor = new Color(0, 0, 0, bgAplha);

        float step = 0;

        yield return new WaitForSeconds(bgAppearDelay);
        SFXPlayer.Ins.PlaySound(winSound, 0);

        while (step < bgAppearTime)
        {
            bgImage.color = Color.Lerp(startColor, targetColor,
                textPopUpCurve.Evaluate(step / bgAppearTime));

            step += Time.deltaTime;
            yield return null;
        }
        bgImage.color = targetColor;
    }

    private IEnumerator StopMusic(float time)
    {
        AudioSource musicSource =
            MusicPlayerSingleton.Ins.audioSource;

        float start = musicSource.volume, target = 0, step = 0;
        yield return new WaitForSeconds(stopMusicDelay);

        while (step < time)
        {
            musicSource.volume = Mathf.Lerp(start, target,
                stopMusicCurve.Evaluate(step / time));

            step += Time.deltaTime;
            yield return null;
        }
        musicSource.volume = 0;
        musicSource.Stop();
    }

    public IEnumerator LoadMainMenu()
    {
        Image bgImage =
            blackTransition.GetComponent<Image>();

        Color startColor = new Color(0, 0, 0, 0),
            targetColor = new Color(0, 0, 0, 1);
        float step = 0;

        while (step < transitionTime)
        {
            bgImage.color = Color.Lerp(startColor, targetColor,
                transitionCurve.Evaluate(step / transitionTime));
            step += Time.deltaTime;
            yield return null;
        }
        bgImage.color = targetColor;

        AudioSource musicSource =
            MusicPlayerSingleton.Ins.audioSource;
        musicSource.volume = .5f;
        musicSource.Play();

        SceneManager.LoadScene(0);
    }
    //A short game with a very dizzy cat, select your character and start rotating.
}
