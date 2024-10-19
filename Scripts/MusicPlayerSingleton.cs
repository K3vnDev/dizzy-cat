using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayerSingleton : MonoBehaviour
{
    public static MusicPlayerSingleton Instance;
    public AudioSource audioSource;
    public AudioMixer audioMixer;
    public float audioMixerVol = 70;
    public AnimationCurve curve;

    private void Awake()
    {
        if (MusicPlayerSingleton.Instance == null)
        {
            MusicPlayerSingleton.Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Update()
    {
        if (GameObject.FindWithTag("Player") != null)
        {
            PlayerController playerController = GameObject.FindWithTag("Player")
                .GetComponent<PlayerController>();

            float mainVolume = (curve.Evaluate(audioMixerVol / 100) * 80) - 80;

            audioMixer.SetFloat("Volume", playerController.gameIsPaused ?
                mainVolume - 15f: mainVolume);
        }
    }
}
