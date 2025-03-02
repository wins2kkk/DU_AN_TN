using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private AudioManager audioManager;

    private void Start()
    {
        //// Load giá trị mặc định từ AudioSettingsData
        //musicSlider.value = audioManager.audioSettings.musicVolume;
        //sfxSlider.value = audioManager.audioSettings.sfxVolume;

        //// Gán sự kiện thay đổi giá trị
        //musicSlider.onValueChanged.AddListener(audioManager.SetMusicVolume);
        //sfxSlider.onValueChanged.AddListener(audioManager.SetSFXVolume);
    }
}
