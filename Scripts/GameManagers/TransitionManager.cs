using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using System;

public enum TMScene { MainMenu, CurrentLevel, NextLevel }
public enum TMTransition { LensCircle, None }

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Ins;
    public TMTransition CurrentTransition { get; private set; } = TMTransition.None;

    Camera mainCamera;
    LensCircleController lensCircle;

    bool hasJustChangedScene = false;

    private void Awake()
    {
        if (Ins != null && Ins != this)
        {
            Destroy(this);
            return;
        }
        Ins = this;
    }

    public void LoadScene(TMScene scene)
    {
        switch (scene)
        {
            case TMScene.MainMenu:
                SceneManager.LoadScene("MainMenu");
                break;

            case TMScene.CurrentLevel:
                SceneManager.LoadScene(GameManager.Ins.currentLevel);
                break;

            case TMScene.NextLevel:
            {
                int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
                GameManager.Ins.currentLevel = nextScene;

                SceneManager.LoadScene(nextScene);
                break;
            }

        }
    }

    public void LoadScene(TMScene scene, TMTransition transition)
    {
        if (transition == TMTransition.None)
        {
            LoadScene(scene);
            return;
        }
        CurrentTransition = transition;

        if (CurrentTransition == TMTransition.LensCircle)
        {
            StartCoroutine(Transitionate(scene, lensCircle.TransitionIn, lensCircle.TransitionOut));
        }
    }

    IEnumerator Transitionate(TMScene scene, Func<float> transitionerIn, Func<float> transitionerOut)
    {
        RefreshValues();

        // Fade out song and start transition animation
        float transitionInDuration = transitionerIn();

        AudioSource source = MusicPlayer.Ins.audioSource;
        source.DOFade(0, transitionInDuration).SetEase(Ease.InSine);

        yield return new WaitForSeconds(transitionInDuration);

        // Disable camera rendering and load desired scene
        mainCamera.cullingMask = 0;
        LoadScene(scene);

        yield return new WaitUntil(() => hasJustChangedScene);
        hasJustChangedScene = false;

        RefreshValues();

        // Fade in song and end transition animation
        mainCamera.cullingMask = -1;
        float transitionOutDuration = transitionerOut();
        source.volume = 0.5f;

        yield return new WaitForSeconds(transitionOutDuration);
        CurrentTransition = TMTransition.None;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoadNewScene;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded += OnLoadNewScene;
    }

    private void RefreshValues()
    {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        lensCircle = GameObject.FindWithTag("LensCircle").GetComponent<LensCircleController>();
    }

    private void OnLoadNewScene(Scene scene, LoadSceneMode sceneMode)
    {
        RefreshValues();

        if (CurrentTransition != TMTransition.None)
        {
            hasJustChangedScene = true;
        }
    }
}
