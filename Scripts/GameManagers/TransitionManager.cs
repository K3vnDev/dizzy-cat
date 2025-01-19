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
                //Cursor.visible = false;
                break;

            case TMScene.NextLevel:
            {
                GameManager.I.currentLevel++;
                //Cursor.visible = false;

                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                SceneManager.LoadScene(nextSceneIndex);
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

        MusicPlayer.I.SetSourceVolume(0, transitionInDuration, Ease.OutCubic);

        yield return new WaitForSecondsRealtime(transitionInDuration);

        // Disable camera rendering and load desired scene
        mainCamera.enabled = false;
        LoadScene(scene);

        yield return new WaitUntil(() => hasJustChangedScene);
        hasJustChangedScene = false;

        // Fade in song and end transition animation
        float transitionOutDuration = transitionerOut();

        yield return new WaitForSecondsRealtime(transitionOutDuration);

        CurrentTransition = TMTransition.None;
        IsTransitioning = false;
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

        Time.timeScale = 1f;
        MusicPlayer.I.Refresh();

        if (CurrentTransition != TMTransition.None)
        {
            hasJustChangedScene = true;
            mainCamera.enabled = true;
        }
    }
}
