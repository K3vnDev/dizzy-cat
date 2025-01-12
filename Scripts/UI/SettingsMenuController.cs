using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] GameObject eventFirstSelected, settingsButton;

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

    BackgroundController backgroundController;
    float nextSliderSound = 0;

    private void Start()
    {
        ChangeMusicVolume(MusicPlayer.Ins.currentVolume);
        ChangeSFXVolume(SFXPlayer.Ins.currentVolume);

        fullscreen_toggle.isOn = GameManager.Ins.fsActive;
        backgroundController = GameObject.FindWithTag("Background").GetComponent<BackgroundController>();
    }

    private void OnEnable()
    {
        NavigationSystem.Ins.Select(eventFirstSelected);
    }

    public void OkayButton()
    {
        backgroundController.SetIsOnGrayBackground(false);
        SFXPlayer.Ins.PlaySound(SFXPlayer.ButtonSound.Exit, .1f);
        SwapMenuManager.Ins.ToMain();

        NavigationSystem.Ins.ClearSelected();
        NavigationSystem.Ins.Select(settingsButton);
    }

    public void ChangeMusicVolume(float vol)
    {
        MusicPlayer.Ins.SetVolume(vol);
        music_voltext.text = vol.ToString();

        MusicPlayer.Ins.currentVolume = vol;
        PlaySliderSound();

        music_slider.value = vol;
    }

    public void ChangeSFXVolume(float vol)
    {
        SFXPlayer.Ins.SetVolume(vol);
        sfx_voltext.text = vol.ToString();

        PlaySliderSound();

        sfx_slider.value = vol;
    }

    private void PlaySliderSound()
    {
        if (nextSliderSound < Time.time)
        {
            SFXPlayer.Ins.PlaySound(sliderSound, .1f);
            nextSliderSound = Time.time + sliderSoundCoolDown;
        }
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        SFXPlayer.Ins.PlaySound(
            fullscreen ? fs_sound1 : fs_sound2, .1f);

        GameManager.Ins.fsActive = fullscreen;
    }
}
