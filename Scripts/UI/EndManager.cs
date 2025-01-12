using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{
    [SerializeField] float sceneWaitTime = 2f, initialWaitTime = 0.3f;
    [SerializeField] float fadeOutMusicTime = 1f;

    [Header("Text Values")]
    [SerializeField][Range(1, 2)] float textMaxScale = 1.1f;
    [SerializeField] float textAnimTime = 0.5f;
    [SerializeField] float textTimeBetween = 0.3f;

    [Header("Background Values")]
    [SerializeField][Range (0, 1)] float bgAlpha = 0.25f;
    [SerializeField] float bgAnimTime;

    [Header ("Texts")]
    [SerializeField] GameObject uiTextPrefab;
    [SerializeField] string[] texts;

    readonly List<TextMeshProUGUI> uiTexts = new();
    Image bgImage;

    [SerializeField] AudioClip winSound;
    PauseMenuController pauseMenu;

    private void Awake()
    {
        bgImage = GetComponent<Image>();
    }

    private void Start()
    {
        pauseMenu = GameObject.FindWithTag("Pause Menu").GetComponent<PauseMenuController>();
    }

    public void Trigger()
    {
        GameManager.Ins.currentLevel = 1;
        pauseMenu.gameCanBePaused = false;

        InitializeTexts();
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        yield return new WaitForSeconds(initialWaitTime);

        SFXPlayer.Ins.PlaySound(winSound);
        bgImage.DOFade(bgAlpha, bgAnimTime);

        MusicPlayer.Ins.audioSource.DOFade(0, fadeOutMusicTime);

        foreach (TextMeshProUGUI textTMPro in uiTexts)
        {
            textTMPro.DOFade(1, textAnimTime).SetEase(Ease.InSine);
            textTMPro.transform.DOScale(textMaxScale, textAnimTime).SetEase(Ease.OutBack);

            yield return new WaitForSeconds(textTimeBetween);
        }

        yield return new WaitForSeconds(sceneWaitTime);

        TransitionManager.Ins.LoadScene(TMScene.MainMenu, TMTransition.LensCircle);
    }

    void InitializeTexts()
    {
        foreach (string text in texts)
        {
            GameObject textObject = Instantiate(uiTextPrefab, transform);
            TextMeshProUGUI textTMPro = textObject.GetComponent<TextMeshProUGUI>();

            textTMPro.DOFade(0, 0);
            textTMPro.text = text;

            uiTexts.Add(textTMPro);
        }
    }
}

