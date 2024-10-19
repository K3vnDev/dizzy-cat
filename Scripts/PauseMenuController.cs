using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = GameObject.FindWithTag("Player") 
            .GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) ResumeButton();
    }

    public void ResumeButton()
    {
        this.gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("Player")
            .GetComponent<PlayerController>().gameIsPaused = false;

        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("enter"), .1f);
        playerController.gameIsPaused = false;
        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    public void MainMenuButton()
    {
        SFXPlayerSingleton.Instance.PlaySound(
            SFXPlayerSingleton.Instance.GetButtonSound("exit"), .1f);

        playerController.gameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
