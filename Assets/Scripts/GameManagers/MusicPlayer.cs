using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer I;
    [HideInInspector] public AudioSource audioSource;

    [Range (0, 100)] public int currentVolume = 70;
    public AudioMixer audioMixer;

    [Header ("Songs")]
    [SerializeField] AudioClip menuSong;
    [SerializeField] AudioClip gameplaySong;

    [Header ("Low Volume")]
    [SerializeField][Range (0, 0.5f)] float lowVolume = 0.15f;
    [SerializeField] float volumeFadeTime = 0.3f;

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

    public void LowSourceVolume()
    {
        SetSourceVolume(lowVolume, volumeFadeTime, Ease.OutQuart);
    }

    public void DefaultSourceVolume()
    {
        SetSourceVolume(0.5f, volumeFadeTime, Ease.OutQuart);
    }

    public void SetSourceVolume(float value, float time = 0, Ease easing = Ease.Linear)
    {
        float newVolume = Mathf.Clamp01(value);

        if (time <= 0)
        {
            audioSource.volume = newVolume;
            return;
        }
        audioSource.DOFade(newVolume, time).SetEase(easing).SetUpdate(true);
    }

    public void SetVolume(int value)
    {
        currentVolume = value;
        float newVolume = Utils.ParseVolume(value);
        audioMixer.SetFloat("Volume", newVolume);
    }

    /// <summary>Sets the AudioSource with the assigned music for the current scene.</summary>
    public void Refresh(bool restartOnDifferentAudioClip = true)
    {
        AudioClip newClip = GetSceneMusic();
        bool onDifferentAudioClip = audioSource.clip != newClip;

        DefaultSourceVolume();
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
