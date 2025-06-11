using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] NavigationTarget firstSelected, settingsButton;

    [Header ("SFX")]
    [SerializeField] AudioClip sliderSound;
    [SerializeField] AudioClip fs_sound1;
    [SerializeField] AudioClip fs_sound2;
    [SerializeField] float sliderSoundCoolDown;

    [Header("Texts")]
    [SerializeField] TextMeshProUGUI music_voltext;
    [SerializeField] TextMeshProUGUI sfx_voltext;

    [Header("Sliders & Toggable")]
    [SerializeField] Slider music_slider;
    [SerializeField] Slider sfx_slider;
    [SerializeField] Toggle fullscreen_toggle;

    float nextSliderSound = 0;

    private void Start()
    {
        ChangeMusicVolume(MusicPlayer.I.currentVolume);
        ChangeSFXVolume(SFXPlayer.I.currentVolume);

        bool fullscreen = Screen.fullScreen;

        GameManager.I.IsOnFullscreen = fullscreen;
        fullscreen_toggle.isOn = fullscreen;
    }

    private void Awake()
    {
        NavigationSystem.I.Initialize(gameObject, firstSelected);
    }

    private void OnEnable()
    {
        NavigationSystem.I.Refresh(gameObject, firstSelected);
    }

    public void OkayButton()
    {
        SFXPlayer.I.PlaySound(SFXPlayer.Sound.Exit, 0.1f);
        NavigationSystem.I.Select(settingsButton);

        SwapMenuManager.I.SwapMenu(SwapMenuManager.Menu.Main);
    }

    public void ChangeMusicVolume(float vol)
    {
        MusicPlayer.I.SetVolume((int) vol);
        music_voltext.text = vol.ToString();

        PlaySliderSound();

        music_slider.value = vol;
    }

    public void ChangeSFXVolume(float vol)
    {
        SFXPlayer.I.SetVolume((int) vol);
        sfx_voltext.text = vol.ToString();

        PlaySliderSound();

        sfx_slider.value = vol;
    }

    private void PlaySliderSound()
    {
        if (nextSliderSound < Time.time)
        {
            SFXPlayer.I.PlaySound(sliderSound, .1f);
            nextSliderSound = Time.time + sliderSoundCoolDown;
        }
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        GameManager.I.IsOnFullscreen = fullscreen;

        AudioClip sfx = fullscreen ? fs_sound1 : fs_sound2;
        SFXPlayer.I.PlaySound(sfx, .1f);
    }
}
