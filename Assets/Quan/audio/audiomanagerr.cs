using UnityEngine;

public class audiomanagerrr : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Settings")]
    public AudioSettingsData audioSettings;

    private void Start()
    {
        ApplySettings();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip, audioSettings.sfxVolume);
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.volume = audioSettings.musicVolume;
        musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        audioSettings.musicVolume = volume;
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        audioSettings.sfxVolume = volume;
    }

    private void ApplySettings()
    {
        musicSource.volume = audioSettings.musicVolume;
        sfxSource.volume = audioSettings.sfxVolume;
    }
}
