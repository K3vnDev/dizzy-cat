using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer I;
    [HideInInspector] public AudioSource audioSource;

    public AudioMixer audioMixer;
    [SerializeField] AnimationCurve curve;

    public float currentVolume = 70;
    readonly float LOW_VOLUME = 0.15f;

    [SerializeField] AudioClip menuSong, gameplaySong;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(this);
            return;
        }
        I = this;

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetVolume(currentVolume);
        Refresh();
    }

    public void Restart() => audioSource.Play();

    public void LowVolume()
    {
        audioSource.volume = LOW_VOLUME;
    }

    public void DefaultVolume()
    {
        audioSource.volume = 0.5f;
    }

    public void SetVolume(float value)
    {
        currentVolume = value;
        float newVolume = Utils.ParseVolume(value, curve);
        audioMixer.SetFloat("Volume", newVolume);
    }

    /// <summary>Sets the AudioSource with the assigned music for the current scene.</summary>
    public void Refresh(bool restartOnDifferentAudioClip = true)
    {
        AudioClip newClip = GetSceneMusic();
        bool onDifferentAudioClip = audioSource.clip != newClip;

        DefaultVolume();
        audioSource.clip = newClip;

        if (restartOnDifferentAudioClip && onDifferentAudioClip)
        {
            audioSource.Play();
        }
    }

    AudioClip GetSceneMusic()
    {
        bool isOnMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
        return isOnMainMenu ? menuSong : gameplaySong;
    }
}
