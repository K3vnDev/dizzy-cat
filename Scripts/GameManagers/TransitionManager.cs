using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Collections;
using System;

public enum TMScene { MainMenu, CurrentLevel, NextLevel }
public enum TMTransition { LensCircle, None }

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager I;
    public TMTransition CurrentTransition { get; private set; } = TMTransition.None;
    public bool IsTransitioning { get; private set; } = false;

    Camera mainCamera;
    LensCircleController lensCircle;

    bool hasJustChangedScene = false;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        I = this;
    }

    public void LoadScene(TMScene scene)
    {
        switch (scene)
        {
            case TMScene.MainMenu:
                SceneManager.LoadScene("MainMenu");
                break;

            case TMScene.CurrentLevel:
                SceneManager.LoadScene(GameManager.I.currentLevel);
                break;

            case TMScene.NextLevel:
            {
                int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
                GameManager.I.currentLevel = nextScene;

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
        IsTransitioning = true;

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

        AudioSource source = MusicPlayer.I.audioSource;
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

        MusicPlayer.I.Restart();
        source.volume = 0.5f;

        yield return new WaitForSeconds(transitionOutDuration);

        CurrentTransition = TMTransition.None;
        IsTransitioning = false;

        RefreshValues();
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

        PauseMenuController pauseMenu = GameObject
            .FindWithTag("Pause Menu")?.GetComponent<PauseMenuController>();

        if ( pauseMenu != null ) pauseMenu.gameCanBePaused = !IsTransitioning;
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
